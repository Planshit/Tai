using Core.Enums;
using Core.Event;
using Core.Librarys;
using Core.Librarys.Browser;
using Core.Librarys.Browser.Favicon;
using Core.Librarys.SQLite;
using Core.Models.WebPage;
using Core.Servicers.Interfaces;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Threading;
using Site = Core.Models.WebPage.Site;

namespace Core.Servicers.Instances
{
    public class BrowserObserver : IBrowserObserver
    {
        public event BrowserObserverEventHandler OnSiteChanged;

        private readonly IAppObserver _appObserver;
        private readonly IWebData _webData;
        private readonly IWebFilter _webFilter;
        private readonly ISleepdiscover _sleepdiscover;
        private readonly IDateTimeObserver _dateTimeObserver;

        private IBrowserWatcher _browserWatcher;
        private bool _isStart = false;
        /// <summary>
        /// 忽略的URL集合
        /// </summary>
        private readonly string[] _ignoreURL = { "about:blank" };
        /// <summary>
        /// 当前浏览站点
        /// </summary>
        private Site _activeSite = null;
        /// <summary>
        /// 浏览开始时间
        /// </summary>
        private DateTime _activeStartTime = DateTime.MinValue;
        //  已保存图标的链接
        private List<string> _savedIconUrls = new List<string>();
        public BrowserObserver(IAppObserver appObserver_, IWebData webData_, IWebFilter webFilter_, ISleepdiscover sleepdiscover_, IDateTimeObserver dateTimeObserver_)
        {
            _appObserver = appObserver_;
            _webData = webData_;
            _webFilter = webFilter_;
            _sleepdiscover = sleepdiscover_;
            _dateTimeObserver = dateTimeObserver_;
        }
        public void Start()
        {
            if (_isStart)
            {
                return;
            }
            _isStart = true;
            _appObserver.OnAppActive += AppObserver_OnAppActive;
            _sleepdiscover.SleepStatusChanged += _sleepdiscover_SleepStatusChanged;
            _dateTimeObserver.OnDateTimeChanging += _dateTimeObserver_OnDateTimeChanging;
        }

        private void _dateTimeObserver_OnDateTimeChanging(object sender, DateTime e)
        {
            SaveTime(false);
        }

        public void Stop()
        {
            _appObserver.OnAppActive -= AppObserver_OnAppActive;
            _sleepdiscover.SleepStatusChanged -= _sleepdiscover_SleepStatusChanged;
            _dateTimeObserver.OnDateTimeChanging -= _dateTimeObserver_OnDateTimeChanging;

            _isStart = false;
            StopWatch();
        }

        private void _sleepdiscover_SleepStatusChanged(SleepStatus sleepStatus)
        {
            if (sleepStatus == SleepStatus.Sleep)
            {
                //  睡眠状态
                Stop();
            }
            else
            {
                //  唤醒
                Start();
            }
        }

        private void AppObserver_OnAppActive(Models.AppObserver.AppObserverEventArgs args)
        {
            switch (args.ProcessName)
            {
                case "chrome":
                    StartWatch(BrowserType.Chrome, args.Handle);
                    break;
                case "msedge":
                    StartWatch(BrowserType.MSEdge, args.Handle);
                    break;
                default:
                    StopWatch();
                    break;

            }
        }

        private void StartWatch(BrowserType browserType, IntPtr handle)
        {
            StopWatch();

            switch (browserType)
            {
                case BrowserType.Chrome:
                    _browserWatcher = new ChromeBrowserWatch();
                    break;
                case BrowserType.MSEdge:
                    _browserWatcher = new MSEdgeBrowserWatch();
                    break;
            }

            if (_browserWatcher == null)
            {
                Logger.Error("无法启动浏览器监听，浏览器类型：" + browserType.ToString());
                return;
            }

            _browserWatcher.OnSiteChanged += BrowserWatcher_OnSiteChanged; ;
            _browserWatcher.StartWatch(handle);
        }

        private void BrowserWatcher_OnSiteChanged(BrowserType browserType, Models.WebPage.Site site)
        {
            Debug.WriteLine(site.ToString());

            //  过滤URL
            if (_webFilter.IsIgnore(site.Url))
            {
                Debug.WriteLine($"已被过滤的URL {site.Url}");
                return;
            }
            //Debug.WriteLine($"域名：{UrlHelper.GetDomain(site.Url)}");
            //  更新网页时间
            SaveTime();

            _activeSite = site;
            _activeStartTime = DateTime.Now;

            OnSiteChanged?.Invoke(browserType, site);

            //  更新网页图标
            UpdateFavicon(browserType, site);
        }


        #region saveTime
        private void SaveTime(bool isReset_ = true)
        {
            if (_activeSite != null && _activeStartTime != DateTime.MinValue)
            {
                SaveTime(new Site(_activeSite), new DateTime(_activeStartTime.Ticks));

                if (isReset_)
                {
                    _activeStartTime = DateTime.MinValue;
                    _activeSite = null;
                }
                else
                {
                    _activeStartTime = DateTime.Now;
                }
            }
        }
        private void SaveTime(Site site_, DateTime startTime_)
        {
            Task.Run(() =>
            {
                if (!IsTrueURL(site_.Url))
                {
                    return;
                }

                TimeSpan browseTime = DateTime.Now - startTime_;
                int seconds = (int)(browseTime.TotalSeconds == 0 ? 1 : browseTime.TotalSeconds);

                if (seconds > 0)
                {
                    //  更新到数据库
                    _webData.AddUrlBrowseTime(site_, seconds, startTime_);
                    Debug.WriteLine($"网页：{site_.Title} 浏览了：{seconds}秒。开始时间：{startTime_}");
                }
            });
        }

        #endregion

        #region updatefavicon
        /// <summary>
        /// 更新获取网页favicon到tai数据库中
        /// </summary>
        /// <param name="browserType"></param>
        /// <param name="site"></param>
        private void UpdateFavicon(BrowserType browserType, Models.WebPage.Site site)
        {
            if (!IsTrueURL(site.Url))
            {
                return;
            }
            try
            {
                if (_savedIconUrls.Contains(site.Url))
                {
                    //  已经缓存的链接不再重复获取
                    return;
                }

                if (_savedIconUrls.Count > 100)
                {
                    //  限制缓存链接条数
                    _savedIconUrls.RemoveAt(0);
                }

                Task.Run(async () =>
                {
                    //  获取favicon
                    IFaviconManager fm = null;
                    switch (browserType)
                    {
                        case BrowserType.Chrome:
                            fm = new ChromeFaviconManager();
                            break;
                        case BrowserType.MSEdge:
                            fm = new ChromeFaviconManager(BrowserType.MSEdge);
                            break;
                    }

                    var data = await fm.GetIconDataAsync(site.Url);
                    if (data == null)
                    {
                        //  浏览器可能还没更新缓存，延迟再获取一次
                        Thread.Sleep(3000);
                        data = await fm.GetIconDataAsync(site.Url);
                    }
                    if (data != null)
                    {
                        //  保存到 tai目录下
                        string path = await fm.SaveToLocalIconAsync(data);

                        //  更新到数据库
                        _webData.UpdateUrlFavicon(site, path);

                        _savedIconUrls.Add(site.Url);
                    }
                    else
                    {
                        //  如果无法获取则使用站点图标
                        string domain = UrlHelper.GetDomain(site.Url);
                        //  读取站点数据
                        var webSite = _webData.GetWebSite(domain);

                        if (webSite != null)
                        {
                            _webData.UpdateUrlFavicon(site, webSite.IconFile);
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        #endregion

        /// <summary>
        /// 指示URL是否正确
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsTrueURL(string url)
        {
            return !string.IsNullOrEmpty(url) && !_ignoreURL.Contains(url);
        }
        private void StopWatch()
        {
            if (_browserWatcher != null)
            {
                _browserWatcher.OnSiteChanged -= BrowserWatcher_OnSiteChanged;
                _browserWatcher.StopWatch();
                _browserWatcher = null;
            }

            //  保存时间
            SaveTime();
        }


    }
}
