using Core.Librarys;
using Core.Models;
using Core.Models.Config;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.Librays;
using UI.Models;

namespace UI.ViewModels
{
    public class DetailPageVM : DetailPageModel
    {
        private readonly IData data;
        private readonly MainViewModel main;
        private readonly IAppConfig appConfig;

        private ConfigModel config;
        public Command BlockActionCommand { get; set; }
        public Command ClearSelectMonthDataCommand { get; set; }
        public Command InfoMenuActionCommand { get; set; }
        public DetailPageVM(
            IData data, MainViewModel main, IAppConfig appConfig)
        {
            this.data = data;
            this.main = main;
            this.appConfig = appConfig;

            BlockActionCommand = new Command(new Action<object>(OnBlockActionCommand));
            ClearSelectMonthDataCommand = new Command(new Action<object>(OnClearSelectMonthDataCommand));
            InfoMenuActionCommand = new Command(new Action<object>(OnInfoMenuActionCommand));
            Init();
        }



        private async void Init()
        {
            Process = main.Data as ChartsDataModel;

            Date = DateTime.Now;

            PropertyChanged += DetailPageVM_PropertyChanged;

            config = appConfig.GetConfig();

            await LoadData();

            await LoadInfo();

        }
        private void OnInfoMenuActionCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "copy processname":
                    Clipboard.SetText(ProcessName);

                    break;
                case "copy process file":
                    Clipboard.SetText(Process.PopupText);
                    break;
                case "open dir":
                    if (File.Exists(Process.PopupText))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", "/select, " + Process.PopupText);
                    }
                    else
                    {
                        main.Toast("进程文件似乎不存在", Controls.Base.IconTypes.Blocked);
                    }
                    break;
                case "open exe":
                    if (File.Exists(Process.PopupText))
                    {
                        System.Diagnostics.Process.Start(Process.PopupText);
                    }
                    else
                    {
                        main.Toast("进程文件似乎不存在", Controls.Base.IconTypes.Blocked);
                    }
                    break;
            }
        }

        private async void OnClearSelectMonthDataCommand(object obj)
        {
            await Clear();
        }

        private void OnBlockActionCommand(object obj)
        {
            if (obj.ToString() == "block")
            {
                config.Behavior.IgnoreProcessList.Add(ProcessName);
                IsIgnore = true;
                main.Toast("进程已忽略");
            }
            else
            {
                config.Behavior.IgnoreProcessList.Remove(ProcessName);
                IsIgnore = false;
                main.Toast("进程已取消忽略");
            }

            appConfig.Save();
        }

        private async void DetailPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Date))
            {
                await LoadData();
            }
        }

        private async Task LoadInfo()
        {
            await Task.Run(() =>
            {
                if (Process != null)
                {
                    var info = Process.Data as DailyLogModel;
                    if (info != null)
                    {
                        ProcessName = info.ProcessName;
                        //  判断是否是忽略的进程
                        IsIgnore = config.Behavior.IgnoreProcessList.Contains(ProcessName);
                    }

                    var today = data.GetProcess(ProcessName, DateTime.Now);
                    var yesterday = data.GetProcess(ProcessName, DateTime.Now.AddDays(-1));

                    if (today != null)
                    {
                        TodayTime = Timer.Fromat(today.Time);
                    }
                    else
                    {
                        TodayTime = "暂无数据";
                    }

                    if (yesterday != null)
                    {
                        int diffseconds = today != null ? today.Time - yesterday.Time : yesterday.Time;

                        string diffText = string.Empty;
                        if (diffseconds != yesterday.Time)
                        {
                            if (diffseconds == 0)
                            {
                                //  无变化
                                diffText = "持平";
                            }
                            else if (diffseconds > 0)
                            {
                                //  增加
                                diffText = "增加了" + Timer.Fromat(diffseconds);
                            }
                            else
                            {
                                //  减少
                                diffText = "减少了" + Timer.Fromat(Math.Abs(diffseconds));
                            }
                        }
                        Yesterday = diffseconds == yesterday.Time ? "减少100%" : diffText;
                    }
                    else
                    {
                        Yesterday = "昨日未使用";
                    }
                }
            }
            );
        }

        private async Task LoadData()
        {
            await Task.Run(() =>
            {
                if (Process != null)
                {
                    var info = Process.Data as DailyLogModel;
                    var monthData = data.GetProcessMonthLogList(info.ProcessName, Date);
                    int monthTotal = monthData.Sum(m => m.Time);
                    Total = Timer.Fromat(monthTotal);

                    DateTime start = new DateTime(Date.Year, Date.Month, 1);
                    DateTime end = new DateTime(Date.Year, Date.Month, DateTime.DaysInMonth(Date.Year, Date.Month));
                    var monthAllData = data.GetDateRangelogList(start, end);

                    LongDay = "暂无数据";
                    if (monthData.Count > 0)
                    {
                        var longDayData = monthData.OrderByDescending(m => m.Time).FirstOrDefault();
                        LongDay = longDayData.Date.ToString("最长一天是在 dd 日，使用了 " + Timer.Fromat(longDayData.Time));

                    }


                    int monthAllTotal = monthAllData.Sum(m => m.Time);
                    if (monthAllTotal > 0)
                    {
                        Ratio = (((double)monthTotal / (double)monthAllTotal)).ToString("P");
                    }
                    else
                    {
                        Ratio = "暂无数据";
                    }
                    Data = MapToChartsData(monthData);
                    if (Data.Count == 0)
                    {
                        Data.Add(new ChartsDataModel()
                        {
                            DateTime = Date,
                        });
                    }
                }
            });
        }

        private async Task Clear()
        {
            await Task.Run(async () =>
            {
                if (Process != null)
                {
                    main.Toast("正在处理");
                    data.Clear(ProcessName, Date);

                    await LoadData();
                    await LoadInfo();
                    main.Toast("已清空");
                }
            }
            );
        }


        private List<ChartsDataModel> MapToChartsData(List<Core.Models.DailyLogModel> list)
        {
            var resData = new List<ChartsDataModel>();

            foreach (var item in list)
            {
                var bindModel = new ChartsDataModel();
                bindModel.Data = item;
                bindModel.Name = string.IsNullOrEmpty(item.ProcessDescription) ? item.ProcessName : item.ProcessDescription;
                bindModel.Value = item.Time;
                bindModel.Tag = Timer.Fromat(item.Time);
                bindModel.PopupText = item.File;
                bindModel.Icon = Iconer.Get(item.ProcessName, item.ProcessDescription);
                bindModel.DateTime = item.Date;
                resData.Add(bindModel);
            }

            return resData;
        }
    }
}
