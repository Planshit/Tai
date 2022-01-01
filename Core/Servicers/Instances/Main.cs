using Core.Enums;
using Core.Librarys;
using Core.Librarys.SQLite;
using Core.Models.Config;
using Core.Models.Config.Link;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private readonly IDateObserver dateObserver;
        //  忽略的进程
        private readonly string[] IgnoreProcess = new string[] {
            "Tai",
            "SearchHost",
            "Taskmgr",
            "ApplicationFrameHost",
            "StartMenuExperienceHost",
            "ShellExperienceHost",
            "OpenWith",
            "Updater",
            "LockApp",
            "",
            "",
            "",
            "",
            "",
            "",

        };

        /// <summary>
        /// 当前聚焦进程
        /// </summary>
        private string activeProcess, activeProcessDescription, activeProcessFile = null;

        /// <summary>
        /// 焦点开始时间
        /// </summary>
        private DateTime activeStartTime = DateTime.Now;

        ///// <summary>
        ///// 活动计时器
        ///// </summary>
        //private Timer activeTimer;
        /// <summary>
        /// 睡眠状态
        /// </summary>
        private SleepStatus sleepStatus;

        /// <summary>
        /// app config
        /// </summary>
        private ConfigModel config;

        public event EventHandler OnUpdateTime;
        ///// <summary>
        ///// 当前焦点使用时长（秒）
        ///// </summary>
        //private int activeSeconds = 0;
        public Main(
            IObserver observer,
            IData data,
            ISleepdiscover sleepdiscover,
            IAppConfig appConfig, IDateObserver dateObserver)
        {
            this.observer = observer;
            this.data = data;
            this.sleepdiscover = sleepdiscover;
            this.appConfig = appConfig;
            this.dateObserver = dateObserver;

            observer.OnAppActive += Observer_OnAppActive;
            sleepdiscover.SleepStatusChanged += Sleepdiscover_SleepStatusChanged;
            //activeTimer = new Timer();
            //activeTimer.Interval = 1000;
            //activeTimer.Elapsed += ActiveTimer_Elapsed;
            appConfig.ConfigChanged += AppConfig_ConfigChanged;
            dateObserver.OnDateChanging += DateObserver_OnDateChanging;
        }

        //private void ActiveTimer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    activeSeconds++;
        //}

        private void DateObserver_OnDateChanging(object sender, EventArgs e)
        {
            if (sleepStatus == SleepStatus.Wake)
            {
                Debug.WriteLine("日期变更前：" + DateTime.Now.ToString());
                UpdateTime();
            }
        }

        private void AppConfig_ConfigChanged(ConfigModel oldConfig, ConfigModel newConfig)
        {
            if (oldConfig != newConfig)
            {
                //  处理开机自启
                Shortcut.SetStartup(newConfig.General.IsStartatboot);
            }
        }

        public void Run()
        {
            //  加载应用配置（确保配置文件最先加载
            appConfig.Load();
            config = appConfig.GetConfig();

            //  日期变化观察
            dateObserver.Start();

            //  启动进程观察
            observer.Start();

            //  启动睡眠监测
            sleepdiscover.Start();

        }

        public void Exit()
        {
            observer?.Stop();
        }


        private void Sleepdiscover_SleepStatusChanged(Enums.SleepStatus sleepStatus)
        {
            this.sleepStatus = sleepStatus;

            Debug.WriteLine("睡眠状态：" + sleepStatus);

            if (sleepStatus == SleepStatus.Sleep)
            {
                //  更新时间
                UpdateTime();

                activeProcess = null;
                //activeSeconds = 0;
                //activeTimer.Stop();
            }
        }

        private void Observer_OnAppActive(string processName, string description, string file)
        {
            if (activeProcess != processName && activeProcessFile != file)
            {
                UpdateTime();
            }

            if (!config.Behavior.IgnoreProcessList.Contains(processName)
                && file != string.Empty
                && sleepStatus != SleepStatus.Sleep
                && !IgnoreProcess.Contains(processName))
            {
                activeProcess = processName;
                activeProcessDescription = description;
                activeProcessFile = file;

                activeStartTime = DateTime.Now;

                //activeSeconds = 0;
                //if (!activeTimer.Enabled)
                //{
                //    activeTimer.Start();
                //}

                //  提取icon
                Iconer.ExtractFromFile(file, processName, description);
            }
            else
            {
                activeProcess = null;
                //activeSeconds = 0;
                //activeTimer.Stop();
            }
        }

        private void HandleLinks(string processName, int seconds)
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
                                    data.Set(linkProcess, seconds);
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

        private void UpdateTime()
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

                if (seconds <= 0)
                {
                    return;
                }



                Logger.Info(sleepStatus + " 进程：" + activeProcess + " 更新时间：" + seconds);


                data.Set(activeProcess, activeProcessDescription, activeProcessFile, seconds);

                //  关联进程更新
                HandleLinks(activeProcess, seconds);

                OnUpdateTime?.Invoke(this, null);
            }
        }

    }
}
