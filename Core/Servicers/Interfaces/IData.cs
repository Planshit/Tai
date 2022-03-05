using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface IData
    {
        /// <summary>
        /// 设置进程数据
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="processDescription"></param>
        /// <param name="file"></param>
        /// <param name="seconds"></param>
        void Set(string processName, string processDescription, string file, int seconds);

        /// <summary>
        /// 获取今天的数据
        /// </summary>
        /// <returns></returns>
        List<DailyLogModel> GetTodaylogList();
        /// <summary>
        /// 查询指定范围数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<DailyLogModel> GetDateRangelogList(DateTime start, DateTime end);

        /// <summary>
        /// 获取本周的数据
        /// </summary>
        /// <returns></returns>
        List<DailyLogModel> GetThisWeeklogList();
        /// <summary>
        /// 获取上周的数据
        /// </summary>
        /// <returns></returns>
        List<DailyLogModel> GetLastWeeklogList();
        /// <summary>
        /// 设置进程数据（仅通过进程名可能会出现重复的问题
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="seconds"></param>
        void Set(string processName, int seconds);

        /// <summary>
        /// 获取指定进程某个月的数据
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<DailyLogModel> GetProcessMonthLogList(string processName, DateTime month);


        /// <summary>
        /// 清空指定进程某月的数据
        /// </summary>
        /// <param name="processName"></param>
        void Clear(string processName, DateTime month);

        /// <summary>
        /// 获取指定进程某天的数据
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        DailyLogModel GetProcess(string processName, DateTime day);
    }
}
