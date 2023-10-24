using Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface ISleepdiscover
    {
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 休眠状态发生更改
        /// </summary>
        event SleepdiscoverEventHandler SleepStatusChanged;
    }
}
