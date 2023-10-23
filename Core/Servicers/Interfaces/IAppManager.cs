using Core.Models.AppObserver;
using NPOI.XSSF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface IAppManager
    {
        /// <summary>
        /// 通过窗口句柄获取应用信息
        /// </summary>
        /// <param name="hwnd_">窗口句柄</param>
        /// <returns></returns>
        AppInfo GetAppInfo(IntPtr hwnd_);
    }
}
