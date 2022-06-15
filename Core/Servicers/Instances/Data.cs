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

namespace Core.Servicers.Instances
{
    public class Data : IData
    {
        private readonly IAppData appData;
        public Data(IAppData appData)
        {
            this.appData = appData;
        }
        public void Set(string processName, string processDescription, string file, int seconds)
        {
            if (string.IsNullOrEmpty(processName) || seconds <= 0)
            {
                return;
            }
            processDescription = processDescription == null ? string.Empty : processDescription;

            AppModel app = appData.GetApp(processName);

            if (app == null)
            {
                return;
            }


            var today = DateTime.Now.Date;
            using (var db = new TaiDbContext())
            {
                var res = db.DailyLog.SingleOrDefault(m => m.Date == today && m.AppModelID == app.ID);
                if (res == null)
                {
                    //数据库中没有时则创建
                    db.DailyLog.Add(new Models.DailyLogModel()
                    {
                        Date = today,
                        AppModelID = app.ID,
                        Time = seconds,
                    });
                }
                else
                {
                    res.Time += seconds;
                }

                //  分时段记录数据

                //  当前时段
                var nowtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                var hourslog = db.HoursLog.SingleOrDefault(
                    m =>
                    m.DataTime == nowtime
                    && m.AppModelID == app.ID
                    );
                if (hourslog == null)
                {
                    //  没有时创建
                    db.HoursLog.Add(new Models.HoursLogModel()
                    {
                        DataTime = nowtime,
                        AppModelID = app.ID,
                        Time = seconds
                    });
                }
                else
                {
                    hourslog.Time += seconds;
                }

                //  统计使用时长
                app.TotalTime += seconds;

                appData.UpdateApp(app);

                db.SaveChanges();
            }
        }

        public List<DailyLogModel> GetTodaylogList()
        {

            //return null;

            using (var db = new TaiDbContext())
            {
                var today = DateTime.Now.Date;
                var res = db.DailyLog.Where(m => m.Date == today && m.AppModelID != 0);
                return res.ToList();
            }
        }

        public IEnumerable<DailyLogModel> GetDateRangelogList(DateTime start, DateTime end, int take = -1)
        {

            IEnumerable<AppModel> apps = appData.GetAllApps();

            using (var db = new TaiDbContext())
            {
                var data = take == -1 ? db.DailyLog
                .Where(m => m.Date >= start && m.Date <= end && m.AppModelID != 0)
                .GroupBy(m => m.AppModelID)
                .Select(m => new
                {
                    Time = m.Sum(a => a.Time),
                    Date = m.FirstOrDefault().Date,
                    AppID = m.FirstOrDefault().AppModelID
                }) : db.DailyLog
                .Where(m => m.Date >= start && m.Date <= end && m.AppModelID != 0)
                .GroupBy(m => m.AppModelID)
                .Select(m => new
                {
                    Time = m.Sum(a => a.Time),
                    Date = m.FirstOrDefault().Date,
                    AppID = m.FirstOrDefault().AppModelID
                })
                .OrderByDescending(m => m.Time)
                .Take(take);

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
                    log.AppModel = appData.GetApp(log.AppModelID);
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

        public void Set(string processName, int seconds)
        {
            if (string.IsNullOrEmpty(processName) || seconds <= 0)
            {
                return;
            }

            AppModel app = appData.GetApp(processName);
            if (app == null)
            {
                return;
            }

            using (var db = new TaiDbContext())
            {
                var today = DateTime.Now.Date;
                var res = db.DailyLog.SingleOrDefault(m => m.Date == today && m.AppModelID == app.ID);
                if (res != null)
                {
                    res.Time += seconds;
                }

                //  分时段记录数据

                //  当前时段
                var nowtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                var hourslog = db.HoursLog.SingleOrDefault(
                    m =>
                    m.DataTime == nowtime
                    && m.AppModelID == app.ID);
                if (hourslog != null)
                {
                    hourslog.Time += seconds;
                }


                //  统计使用时长

                app.TotalTime += seconds;

                appData.UpdateApp(app);


                db.SaveChanges();
            }

        }

        public List<DailyLogModel> GetProcessMonthLogList(int appID, DateTime month)
        {
            //var app = appData.GetApp(processName);
            //if (app == null)
            //{
            //    return new List<DailyLogModel>();
            //}
            using (var db = new TaiDbContext())
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

            using (var db = new TaiDbContext())
            {

                db.DailyLog.RemoveRange(
                db.DailyLog.Where(m =>
                m.AppModelID == appID
                && m.Date.Year == month.Year
                && m.Date.Month == month.Month));
                db.SaveChanges();
            }
        }

        public DailyLogModel GetProcess(int appID, DateTime day)
        {


            using (var db = new TaiDbContext())
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
            using (var db = new TaiDbContext())
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
            using (var db = new TaiDbContext())
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
            using (var db = new TaiDbContext())
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
            using (var db = new TaiDbContext())
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
            using (var db = new TaiDbContext())
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
            using (var db = new TaiDbContext())
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
    }
}
