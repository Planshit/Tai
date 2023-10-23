using Core.Librarys.SQLite;
using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data.Entity;
using System.Windows.Threading;
using Core.Models.Data;
using Core.Librarys;
using Npoi.Mapper;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace Core.Servicers.Instances
{
    public class Data : IData
    {
        private readonly IAppData _appData;
        private readonly IDatabase _database;
        private readonly object setLock = new object();
        public Data(IAppData appData_, IDatabase database_)
        {
            _appData = appData_;
            _database = database_;
        }

        public void UpdateAppDuration(string process_, int duration_, DateTime startTime_)
        {
            //  过滤无效值
            if (string.IsNullOrEmpty(process_) || duration_ <= 0 || startTime_ == DateTime.MinValue) return;

            Logger.Info($"UpdateAppDuration,process:{process_},duration:{duration_},start:{startTime_}");
            //  开始时间剩余最大统计时长
            int startTimeMaxHoursDuration = (59 - startTime_.Minute) * 60 + (60 - startTime_.Second);
            //  开始时间使用时长
            int startTimeHoursDuration = duration_ > startTimeMaxHoursDuration ? startTimeMaxHoursDuration : duration_;
            //  剩余时长
            int outHoursDuration = duration_ - startTimeHoursDuration;
            //  结束时间
            DateTime endTime = new DateTime(startTime_.Year, startTime_.Month, startTime_.Day, startTime_.Hour, 0, 0);
            //  时段使用数据
            Dictionary<DateTime, int> durationHoursData = new Dictionary<DateTime, int>
            {
                { endTime, startTimeHoursDuration }
            };
            //  计算时段数据
            if (outHoursDuration > 0)
            {
                int outHours = outHoursDuration / 3600;

                DateTime outStartTime = new DateTime(startTime_.Year, startTime_.Month, startTime_.Day, startTime_.Hour, 0, 0);
                for (int i = 0; i < outHours; i++)
                {
                    outStartTime = outStartTime.AddHours(1);
                    int duration = 3600;
                    durationHoursData.Add(outStartTime, duration);
                }
                if (outHoursDuration % 3600 > 0)
                {
                    outStartTime = outStartTime.AddHours(1);
                    int duration = outHoursDuration - outHours * 3600;
                    durationHoursData.Add(outStartTime, duration);
                }
                endTime = outStartTime;
            }

            //  计算每日统计数据
            DateTime nextDayTime = new DateTime(startTime_.Year, startTime_.Month, startTime_.Day, 0, 0, 0).AddDays(1);
            int startTimeMaxDayDuration = (int)(nextDayTime - startTime_).TotalSeconds;
            //  开始时间使用时长
            int startTimeDayDuration = duration_ > startTimeMaxDayDuration ? startTimeMaxDayDuration : duration_;
            //  剩余时长
            int outDayDuration = duration_ - startTimeDayDuration;
            //  结束时间
            DateTime endDayTime = startTime_.Date;
            //  每日使用数据
            Dictionary<DateTime, int> durationDayData = new Dictionary<DateTime, int>
            {
                { startTime_.Date, startTimeDayDuration }
            };
            if (outDayDuration > 0)
            {
                int outDays = outDayDuration / 86400;

                DateTime outStartTime = new DateTime(startTime_.Year, startTime_.Month, startTime_.Day, 0, 0, 0);
                for (int i = 0; i < outDays; i++)
                {
                    outStartTime = outStartTime.AddDays(1);
                    int duration = 86400;
                    durationDayData.Add(outStartTime, duration);
                }
                if (outDayDuration % 86400 > 0)
                {
                    outStartTime = outStartTime.AddDays(1);
                    int duration = outDayDuration - outDays * 86400;
                    durationDayData.Add(outStartTime, duration);
                }
                endDayTime = outStartTime.Date;
            }

            //  开始写入数据
            try
            {
                using (var db = _database.GetWriterContext())
                {
                    var app = db.App.Where(m => m.Name == process_).FirstOrDefault();
                    if (app == null)
                    {
                        _database.CloseWriter();
                        return;
                    }

                    //  更新app累计总时长
                    app.TotalTime += duration_;

                    //  更新每日数据
                    var dailyLogs = db.DailyLog.Where(m => m.Date >= startTime_.Date && m.Date <= endDayTime && m.AppModelID == app.ID).ToList();
                    foreach (var item in durationDayData)
                    {
                        var log = dailyLogs.Where(m => m.Date == item.Key).FirstOrDefault();
                        if (log == null)
                        {
                            //数据库中没有时则创建
                            db.DailyLog.Add(new DailyLogModel()
                            {
                                Date = item.Key,
                                AppModelID = app.ID,
                                Time = item.Value,
                            });
                        }
                        else
                        {
                            int time = log.Time + item.Value;
                            log.Time = time > 86400 ? 86400 : time;
                        }
                    }
                    //  更新时段数据
                    var startDataTime = new DateTime(startTime_.Year, startTime_.Month, startTime_.Day, startTime_.Hour, 0, 0);
                    var hoursLogs = db.HoursLog.Where(m => m.DataTime >= startDataTime && startDataTime <= endTime && m.AppModelID == app.ID).ToList();
                    foreach (var item in durationHoursData)
                    {
                        var log = hoursLogs.Where(m => startDataTime == item.Key).FirstOrDefault();
                        if (log == null)
                        {
                            //  没有记录时创建
                            db.HoursLog.Add(new HoursLogModel()
                            {
                                DataTime = item.Key,
                                AppModelID = app.ID,
                                Time = item.Value
                            });
                        }
                        else
                        {
                            int time = log.Time + item.Value;
                            log.Time = time > 3600 ? 3600 : time;
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UpdateAppDuration error!Process:{process_},Duration:{duration_},StartDateTime:{startTime_}.\r\nError:\r\n{e.Message}");
            }
            finally
            {
                _database.CloseWriter();
            }
        }

        public List<DailyLogModel> GetTodaylogList()
        {
            using (var db = _database.GetReaderContext())
            {
                var today = DateTime.Now.Date;
                var res = db.DailyLog.Where(m => m.Date == today && m.AppModelID != 0);
                return res.ToList();
            }
        }

        public IEnumerable<DailyLogModel> GetDateRangelogList(DateTime start, DateTime end, int take = -1, int skip = -1)
        {

            IEnumerable<AppModel> apps = _appData.GetAllApps();

            using (var db = _database.GetReaderContext())
            {
                var data = db.DailyLog
                .Where(m => m.Date >= start && m.Date <= end && m.AppModelID != 0)
                .GroupBy(m => m.AppModelID)
                .Select(m => new
                {
                    Time = m.Sum(a => a.Time),
                    Date = m.FirstOrDefault().Date,
                    AppID = m.FirstOrDefault().AppModelID
                });
                if (skip > 0 && take > 0)
                {
                    data = data.OrderByDescending(m => m.Time).Skip(skip).Take(take);
                }
                else if (skip > 0)
                {
                    data = data.OrderByDescending(m => m.Time).Skip(skip);
                }
                else if (take > 0)
                {
                    data = data.OrderByDescending(m => m.Time).Take(take);
                }
                else
                {
                    data = data.OrderByDescending(m => m.Time);
                }



                //: db.DailyLog
                //.Where(m => m.Date >= start && m.Date <= end && m.AppModelID != 0)
                //.GroupBy(m => m.AppModelID)
                //.Select(m => new
                //{
                //    Time = m.Sum(a => a.Time),
                //    Date = m.FirstOrDefault().Date,
                //    AppID = m.FirstOrDefault().AppModelID
                //})
                //.OrderByDescending(m => m.Time)
                //.Take(take);

                var res = data
                .ToList()
                .Select(m => new DailyLogModel
                {
                    Time = m.Time,
                    Date = m.Date,
                    AppModelID = m.AppID
                }).ToList();

                foreach (var log in res)
                {
                    log.AppModel = _appData.GetApp(log.AppModelID);
                }

                return res;
            }
        }

        public IEnumerable<DailyLogModel> GetThisWeeklogList()
        {
            DateTime weekStartDate = DateTime.Now, weekEndDate = DateTime.Now;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                weekStartDate = DateTime.Now.Date;
                weekEndDate = DateTime.Now.Date.AddDays(6);
            }
            else
            {
                int weekNum = (int)DateTime.Now.DayOfWeek;
                if (weekNum == 0)
                {
                    weekNum = 7;
                }
                weekNum -= 1;
                weekStartDate = DateTime.Now.Date.AddDays(-weekNum);
                weekEndDate = weekStartDate.Date.AddDays(6);
            }

            return GetDateRangelogList(weekStartDate, weekEndDate);
        }

        public IEnumerable<DailyLogModel> GetLastWeeklogList()
        {
            DateTime weekStartDate = DateTime.Now, weekEndDate = DateTime.Now;

            int weekNum = (int)DateTime.Now.DayOfWeek;
            if (weekNum == 0)
            {
                weekNum = 7;
            }


            weekStartDate = DateTime.Now.Date.AddDays(-6 - weekNum);
            weekEndDate = weekStartDate.AddDays(6);

            return GetDateRangelogList(weekStartDate, weekEndDate);
        }

        public List<DailyLogModel> GetProcessMonthLogList(int appID, DateTime month)
        {
            //var app = appData.GetApp(processName);
            //if (app == null)
            //{
            //    return new List<DailyLogModel>();
            //}
            using (var db = _database.GetReaderContext())
            {

                var res = db.DailyLog.Include(m => m.AppModel).Where(
                m =>
                m.Date.Year == month.Year
                && m.Date.Month == month.Month
                && m.AppModelID == appID
                );
                if (res != null)
                {
                    return res.ToList();
                }
                return new List<DailyLogModel>();
            }
        }


        public void Clear(int appID, DateTime month)
        {

            using (var db = _database.GetReaderContext())
            {

                db.DailyLog.RemoveRange(
                db.DailyLog.Where(m =>
                m.AppModelID == appID
                && m.Date.Year == month.Year
                && m.Date.Month == month.Month));

                db.HoursLog.RemoveRange(
                    db.HoursLog.Where(m => m.AppModelID == appID
                    && m.DataTime.Year == month.Year
                    && m.DataTime.Month == month.Month));

                db.SaveChanges();
            }
        }

        public DailyLogModel GetProcess(int appID, DateTime day)
        {


            using (var db = _database.GetReaderContext())
            {


                var res = db.DailyLog.Where(m =>
            m.AppModelID == appID
            && m.Date.Year == day.Year
            && m.Date.Month == day.Month
            && m.Date.Day == day.Day);
                if (res != null)
                {
                    return res.FirstOrDefault();
                }
                return null;
            }
        }

        public struct CategoryHoursDataModel
        {
            public int Total { get; set; }
            public int CategoryID { get; set; }
            public DateTime Time { get; set; }

        }

        public List<ColumnDataModel> GetCategoryHoursData(DateTime date)
        {
            using (var db = _database.GetReaderContext())
            {
                //  查出有数据的分类

                //var categorys = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,HoursLogModels.DataTime as Time from HoursLogModels join AppModels on AppModels.ID=HoursLogModels.AppModelID where AppModels.CategoryID<>0 and HoursLogModels.DataTime>='" + date.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and HoursLogModels.DataTime<= '" + date.Date.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY AppModels.CategoryID ").ToArray();


                //var data = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,HoursLogModels.DataTime as Time from HoursLogModels join AppModels on AppModels.ID=HoursLogModels.AppModelID where AppModels.CategoryID<>0 and HoursLogModels.DataTime>='" + date.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and HoursLogModels.DataTime<= '" + date.Date.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY AppModels.CategoryID,HoursLogModels.DataTime ").ToArray();
                var categorys = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,HoursLogModels.DataTime as Time from HoursLogModels join AppModels on AppModels.ID=HoursLogModels.AppModelID where  HoursLogModels.DataTime>='" + date.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and HoursLogModels.DataTime<= '" + date.Date.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY AppModels.CategoryID ").OrderByDescending(m => m.CategoryID).ToArray();


                var data = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,HoursLogModels.DataTime as Time from HoursLogModels join AppModels on AppModels.ID=HoursLogModels.AppModelID where  HoursLogModels.DataTime>='" + date.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and HoursLogModels.DataTime<= '" + date.Date.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY AppModels.CategoryID,HoursLogModels.DataTime ").ToArray();

                List<ColumnDataModel> list = new List<ColumnDataModel>();
                foreach (var category in categorys)
                {
                    list.Add(new ColumnDataModel()
                    {
                        CategoryID = category.CategoryID,
                        Values = new double[24]
                    });
                }

                for (int i = 0; i < 24; i++)
                {
                    string hours = i < 10 ? "0" + i : i.ToString();
                    var time = date.ToString($"yyyy-MM-dd {hours}:00:00");
                    foreach (var category in categorys)
                    {
                        var log = data.Where(m => m.CategoryID == category.CategoryID && m.Time.ToString("yyyy-MM-dd HH:00:00") == time).FirstOrDefault();

                        var item = list.Where(m => m.CategoryID == category.CategoryID).FirstOrDefault();

                        item.Values[i] = log.Total;
                    }
                }
                return list;
            }
        }

        public List<ColumnDataModel> GetCategoryRangeData(DateTime start, DateTime end)
        {
            using (var db = _database.GetReaderContext())
            {
                //  查出有数据的分类

                //var categorys = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where AppModels.CategoryID<>0 and DailyLogModels.Date>='" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID").ToArray();


                //var data = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where AppModels.CategoryID<>0 and DailyLogModels.Date>='" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID,DailyLogModels.Date ").ToArray();
                var categorys = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where  DailyLogModels.Date>='" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID").OrderByDescending(m => m.CategoryID).ToArray();


                var data = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where  DailyLogModels.Date>='" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID,DailyLogModels.Date ").ToArray();

                var ts = end - start;
                var days = ts.TotalDays + 1;


                List<ColumnDataModel> list = new List<ColumnDataModel>();


                foreach (var category in categorys)
                {
                    list.Add(new ColumnDataModel()
                    {
                        CategoryID = category.CategoryID,
                        Values = new double[(int)days]
                    });
                }



                for (int i = 0; i < days; i++)
                {
                    string day = i < 10 ? "0" + i : i.ToString();
                    var time = start.AddDays(i).ToString($"yyyy-MM-dd 00:00:00");
                    Debug.WriteLine(time);
                    foreach (var category in categorys)
                    {
                        var log = data.Where(m => m.CategoryID == category.CategoryID && m.Time.ToString("yyyy-MM-dd 00:00:00") == time).FirstOrDefault();

                        var item = list.Where(m => m.CategoryID == category.CategoryID).FirstOrDefault();

                        item.Values[i] = log.Total;
                    }
                }
                return list;
            }
        }

        public List<ColumnDataModel> GetCategoryYearData(DateTime date)
        {
            using (var db = _database.GetReaderContext())
            {
                //  查出有数据的分类

                var dateArr = Time.GetYearDate(date);

                //var categorys = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where AppModels.CategoryID<>0 and DailyLogModels.Date>='" + dateArr[0].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + dateArr[1].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID").ToArray();


                //var data = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where AppModels.CategoryID<>0 and DailyLogModels.Date>='" + dateArr[0].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + dateArr[1].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID,DailyLogModels.Date ").ToArray();

                var categorys = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where  DailyLogModels.Date>='" + dateArr[0].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + dateArr[1].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID").OrderByDescending(m => m.CategoryID).ToArray();


                var data = db.Database.SqlQuery<CategoryHoursDataModel>("select sum(Time) as Total,AppModels.CategoryID,DailyLogModels.Date as Time from DailyLogModels join AppModels on AppModels.ID=DailyLogModels.AppModelID where DailyLogModels.Date>='" + dateArr[0].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DailyLogModels.Date<= '" + dateArr[1].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModels.CategoryID,DailyLogModels.Date ").ToArray();
                //var ts = end - start;
                //var days = ts.TotalDays + 1;


                List<ColumnDataModel> list = new List<ColumnDataModel>();


                foreach (var category in categorys)
                {
                    list.Add(new ColumnDataModel()
                    {
                        CategoryID = category.CategoryID,
                        Values = new double[12]
                    });
                }



                for (int i = 1; i < 13; i++)
                {
                    string month = i < 10 ? "0" + i : i.ToString();
                    var dayArr = Time.GetMonthDate(new DateTime(date.Year, i, 1));

                    Debug.WriteLine(dayArr);
                    foreach (var category in categorys)
                    {
                        var total = data.Where(m => m.CategoryID == category.CategoryID && m.Time >= dayArr[0] && m.Time <= dayArr[1]).Sum(m => m.Total);

                        var item = list.Where(m => m.CategoryID == category.CategoryID).FirstOrDefault();

                        item.Values[i - 1] = total;
                    }
                }
                return list;
            }
        }

        public struct ColumnItemDataModel
        {
            public int Total { get; set; }
            public int AppID { get; set; }
            public DateTime Time { get; set; }

        }

        public List<ColumnDataModel> GetAppDayData(int appID, DateTime date)
        {
            using (var db = _database.GetReaderContext())
            {

                var data = db.Database.SqlQuery<ColumnItemDataModel>("select sum(Time) as Total,AppModelID as AppID,DataTime as Time from HoursLogModels  where AppModelID=" + appID + " and DataTime>='" + date.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and DataTime<= '" + date.Date.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY AppModelID,DataTime ").ToArray();


                List<ColumnDataModel> list = new List<ColumnDataModel>();


                list.Add(new ColumnDataModel()
                {
                    AppId = appID,
                    Values = new double[24]
                });

                var item = list[0];

                for (int i = 0; i < 24; i++)
                {
                    string hours = i < 10 ? "0" + i : i.ToString();
                    var time = date.ToString($"yyyy-MM-dd {hours}:00:00");

                    var log = data.Where(m => m.Time.ToString("yyyy-MM-dd HH:00:00") == time).FirstOrDefault();



                    item.Values[i] = log.Total;
                }
                return list;
            }
        }

        public List<ColumnDataModel> GetAppRangeData(int appID, DateTime start, DateTime end)
        {
            using (var db = _database.GetReaderContext())
            {
                //  查出有数据的分类

                var data = db.Database.SqlQuery<ColumnItemDataModel>("select sum(Time) as Total,AppModelID as AppID,Date as Time from DailyLogModels where AppModelID=" + appID + " and Date>='" + start.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and Date<= '" + end.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY Date ").ToArray();


                var ts = end - start;
                var days = ts.TotalDays + 1;


                List<ColumnDataModel> list = new List<ColumnDataModel>();



                list.Add(new ColumnDataModel()
                {
                    AppId = appID,
                    Values = new double[(int)days]
                });


                var item = list[0];

                for (int i = 0; i < days; i++)
                {
                    string day = i < 10 ? "0" + i : i.ToString();
                    var time = start.AddDays(i).ToString($"yyyy-MM-dd 00:00:00");

                    var log = data.Where(m => m.Time.ToString("yyyy-MM-dd 00:00:00") == time).FirstOrDefault();

                    item.Values[i] = log.Total;
                }
                return list;
            }
        }

        public List<ColumnDataModel> GetAppYearData(int appID, DateTime date)
        {
            using (var db = _database.GetReaderContext())
            {
                //  查出有数据的分类

                var dateArr = Time.GetYearDate(date);


                var data = db.Database.SqlQuery<ColumnItemDataModel>("select sum(Time) as Total,AppModelID as AppID,Date as Time from DailyLogModels  where  AppModelID=" + appID + " and Date>='" + dateArr[0].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and Date<= '" + dateArr[1].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY Date").ToArray();




                List<ColumnDataModel> list = new List<ColumnDataModel>();


                list.Add(new ColumnDataModel()
                {
                    AppId = appID,
                    Values = new double[12]
                });


                var item = list[0];

                for (int i = 1; i < 13; i++)
                {
                    string month = i < 10 ? "0" + i : i.ToString();
                    var dayArr = Time.GetMonthDate(new DateTime(date.Year, i, 1));

                    Debug.WriteLine(dayArr);

                    var total = data.Where(m => m.Time >= dayArr[0] && m.Time <= dayArr[1]).Sum(m => m.Total);


                    item.Values[i - 1] = total;
                }
                return list;
            }
        }

        public void ClearRange(DateTime start, DateTime end)
        {
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));

            using (var db = _database.GetReaderContext())
            {
                db.Database.ExecuteSqlCommand("delete from DailyLogModels  where Date>='" + start.Date.ToString("yyyy-MM-01 00:00:00") + "' and Date<= '" + end.Date.ToString("yyyy-MM-dd 23:59:59") + "'");

                db.Database.ExecuteSqlCommand("delete from HoursLogModels  where DataTime>='" + start.Date.ToString("yyyy-MM-01 00:00:00") + "' and DataTime<= '" + end.Date.ToString("yyyy-MM-dd 23:59:59") + "'");
            }
        }

        public void ExportToExcel(string dir, DateTime start, DateTime end)
        {
            start = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
            end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month), 23, 59, 59);

            using (var db = _database.GetReaderContext())
            {
                var day = db.DailyLog.Where(m => m.Date >= start.Date && m.Date <= end.Date)
                    .ToList()
                    .Select(m => new
                    {
                        日期 = m.Date,
                        应用 = m.AppModel != null ? m.AppModel.Name : "未知",
                        描述 = m.AppModel != null ? m.AppModel.Description : "未知",
                        时长 = m.Time,
                        分类 = m.AppModel != null && m.AppModel.Category != null ? m.AppModel.Category.Name : "未知"
                    });

                var hours = db.HoursLog.Where(m => m.DataTime >= start && m.DataTime <= end)
                    .ToList()
                    .Select(m => new
                    {
                        时段 = m.DataTime,
                        应用 = m.AppModel != null ? m.AppModel.Name : "未知",
                        描述 = m.AppModel != null ? m.AppModel.Description : "未知",
                        时长 = m.Time,
                        分类 = m.AppModel != null && m.AppModel.Category != null ? m.AppModel.Category.Name : "未知"
                    });
                var mapper = new Mapper();
                mapper.Put(day, "每日");
                mapper.Put(hours, "时段");

                string name = $"Tai数据({start.ToString("yyyy年MM月")}-{end.ToString("yyyy年MM月")})";
                if (start.Year == end.Year && start.Month == end.Month)
                {
                    name = $"Tai数据({start.ToString("yyyy年MM月")})";
                }
                mapper.Save(Path.Combine(dir, $"{name}.xlsx"));

                //  导出csv
                using (var writer = new StreamWriter(Path.Combine(dir, $"{name}-每日.csv"), false, System.Text.Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(day);
                }

                using (var writer = new StreamWriter(Path.Combine(dir, $"{name}-时段.csv"), false, System.Text.Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(hours);
                }
            }
        }

        public int GetDateRangeAppCount(DateTime start, DateTime end)
        {
            IEnumerable<AppModel> apps = _appData.GetAllApps();

            using (var db = _database.GetReaderContext())
            {
                var res = db.DailyLog
                .Where(m => m.Date >= start && m.Date <= end && m.AppModelID != 0)
                .GroupBy(m => m.AppModelID)
                .Count();

                return res;
            }
        }

        public IEnumerable<HoursLogModel> GetTimeRangelogList(DateTime time)
        {
            time = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
            using (var db = _database.GetReaderContext())
            {
                var res = db.HoursLog.Where(m => m.DataTime == time).ToList();

                foreach (var log in res)
                {
                    log.AppModel = _appData.GetApp(log.AppModelID);
                }

                return res;
            }
        }

        public struct TimeDataModel
        {
            public int Total { get; set; }
            public DateTime Time { get; set; }
        }
        public double[] GetRangeTotalData(DateTime start, DateTime end)
        {
            using (var db = _database.GetReaderContext())
            {
                if (start.Date == end.Date)
                {
                    //  获取24小时
                    var data = db.Database.SqlQuery<TimeDataModel>("select sum(Time) as Total,DataTime as Time from HoursLogModels where  HoursLogModels.DataTime>='" + start.Date.ToString("yyyy-MM-dd 00:00:00") + "' and HoursLogModels.DataTime<= '" + start.Date.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY DataTime ").ToArray();

                    double[] result = new double[24];
                    for (int i = 0; i < 24; i++)
                    {
                        string hours = i < 10 ? "0" + i : i.ToString();
                        var time = start.ToString($"yyyy-MM-dd {hours}:00:00");
                        var log = data.Where(m => m.Time.ToString("yyyy-MM-dd HH:00:00") == time).FirstOrDefault();
                        result[i] = log.Total;
                    }
                    return result;
                }
                else
                {
                    //  获取日期
                    var ts = end.Date - start.Date;
                    int days = (int)ts.TotalDays + 1;


                    var data = db.Database.SqlQuery<TimeDataModel>("select sum(Time) as Total,Date as Time from DailyLogModels where  DailyLogModels.Date>='" + start.Date.ToString("yyyy-MM-dd 00:00:00") + "' and DailyLogModels.Date<= '" + end.Date.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY Date ").ToArray();

                    double[] result = new double[days];
                    for (int i = 0; i < days; i++)
                    {
                        var time = start.Date.AddDays(i).ToString($"yyyy-MM-dd 00:00:00");
                        var log = data.Where(m => m.Time.ToString("yyyy-MM-dd 00:00:00") == time).FirstOrDefault();
                        result[i] = log.Total;
                    }

                    return result;
                }
            }
        }

        public double[] GetMonthTotalData(DateTime date)
        {
            using (var db = _database.GetReaderContext())
            {
                var dateArr = Time.GetYearDate(date);
                var data = db.Database.SqlQuery<TimeDataModel>("select sum(Time) as Total,Date as Time from DailyLogModels  where   Date>='" + dateArr[0].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' and Date<= '" + dateArr[1].Date.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY Date").ToArray();
                double[] result = new double[12];

                for (int i = 1; i < 13; i++)
                {
                    string month = i < 10 ? "0" + i : i.ToString();
                    var dayArr = Time.GetMonthDate(new DateTime(date.Year, i, 1));
                    var total = data.Where(m => m.Time >= dayArr[0] && m.Time <= dayArr[1]).Sum(m => m.Total);

                    result[i - 1] = total;
                }
                return result;
            }
        }
    }
}
