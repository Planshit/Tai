using Core.Models.AppObserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Event
{
    /// <summary>
    /// 应用活跃状态变更事件参数
    /// </summary>
    public class AppActiveChangedEventArgs
    {
        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime ActiveTime { get; }
        /// <summary>
        /// 应用信息
        /// </summary>
        public AppInfo App { get; }
        /// <summary>
        /// 窗口信息
        /// </summary>
        public WindowInfo Window { get; }
        public AppActiveChangedEventArgs(AppInfo app_, WindowInfo window_, DateTime activeTime_)
        {
            App = app_;
            Window = window_;
            ActiveTime = activeTime_;
        }
    }
}
