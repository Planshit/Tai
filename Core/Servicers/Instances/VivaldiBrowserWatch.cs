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
    public class VivaldiBrowserWatch : IBrowserWatcher
    {
        public event BrowserObserverEventHandler OnSiteChanged;
        private string url, title;
        CUIAutomation automation;
        //  根元素（监听站点切换使用）
        IUIAutomationElement rootElement;
        //  主元素（寻找URL使用）
        IUIAutomationElement mainElement;
        //  站点切换事件
        AutomationPropertyChangedEventHandler siteChangedEventHandler;

        public VivaldiBrowserWatch()
        {
            automation = new CUIAutomation();
            siteChangedEventHandler = new AutomationPropertyChangedEventHandler(OnNameChanged);
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

                Debug.WriteLine("启动浏览器监听，目标：" + process.ProcessName);
                //  搜索根元素 
                rootElement = automation.ElementFromHandle(handle);
                if (rootElement == null)
                {
                    throw new Exception("找不到根元素！");
                }
                //  找到窗口
                var condtionPanel = automation.CreatePropertyCondition(30003, 50033);
                var condtionName = automation.CreatePropertyCondition(30005, "Chrome_WidgetWin_1");
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
                IUIAutomationCacheRequest cacheRequest = automation.CreateCacheRequest();
                cacheRequest.AddProperty(30005);
                int[] props = { 30005 };

                automation.AddPropertyChangedEventHandler(rootElement, TreeScope.TreeScope_Element, cacheRequest, siteChangedEventHandler, props);
                Debug.WriteLine("启动浏览器监听成功");

                Handle(process.MainWindowTitle);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        private void OnNameChanged(IUIAutomationElement s, int p, object v)
        {
            Handle(v.ToString());
        }

        #region AutomationPropertyChangedEventHandler
        private class AutomationPropertyChangedEventHandler : IUIAutomationPropertyChangedEventHandler
        {
            public AutomationPropertyChangedEventHandler(Action<IUIAutomationElement, int, object> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                Action = action;
            }

            public Action<IUIAutomationElement, int, object> Action { get; }
            public void HandlePropertyChangedEvent(IUIAutomationElement sender, int propertyId, object newValue) => Action(sender, propertyId, newValue);
        }
        #endregion

        public void StopWatch()
        {
            try
            {
                Debug.WriteLine("已停止浏览器监听");
                automation.RemovePropertyChangedEventHandler(rootElement, siteChangedEventHandler);
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

        private async void Handle(string title)
        {
            title = FormatTitle(title);
            //Debug.WriteLine($"切换标题 {title}");
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
                    var condtionName = automation.CreatePropertyCondition(30011, "urlFieldInput");
                    var condtion = automation.CreateAndCondition(condtionEdit, condtionAccessKey);
                    var addressElement = rootElement.FindFirst(TreeScope.TreeScope_Subtree, condtionName);
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

        private void ResponseEvent(string url, string title)
        {
            if (url != this.url)
            {
                this.url = url;
                this.title = title;
                OnSiteChanged?.Invoke(BrowserType.Chrome, new Models.WebPage.Site()
                {
                    Url = url,
                    Title = title
                });
            }
        }
    }
}
