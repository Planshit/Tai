using Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    /// <summary>
    /// 浏览器观察服务
    /// </summary>
    public interface IBrowserObserver
    {
        /// <summary>
        /// 启动观察
        /// </summary>
        void Start();
        /// <summary>
        /// 停止观察
        /// </summary>
        void Stop();

        /// <summary>
        /// 站点切换时发生
        /// </summary>
        event BrowserObserverEventHandler OnSiteChanged;
    }
}
