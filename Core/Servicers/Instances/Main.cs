﻿using Core.Enums;
using Core.Librarys;
using Core.Librarys.SQLite;
using Core.Models;
using Core.Models.Config;
using Core.Models.Config.Link;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace Core.Servicers.Instances
{
    public class Main : IMain
    {
        private readonly IAppObserver appObserver;
        private readonly IBrowserObserver browserObserver;
        private readonly IData data;
        private readonly ISleepdiscover sleepdiscover;
        private readonly IAppConfig appConfig;
        private readonly IDateTimeObserver dateTimeObserver;
        private readonly IAppData appData;
        private readonly ICategorys categories;
        private readonly IWebFilter _webFilter;
        //  忽略的进程
        private readonly string[] DefaultIgnoreProcess = new string[] {
            "Tai",
            "SearchHost",
            "Taskmgr",
            "ApplicationFrameHost",
            "StartMenuExperienceHost",
            "ShellExperienceHost",
            "OpenWith",
            "Updater",
            "LockApp",
            "dwm",
            "SystemSettingsAdminFlows"
        };

        /// <summary>
        /// 当前聚焦进程
        /// </summary>
        private string activeProcess = null;

        /// <summary>
        /// 焦点开始时间
        /// </summary>
        private DateTime activeStartTime = DateTime.Now;

        /// <summary>
        /// 睡眠状态
        /// </summary>
        private SleepStatus sleepStatus;

        /// <summary>
        /// app config
        /// </summary>
        private ConfigModel config;

        public event EventHandler OnUpdateTime;
        public event EventHandler OnStarted;

        /// <summary>
        /// 忽略进程缓存列表
        /// </summary>
        private List<string> IgnoreProcessCacheList;

        /// <summary>
        /// 配置正则忽略进程列表
        /// </summary>
        private List<string> ConfigIgnoreProcessRegxList;
        /// <summary>
        /// 配置忽略进程名称列表
        /// </summary>
        private List<string> ConfigIgnoreProcessList;
        //  更新应用日期
        private DateTime updadteAppDateTime_ = DateTime.Now.Date;
        //  已经更新过的应用列表
        private List<string> updatedAppList = new List<string>();
        public Main(
            IAppObserver appObserver,
            IData data,
            ISleepdiscover sleepdiscover,
            IAppConfig appConfig,
            IDateTimeObserver dateTimeObserver,
            IAppData appData, ICategorys categories,
            IBrowserObserver browserObserver,
            IWebFilter webFilter_)
        {
            this.appObserver = appObserver;
            this.data = data;
            this.sleepdiscover = sleepdiscover;
            this.appConfig = appConfig;
            this.dateTimeObserver = dateTimeObserver;
            this.appData = appData;
            this.categories = categories;
            this.browserObserver = browserObserver;
            _webFilter = webFilter_;

            IgnoreProcessCacheList = new List<string>();
            ConfigIgnoreProcessRegxList = new List<string>();
            ConfigIgnoreProcessList = new List<string>();

            appObserver.OnAppActive += AppObserver_OnAppActive;
            sleepdiscover.SleepStatusChanged += Sleepdiscover_SleepStatusChanged;
            appConfig.ConfigChanged += AppConfig_ConfigChanged;
            dateTimeObserver.OnDateTimeChanging += DateTimeObserver_OnDateTimeChanging;
        }



        private void DateTimeObserver_OnDateTimeChanging(object sender, DateTime time)
        {
            Logger.Info("[time changed] status:" + sleepStatus + ",process:" + activeProcess + ",start:" + activeStartTime.ToString() + ",end:" + DateTime.Now.ToString() + ",time:" + time.ToString());
            if (sleepStatus == SleepStatus.Wake)
            {
                UpdateTime();
            }
            else
            {
                activeStartTime = DateTime.Now;
            }
        }

        private void AppConfig_ConfigChanged(ConfigModel oldConfig, ConfigModel newConfig)
        {
            if (oldConfig != newConfig)
            {
                //  处理开机自启
                SystemCommon.SetStartup(newConfig.General.IsStartatboot);

                //  更新忽略规则
                UpdateConfigIgnoreProcess();

                //  处理web记录功能启停
                HandleWebServiceConfig();
            }
        }

        public async void Run()
        {
            await Task.Run(() =>
             {
                 CreateDirectory();

                 //  数据库自检
                 using (var db = new TaiDbContext())
                 {
                     db.SelfCheck();
                 }

                 //  加载app信息
                 appData.Load();

                 // 加载分类信息
                 categories.Load();

                 AppState.IsLoading = false;
             });



            //  加载应用配置（确保配置文件最先加载
            appConfig.Load();
            config = appConfig.GetConfig();
            UpdateConfigIgnoreProcess();



            //  启动睡眠监测
            sleepdiscover.Start();

            //  初始化过滤器
            _webFilter.Init();

            //  启动主服务
            Start();

            OnStarted?.Invoke(this, EventArgs.Empty);
        }
        public void Start()
        {
            activeStartTime = DateTime.Now;
            activeProcess = null;

            dateTimeObserver.Start();
            appObserver.Start();

            if (config.General.IsWebEnabled)
            {
                browserObserver.Start();
            }
        }
        public void Stop()
        {
            dateTimeObserver.Stop();
            appObserver.Stop();
        }
        public void Exit()
        {
            appObserver?.Stop();
        }

        /// <summary>
        /// 创建程序目录
        /// </summary>
        private void CreateDirectory()
        {
            string dir = Path.Combine(FileHelper.GetRootDirectory(), "Data");
            Directory.CreateDirectory(dir);
        }

        private void UpdateConfigIgnoreProcess()
        {
            if (config == null)
            {
                return;
            }
            ConfigIgnoreProcessList.Clear();
            ConfigIgnoreProcessRegxList.Clear();
            IgnoreProcessCacheList.Clear();

            ConfigIgnoreProcessList = config.Behavior.IgnoreProcessList.Where(m => !IsRegex(m)).ToList();
            ConfigIgnoreProcessRegxList = config.Behavior.IgnoreProcessList.Where(m => IsRegex(m)).ToList();
        }

        private bool IsRegex(string str)
        {
            return Regex.IsMatch(str, @"[\.|\*|\?|\{|\\|\[|\^|\|]");
        }

        private void Sleepdiscover_SleepStatusChanged(Enums.SleepStatus sleepStatus)
        {
            this.sleepStatus = sleepStatus;

            Logger.Info($"[{sleepStatus}] process:{activeProcess},start:{activeStartTime}");
            if (sleepStatus == SleepStatus.Sleep)
            {
                //  进入睡眠状态
                Debug.WriteLine("进入睡眠状态");

                //  停止服务
                Stop();

                //  更新时间
                UpdateTime();
            }
            else
            {
                //  从睡眠状态唤醒
                Debug.WriteLine("从睡眠状态唤醒");
                Start();
            }
        }


        /// <summary>
        /// 检查应用是否需要记录数据
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="description"></param>
        /// <param name="file"></param>
        private bool IsCheckApp(string processName, string description, string file)
        {
            if (string.IsNullOrEmpty(file) || string.IsNullOrEmpty(processName) || DefaultIgnoreProcess.Contains(processName) || IgnoreProcessCacheList.Contains(processName))
            {
                return false;
            }

            //  从名称判断
            if (ConfigIgnoreProcessList.Contains(processName))
            {
                return false;
            }

            //  正则表达式
            foreach (string reg in ConfigIgnoreProcessRegxList)
            {
                if (RegexHelper.IsMatch(processName, reg) || RegexHelper.IsMatch(file, reg))
                {
                    IgnoreProcessCacheList.Add(processName);
                    return false;
                }
            }

            AppModel app = appData.GetApp(processName);
            if (app == null)
            {
                //  记录应用信息

                //  提取icon
                string iconFile = Iconer.ExtractFromFile(file, processName, description);

                appData.AddApp(new AppModel()
                {
                    Name = processName,
                    Description = description,
                    File = file,
                    CategoryID = 0,
                    IconFile = iconFile,
                });
            }
            else
            {
                if (updadteAppDateTime_ != DateTime.Now.Date)
                {
                    updadteAppDateTime_ = DateTime.Now.Date;
                    updatedAppList.Clear();
                }

                if (!updatedAppList.Contains(processName))
                {
                    //  更新应用信息
                    app.IconFile = Iconer.ExtractFromFile(file, processName, description);

                    // if (app.Description != description)
                    // {
                    //     app.Description = description;
                    // }
                    if (app.File != file)
                    {
                        app.File = file;
                    }
                    appData.UpdateApp(app);
                    updatedAppList.Add(processName);
                }
            }

            return true;
        }

        private void AppObserver_OnAppActive(Models.AppObserver.AppObserverEventArgs args)
        {
            if (sleepStatus == SleepStatus.Sleep)
            {
                return;
            }

            string lastActiveProcess = activeProcess != null ? activeProcess.ToString() : "";

            bool isCheck = IsCheckApp(args.ProcessName, args.Description, args.File);

            Logger.Info($"Active[{isCheck}]:" + args.ProcessName + ",Last:" + lastActiveProcess + ",Time:" + activeStartTime.ToString());

            if (activeProcess != args.ProcessName)
            {
                UpdateTime();
            }

            if (isCheck)
            {
                if (activeProcess == null)
                {
                    activeStartTime = DateTime.Now;
                }
                activeProcess = args.ProcessName;
            }
            else
            {
                activeProcess = null;
            }

        }

        private void HandleLinks(string processName, int seconds, DateTime time)
        {
            Task.Run(() =>
            {
                try
                {
                    List<LinkModel> links = config.Links != null ? config.Links : new List<LinkModel>();
                    foreach (LinkModel link in links)
                    {
                        if (link.ProcessList != null
                            && link.ProcessList.Count >= 2
                            && link.ProcessList.Contains(processName))
                        {
                            //  属于关联进程
                            foreach (string linkProcess in link.ProcessList)
                            {
                                if (linkProcess != processName)
                                {
                                    if (IsProcessRuning(linkProcess))
                                    {
                                        //  同步更新
                                        data.SaveAppDuration(linkProcess, seconds, time);
                                    }

                                }
                            }
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + "，关联进程更新错误，Process Name: " + processName + "，Time: " + seconds);
                }
            });
        }

        #region 判断进程是否在运行中
        /// <summary>
        /// 判断进程是否在运行中
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        private bool IsProcessRuning(string processName)
        {
            Process[] process = Process.GetProcessesByName(processName);
            return process != null && process.Length > 0;
        }
        #endregion

        private void UpdateTime()
        {
            string app = activeProcess != null ? activeProcess.ToString() : "";

            if (!string.IsNullOrEmpty(app))
            {
                var time = new DateTime(activeStartTime.Ticks);

                //  更新计时
                TimeSpan timeSpan = DateTime.Now - time;
                int seconds = (int)timeSpan.TotalSeconds;

                if (seconds > 3600)
                {
                    Logger.Warn($"计时异常。start:{time},process:{app},duration:{seconds},status:{sleepStatus}");
                    activeStartTime = DateTime.Now;
                    return;
                }

                //  如果是休眠状态要减去5分钟
                if (sleepStatus == SleepStatus.Sleep)
                {
                    seconds -= 300;
                }

                if (seconds > 0)
                {
                    data.SaveAppDuration(app, seconds, time);

                    //  关联进程更新
                    HandleLinks(app, seconds, time);
                }

                activeStartTime = DateTime.Now;
                OnUpdateTime?.Invoke(this, null);
            }
        }

        #region 处理网站数据记录配置项开关
        /// <summary>
        /// 处理网站数据记录配置项开关
        /// </summary>
        private void HandleWebServiceConfig()
        {
            if (config == null)
            {
                return;
            }

            if (config.General.IsWebEnabled)
            {
                browserObserver.Start();
            }
            else
            {
                browserObserver.Stop();
            }
        }
        #endregion

    }
}
