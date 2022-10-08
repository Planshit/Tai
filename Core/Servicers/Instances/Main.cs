using Core.Enums;
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
        private readonly IObserver observer;
        private readonly IData data;
        private readonly ISleepdiscover sleepdiscover;
        private readonly IAppConfig appConfig;
        private readonly IDateTimeObserver dateTimeObserver;
        private readonly IAppData appData;
        private readonly ICategorys categories;
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
        };

        /// <summary>
        /// 当前聚焦进程
        /// </summary>
        private string activeProcess, activeProcessDescription, activeProcessFile = null;

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
        public Main(
            IObserver observer,
            IData data,
            ISleepdiscover sleepdiscover,
            IAppConfig appConfig,
            IDateTimeObserver dateTimeObserver,
            IAppData appData, ICategorys categories)
        {
            this.observer = observer;
            this.data = data;
            this.sleepdiscover = sleepdiscover;
            this.appConfig = appConfig;
            this.dateTimeObserver = dateTimeObserver;
            this.appData = appData;
            this.categories = categories;

            IgnoreProcessCacheList = new List<string>();
            ConfigIgnoreProcessRegxList = new List<string>();
            ConfigIgnoreProcessList = new List<string>();

            observer.OnAppActive += Observer_OnAppActive;
            sleepdiscover.SleepStatusChanged += Sleepdiscover_SleepStatusChanged;
            appConfig.ConfigChanged += AppConfig_ConfigChanged;
            dateTimeObserver.OnDateTimeChanging += DateTimeObserver_OnDateTimeChanging;
        }

        private void DateTimeObserver_OnDateTimeChanging(object sender, DateTime time)
        {
            if (sleepStatus == SleepStatus.Wake)
            {
                UpdateTime(time);
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
            }
        }

        public async void Run()
        {
            await Task.Run(() =>
             {
                 Debug.WriteLine("db self check start");
                 //  数据库自检
                 using (var db = new TaiDbContext())
                 {
                     db.SelfCheck();
                 }
                 Debug.WriteLine("db self check over!");

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

            //  日期变化观察
            dateTimeObserver.Start();

            //  启动进程观察
            observer.Start();

            //  启动睡眠监测
            sleepdiscover.Start();

        }

        public void Exit()
        {
            observer?.Stop();

            //appData.SaveAppChanges();
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

            Debug.WriteLine("睡眠状态：" + sleepStatus);
            Logger.Info("【" + sleepStatus + "】当前进程：" + activeProcess);
            if (sleepStatus == SleepStatus.Sleep)
            {
                //  更新时间
                UpdateTime();

                activeProcess = null;
                //activeSeconds = 0;
                //activeTimer.Stop();

                //  更新app数据
                //appData.SaveAppChanges();
            }
            else if (sleepStatus == SleepStatus.Wake)
            {
                //  唤醒时重置计时开始时间
                activeStartTime = DateTime.Now;
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
                if (Regex.IsMatch(processName, reg))
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

            return true;
        }

        private void Observer_OnAppActive(string processName, string description, string file)
        {
            bool isCheck = IsCheckApp(processName, description, file);

            if (activeProcess != processName && activeProcessFile != file)
            {
                UpdateTime();
            }

            if (isCheck && sleepStatus != SleepStatus.Sleep)
            {
                if (activeProcess == null)
                {
                    activeStartTime = DateTime.Now;
                }
                activeProcess = processName;
                activeProcessDescription = description;
                activeProcessFile = file;
            }
            else
            {
                activeProcess = null;
            }
        }

        private void HandleLinks(string processName, int seconds, DateTime? time = null)
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
                                    data.Set(linkProcess, seconds, time);
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

        private void UpdateTime(DateTime? time = null)
        {
            if (!string.IsNullOrEmpty(activeProcess))
            {
                //  更新计时
                TimeSpan timeSpan = DateTime.Now - activeStartTime;

                int seconds = (int)timeSpan.TotalSeconds;

                //  如果是休眠状态要减去5分钟
                if (sleepStatus == SleepStatus.Sleep)
                {
                    seconds -= 300;
                }

                if (seconds > 0)
                {
                    data.Set(activeProcess, seconds, time);

                    //  关联进程更新
                    HandleLinks(activeProcess, seconds, time);
                }

                Logger.Info("status:" + sleepStatus + ",process:" + activeProcess + ",seconds:" + seconds + ",start:" + activeStartTime.ToString() + ",end:" + DateTime.Now.ToString());

                activeStartTime = DateTime.Now;

                OnUpdateTime?.Invoke(this, null);


            }
        }

    }
}
