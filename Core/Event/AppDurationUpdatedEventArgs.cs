using Core.Models.AppObserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Event
{
    public class AppDurationUpdatedEventArgs
    {
        /// <summary>
        /// 使用时长
        /// </summary>
        public int Duration { get; }
        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime ActiveTime { get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; }
        /// <summary>
        /// 应用信息
        /// </summary>
        public AppInfo App { get; }
        /// <summary>
        /// 窗口信息
        /// </summary>
        public WindowInfo Window { get; }
        public AppDurationUpdatedEventArgs(int duration_, AppInfo app_, WindowInfo window_, DateTime activeTime_, DateTime endTime_)
        {
            Duration = duration_;
            App = app_;
            Window = window_;
            ActiveTime = activeTime_;
            EndTime = endTime_;
        }

        public override string ToString()
        {
            return $"Duration:{Duration},App:{App},Window:{Window},ActiveTime:{ActiveTime},EndTime:{EndTime}";

        }
    }
}
