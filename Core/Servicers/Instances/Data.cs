using Core.Librarys.SQLite;
using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Data.Entity;
using System.Windows.Threading;

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

        public IEnumerable<DailyLogModel> GetDateRangelogList(DateTime start, DateTime end)
        {

            IEnumerable<AppModel> apps = appData.GetAllApps();

            //using (var db = new StatisticContext())
            //{


            //    //var test = db.DailyLog.Include(m=>m.AppModel).Where(m => m.AppModelID != 0).Take(4).ToList();
            //    //var res = db.DailyLog.Include(m => m.AppModel).SqlQuery("Select *,Sum(Time) as Time,DailyLogModels.ID,AppModelID,Date,AppModels.ID,AppModels.Name,AppModels.Description,AppModels.File,AppModels.CategoryID,AppModels.IconFile,AppModels.TotalTime from DailyLogModels  LEFT OUTER JOIN AppModels ON AppModels.ID=DailyLogModels.ID WHERE AppModelID<>0 AND Date between '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + end.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY AppModelID ").ToList();
            using (var db = new TaiDbContext())
            {
                //var res = db.DailyLog
                //.Include(m => m.AppModel)
                //.Where(m => m.Date >= start && m.Date <= end && m.AppModelID != 0)
                //.GroupBy(m => m.AppModelID)
                //.Select(m => new
                //{
                //    Time = m.Sum(a => a.Time),
                //    App = m.FirstOrDefault().AppModel,
                //    Date = m.FirstOrDefault().Date,
                //})
                //.ToList()
                //.Select(m => new DailyLogModel
                //{
                //    Time = m.Time,
                //    AppModel = m.App,
                //    Date = m.Date
                //});

                var res = db.DailyLog
                .Where(m => m.Date >= start && m.Date <= end && m.AppModelID != 0)
                .GroupBy(m => m.AppModelID)
                .Select(m => new
                {
                    Time = m.Sum(a => a.Time),
                    Date = m.FirstOrDefault().Date,
                    AppID = m.FirstOrDefault().AppModelID
                })
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


    }
}
