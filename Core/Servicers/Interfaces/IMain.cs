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
        /// 启动服务
        /// </summary>
        void Run();
        /// <summary>
        /// 退出服务
        /// </summary>
        void Exit();
    }
}
