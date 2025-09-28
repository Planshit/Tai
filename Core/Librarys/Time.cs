using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys
{
    public class Time
    {
        public static string ToHoursString(double seconds)
        {
            double hours = seconds / 60 / 60;

            if (hours > 0.1)
            {
                return hours.ToString("f2");
            }
            else
            {
                return "0";
            }
        }
        
        public static object[] ToString(int seconds)
        {
            if (seconds < 60)
            {
                return new Object[]{"time.seconds", seconds};
            }
            else
            {
                double minutes = (double)seconds / 60;
                if (minutes < 60)
                {
                    if (minutes != (int)minutes)
                    {
                        double pointNumber = minutes - (int)minutes;
                        int seconds_ = (int)(pointNumber * 60);
                        if (seconds_ > 0)
                        {
                            return new Object[]{"time.minutesWithSeconds", (int) minutes, seconds_};
                        }
                    }

                    return new Object[]{"time.minutes", (int) minutes};
                }
                else
                {
                    double hours = (double)minutes / 60;
                    if (hours != (int)hours)
                    {
                        double pointNumber = hours - (int)hours;
                        int minutes_ = (int)(pointNumber * 60);
                        if (minutes_ > 0)
                        {
                            return new Object[]{"time.hoursWithMinutes", (int) hours, minutes_};
                        }
                    }
                    
                    return new Object[]{"time.hours", (int) hours};
                }
            }
        }

        public static DateTime[] GetThisWeekDate()
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
            return new[] { weekStartDate, weekEndDate };
        }

        public static DateTime[] GetLastWeekDate()
        {
            DateTime weekStartDate = DateTime.Now, weekEndDate = DateTime.Now;

            int weekNum = (int)DateTime.Now.DayOfWeek;
            if (weekNum == 0)
            {
                weekNum = 7;
            }


            weekStartDate = DateTime.Now.Date.AddDays(-6 - weekNum);
            weekEndDate = weekStartDate.AddDays(6);

            return new[] { weekStartDate, weekEndDate };
        }

        public static DateTime[] GetMonthDate(DateTime date)
        {
            var dateStart = new DateTime(date.Year, date.Month, 1);
            var dateEnd = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            return new[] { dateStart, dateEnd };
        }

        public static DateTime[] GetYearDate(DateTime date)
        {
            var dateStart = new DateTime(date.Year, 1, 1, 0, 0, 0);
            var dateEnd = new DateTime(date.Year, 12, DateTime.DaysInMonth(date.Year, 12), 23, 59, 59);
            return new[] { dateStart, dateEnd };

        }
    }
}
