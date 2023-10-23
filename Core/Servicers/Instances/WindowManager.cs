using Core.Librarys;
using Core.Models.AppObserver;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class WindowManager : IWindowManager
    {

        public WindowInfo GetWindowInfo(IntPtr handle_)
        {
            try
            {
                string title = GetWindowTitle(handle_);
                var rect = Win32API.GetWindowRect(handle_);
                string className = Win32API.GetWindowClassName(handle_);
                int width = rect.Width;
                int height = rect.Height;
                int x = rect.Left;
                int y = rect.Top;

                return new WindowInfo(className, title, handle_, width, height, x, y);
            }
            catch (Exception e)
            {
                return WindowInfo.Empty;
            }

        }

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <param name="handle_"></param>
        /// <returns></returns>
        private string GetWindowTitle(IntPtr handle_)
        {
            try
            {
                int titleCapacity = Win32API.GetWindowTextLength(handle_) * 2;
                StringBuilder stringBuilder = new StringBuilder(titleCapacity);
                Win32API.GetWindowText(handle_, stringBuilder, stringBuilder.Capacity);
                string title = stringBuilder.ToString();
                return stringBuilder.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
