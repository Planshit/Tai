using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    /// <summary>
    /// 日期变更观察（用于监听日期变化
    /// </summary>
    public interface IDateObserver
    {
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 日期即将变化时发生（前30秒）
        /// </summary>
        event EventHandler OnDateChanging;
    }
}
