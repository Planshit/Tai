using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    /// <summary>
    /// 主要服务
    /// </summary>
    public interface IMain
    {
        /// <summary>
        /// 运行服务（仅首次启动调用）
        /// </summary>
        void Run();
        /// <summary>
        /// 退出服务
        /// </summary>
        void Exit();
        /// <summary>
        /// 启动服务
        /// </summary>
        void Start();
        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();

        /// <summary>
        /// 更新某个进程时间时发生
        /// </summary>
        event EventHandler OnUpdateTime;
        /// <summary>
        /// 初始化完成时发生
        /// </summary>
        event EventHandler OnStarted;
    }
}
