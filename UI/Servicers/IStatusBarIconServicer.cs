using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Servicers
{
    /// <summary>
    /// 状态栏图标服务
    /// </summary>
    public interface IStatusBarIconServicer
    {
        /// <summary>
        /// 初始化状态栏图标
        /// </summary>
        void Init();
        /// <summary>
        /// 显示主窗口
        /// </summary>
        void ShowMainWindow();
    }
}
