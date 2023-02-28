using Core.Enums;
using Core.Event;
using Core.Librarys;
using Core.Librarys.Browser.Favicon;
using Core.Models.WebPage;
using Core.Servicers.Interfaces;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Interop;
using UIAutomationClient;

namespace Core.Servicers.Instances
{
    public class MSEdgeBrowserWatch : IBrowserWatcher
    {
        public event BrowserObserverEventHandler OnSiteChanged;
        private string url, title;
        CUIAutomation automation;
        //  根元素（监听站点切换使用）
        IUIAutomationElement rootElement;
        //  主元素（寻找URL使用）
        IUIAutomationElement mainElement;

        IntPtr hook;
        Win32API.WinEventDelegate winEventDelegate;
        public MSEdgeBrowserWatch()
        {
            automation = new CUIAutomation();
            winEventDelegate = new Win32API.WinEventDelegate(WinEventProc);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            //Debug.WriteLine("idObject:" + idObject + ",idChild:" + idChild);

            if (idObject == 0 && idChild == 0)
            {
                Debug.WriteLine("切换选项卡 ->  " + dwmsEventTime);
                Handle();
            }
        }

        public void StartWatch(IntPtr handle)
        {
            try
            {
                if (handle == IntPtr.Zero)
                {
                    throw new Exception("未传入浏览器句柄");
                }

                Win32API.GetWindowThreadProcessId(handle, out int processID);
                Process process = Process.GetProcessById(processID);
                if (process == null)
                {
                    return;
                }

                Debug.WriteLine("启动浏览器监听，目标：" + process.ProcessName + "，" + handle);
                //  搜索根元素 
                rootElement = automation.ElementFromHandle(handle);
                if (rootElement == null)
                {
                    throw new Exception("找不到根元素！");
                }
                //  找到窗口
                var condtionPanel = automation.CreatePropertyCondition(30003, 50033);
                var condtionName = automation.CreatePropertyCondition(30012, "BrowserRootView");
                var condtionFindWindow = automation.CreateAndCondition(condtionPanel, condtionName);
                var window = rootElement.FindFirst(TreeScope.TreeScope_Children, condtionFindWindow);
                if (window != null)
                {
                    //  第二个窗口
                    mainElement = window.FindFirst(TreeScope.TreeScope_Children, automation.CreateTrueCondition());
                }
                else
                {
                    mainElement = rootElement;
                    Debug.WriteLine("没有找到主窗口，使用根元素作为主元素");
                }


                if (mainElement == null)
                {
                    throw new Exception("找不到主元素！");
                }

                //  注册事件

                ////  找到窗口
                //var condtionTab = automation.CreatePropertyCondition(30003, 50018);
                //var condtionClassName = automation.CreatePropertyCondition(30012, "TabStripRegionView");
                //var tabElement = mainElement.FindFirst(TreeScope.TreeScope_Descendants, automation.CreateAndCondition(condtionTab, condtionClassName));
                hook = Win32API.SetWinEventHook(0x800C, 0x800C, handle, winEventDelegate, 0, 0, 0);

                Debug.WriteLine("启动浏览器监听成功");

                Handle();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        public void StopWatch()
        {
            try
            {
                Win32API.UnhookWinEvent(hook);
                Debug.WriteLine("已停止浏览器监听");
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        private string FormatTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return title;
            }
            int length = title.LastIndexOf('-') - 1;
            if (length < 1)
            {
                length = 1;
            }

            return title.Substring(0, length);
        }

        private async void Handle()
        {
            string title = await GetURLTitleSync();
            title = FormatTitle(title);
            Site site = null;
            if (!string.IsNullOrEmpty(title))
            {
                site = SiteCache.Get(title);
            }

            if (site == null)
            {
                string url = await GetURLSync();
                site = new Site()
                {
                    Url = url,
                    Title = title
                };
                SiteCache.Set(title, url);
            }

            ResponseEvent(site.Url, site.Title);
        }

        #region 获取浏览器当前URL
        /// <summary>
        /// 获取浏览器当前URL
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetURLSync()
        {
            return await Task.Run(() =>
               {
                   string result = string.Empty;
                   try
                   {
                       var condtionEdit = automation.CreatePropertyCondition(30003, 50004);
                       var condtionAccessKey = automation.CreatePropertyCondition(30006, "Ctrl+L");
                       var condtion = automation.CreateAndCondition(condtionEdit, condtionAccessKey);
                       var addressElement = rootElement.FindFirst(TreeScope.TreeScope_Descendants, condtion);
                       if (addressElement != null)
                       {
                           result = ((IUIAutomationValuePattern)addressElement.GetCurrentPattern(10002)).CurrentValue;
                       }
                   }
                   catch (Exception e)
                   {
                       Logger.Error(e.ToString());
                   }
                   return result;
               });

        }
        #endregion

        #region 获取浏览器当前网页标题
        /// <summary>
        /// 获取浏览器当前网页标题
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetURLTitleSync()
        {
            return await Task.Run(() =>
            {
                string result = string.Empty;
                try
                {
                    var condtionEdit = automation.CreatePropertyCondition(30003, 50033);
                    var condtionCN = automation.CreatePropertyCondition(30012, "BrowserRootView");
                    var condtion = automation.CreateAndCondition(condtionEdit, condtionCN);
                    var element = rootElement.FindFirst(TreeScope.TreeScope_Descendants, condtion);
                    if (element != null)
                    {
                        result = element.GetCurrentPropertyValue(30005);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
                return result;
            });

        }
        #endregion

        private void ResponseEvent(string url, string title)
        {
            if (url != this.url)
            {
                this.url = url;
                this.title = title;
                OnSiteChanged?.Invoke(BrowserType.MSEdge, new Models.WebPage.Site()
                {
                    Url = url,
                    Title = title
                });
            }
        }
    }
}
