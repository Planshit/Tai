using Core.Models;
using Core.Models.Data;
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
        /// 保存APP使用时长数据
        /// </summary>
        /// <param name="processName_">进程名称</param>
        /// <param name="duration_">时长（秒）</param>
        /// <param name="startDateTime_">记录开始时间</param>
        void UpdateAppDuration(string processName_, int duration_, DateTime startDateTime_);

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
        IEnumerable<DailyLogModel> GetDateRangelogList(DateTime start, DateTime end, int take = -1, int skip = -1);

        /// <summary>
        /// 获取本周的数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<DailyLogModel> GetThisWeeklogList();
        /// <summary>
        /// 获取上周的数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<DailyLogModel> GetLastWeeklogList();

        /// <summary>
        /// 获取指定进程某个月的数据
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        List<DailyLogModel> GetProcessMonthLogList(int appID, DateTime month);


        /// <summary>
        /// 清空指定进程某月的数据
        /// </summary>
        /// <param name="processName"></param>
        void Clear(int appID, DateTime month);

        /// <summary>
        /// 获取指定进程某天的数据
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        DailyLogModel GetProcess(int appID, DateTime day);

        /// <summary>
        /// 获取指定日期所有分类时段统计数据
        /// </summary>
        /// <returns></returns>
        List<ColumnDataModel> GetCategoryHoursData(DateTime date);
        /// <summary>
        /// 获取指定日期范围所有分类按天统计数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<ColumnDataModel> GetCategoryRangeData(DateTime start, DateTime end);
        /// <summary>
        /// 获取指定年份所有分类按月份统计数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        List<ColumnDataModel> GetCategoryYearData(DateTime date);

        List<ColumnDataModel> GetAppDayData(int appID, DateTime date);
        List<ColumnDataModel> GetAppRangeData(int appID, DateTime start, DateTime end);
        List<ColumnDataModel> GetAppYearData(int appID, DateTime date);
        /// <summary>
        /// 清空指定时间范围数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void ClearRange(DateTime start, DateTime end);
        /// <summary>
        /// 导出数据到EXCEL
        /// </summary>
        void ExportToExcel(string dir, DateTime start, DateTime end);
        /// <summary>
        /// 获取指定日期范围使用应用量
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        int GetDateRangeAppCount(DateTime start, DateTime end);
        /// <summary>
        /// 获取指定时间（小时）所有使用app数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerable<HoursLogModel> GetTimeRangelogList(DateTime time);

        /// <summary>
        /// 获取指定时间范围内的汇总数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        double[] GetRangeTotalData(DateTime start, DateTime end);
        /// <summary>
        /// 获取指定年份按月统计数据
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        double[] GetMonthTotalData(DateTime year);
        /// <summary>
        /// 清空所有统计数据
        /// </summary>
        /// <param name="appID_">应用ID</param>
        void Clear(int appID_);
    }
}
