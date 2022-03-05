using Core.Librarys.SQLite;
using Core.Models;
using Core.Models.Config;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class Data : IData
    {
        public void Set(string processName, string processDescription, string file, int seconds)
        {
            if (string.IsNullOrEmpty(processName) || seconds <= 0)
            {
                return;
            }
            processDescription = processDescription == null ? string.Empty : processDescription;
            using (var db = new StatisticContext())
            {
                var today = DateTime.Now.Date;
                var res = db.DailyLog.SingleOrDefault(m => m.Date == today && m.ProcessName == processName);
                if (res == null)
                {
                    //数据库中没有时则创建
                    db.DailyLog.Add(new Models.DailyLogModel()
                    {
                        Date = today,
                        File = file,
                        ProcessDescription = processDescription,
                        ProcessName = processName,
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
                    && m.ProcessName == processName
                    );
                if (hourslog == null)
                {
                    //  没有时创建
                    db.HoursLog.Add(new Models.HoursLogModel()
                    {
                        DataTime = nowtime,
                        ProcessName = processName,
                        File = file,
                        Time = seconds
                    });
                }
                else
                {
                    hourslog.Time += seconds;
                }

                db.SaveChanges();
            }
        }

        public List<DailyLogModel> GetTodaylogList()
        {
            using (var db = new StatisticContext())
            {
                var today = DateTime.Now.Date;
                var res = db.DailyLog.Where(m => m.Date == today);
                return res.ToList();
            }
        }

        public List<DailyLogModel> GetDateRangelogList(DateTime start, DateTime end)
        {

            using (var db = new StatisticContext())
            {
                var today = DateTime.Now.Date;
                var res = db.DailyLog.SqlQuery("Select Sum(Time) as Time,ProcessName,File,ID,Date,ProcessDescription from DailyLogModels WHERE Date between '" + start.ToString("yyyy-MM-dd HH:mm:ss") + "' AND '" + end.ToString("yyyy-MM-dd HH:mm:ss") + "' GROUP BY ProcessName ").ToList();
                return res.ToList();
            }
        }

        public List<DailyLogModel> GetThisWeeklogList()
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

        public List<DailyLogModel> GetLastWeeklogList()
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
            using (var db = new StatisticContext())
            {
                var today = DateTime.Now.Date;
                var res = db.DailyLog.SingleOrDefault(m => m.Date == today && m.ProcessName == processName);
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
                    && m.ProcessName == processName);
                if (hourslog != null)
                {
                    hourslog.Time += seconds;
                }
                db.SaveChanges();

            }
        }

        public List<DailyLogModel> GetProcessMonthLogList(string processName, DateTime month)
        {
            using (var db = new StatisticContext())
            {

                var res = db.DailyLog.Where(
                    m =>
                    m.Date.Year == month.Year
                    && m.Date.Month == month.Month
                    && m.ProcessName == processName
                    //&& m.ProcessDescription == processDescription
                    //&& m.File == file
                    );
                if (res != null)
                {
                    return res.ToList();
                }
                return new List<DailyLogModel>();
            }
        }


        public void Clear(string processName, DateTime month)
        {
            if (string.IsNullOrEmpty(processName))
            {
                return;
            }
            using (var db = new StatisticContext())
            {

                db.DailyLog.RemoveRange(
                    db.DailyLog.Where(m =>
                    m.ProcessName == processName
                    && m.Date.Year == month.Year
                    && m.Date.Month == month.Month));
                db.SaveChanges();
            }
        }

        public DailyLogModel GetProcess(string processName, DateTime day)
        {
            using (var db = new StatisticContext())
            {


                var res = db.DailyLog.Where(m =>
                m.ProcessName == processName
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
