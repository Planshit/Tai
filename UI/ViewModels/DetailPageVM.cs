﻿using Core.Librarys;
using Core.Models;
using Core.Models.Config;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.Controls.Select;
using UI.Models;
using UI.Servicers;

namespace UI.ViewModels
{
    public class DetailPageVM : DetailPageModel
    {
        private readonly IData data;
        private readonly MainViewModel main;
        private readonly IAppConfig appConfig;
        private readonly ICategorys categories;
        private readonly IAppData appData;
        private readonly IInputServicer inputServicer;

        private ConfigModel config;
        public Command BlockActionCommand { get; set; }
        public Command ClearSelectMonthDataCommand { get; set; }
        public Command InfoMenuActionCommand { get; set; }
        public Command RefreshCommand { get; set; }

        public DetailPageVM(
            IData data, MainViewModel main, IAppConfig appConfig, ICategorys categories, IAppData appData, IInputServicer inputServicer)
        {
            this.data = data;
            this.main = main;
            this.appConfig = appConfig;
            this.categories = categories;
            this.appData = appData;
            this.inputServicer = inputServicer;

            BlockActionCommand = new Command(new Action<object>(OnBlockActionCommand));
            ClearSelectMonthDataCommand = new Command(new Action<object>(OnClearSelectMonthDataCommand));
            InfoMenuActionCommand = new Command(new Action<object>(OnInfoMenuActionCommand));
            RefreshCommand = new Command(new Action<object>(OnRefreshCommand));

            Init();
        }



        private async void Init()
        {
            App = main.Data as AppModel;

            Date = DateTime.Now;

            TabbarData = new System.Collections.ObjectModel.ObservableCollection<string>()
            {
                "按天","按周","按月","按年"
            };

            var weekOptions = new List<SelectItemModel>();
            weekOptions.Add(new SelectItemModel()
            {
                Name = "本周"
            });
            weekOptions.Add(new SelectItemModel()
            {
                Name = "上周"
            });

            WeekOptions = weekOptions;

            SelectedWeek = weekOptions[0];

            MonthDate = DateTime.Now;

            TabbarSelectedIndex = 0;

            Date = DateTime.Now;

            YearDate = DateTime.Now;

            ChartDate = DateTime.Now;

            PropertyChanged += DetailPageVM_PropertyChanged;

            config = appConfig.GetConfig();

            await LoadData();

            await LoadInfo();

            LoadDayData();

            inputServicer.OnKeyUpInput += InputServicer_OnKeyUpInput;

            //  判断正则忽略
            var regexList = config.Behavior.IgnoreProcessList.Where(m => Regex.IsMatch(m, @"[\.|\*|\?|\{|\\|\[|\^|\|]"));
            foreach (string reg in regexList)
            {
                if (RegexHelper.IsMatch(App.Name, reg) || RegexHelper.IsMatch(App.File, reg))
                {
                    IsRegexIgnore = true;
                    break;
                }
            }
        }


        private async void InputServicer_OnKeyUpInput(object sender, System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.F5)
            {
                //  刷新
                LoadChartData();
                await LoadData();
            }
        }

        private async void OnRefreshCommand(object obj)
        {
            //  刷新
            LoadChartData();
            await LoadData();
        }

        private void OnInfoMenuActionCommand(object obj)
        {
            switch (obj.ToString())
            {
                case "copy processname":
                    Clipboard.SetText(App.Name);

                    break;
                case "copy process file":
                    Clipboard.SetText(App.File);
                    break;
                case "open dir":
                    if (File.Exists(App.File))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", "/select, " + App.File);
                    }
                    else
                    {
                        main.Toast("应用文件似乎不存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.IncidentTriangle);
                    }
                    break;
                case "open exe":
                    if (File.Exists(App.File))
                    {
                        System.Diagnostics.Process.Start(App.File);
                    }
                    else
                    {
                        main.Toast("应用似乎不存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.IncidentTriangle);
                    }
                    break;
                case "update description":
                    var app = appData.GetApp(App.Name);
                    app.Description = App.Description;
                    appData.UpdateApp(app);
                    break;
                    //case "geticon":
                    //    string iconFile = Iconer.ExtractFromFile(App.File, App.Name, App.Description, false);
                    //    if (string.IsNullOrEmpty(iconFile))
                    //    {
                    //        main.Toast("图标获取失败", Controls.Window.ToastType.Error, Controls.Base.IconTypes.Blocked);
                    //        return;
                    //    }
                    //    var app = appData.GetApp(App.ID);
                    //    if (app != null)
                    //    {
                    //        app.IconFile = iconFile;
                    //        appData.UpdateApp(app);
                    //        main.Toast("图标已更新", Controls.Window.ToastType.Success, Controls.Base.IconTypes.Accept);
                    //    }

                    //    break;
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
                config.Behavior.IgnoreProcessList.Add(App.Name);
                IsIgnore = true;
                main.Toast("应用已忽略", Controls.Window.ToastType.Success);
            }
            else
            {
                config.Behavior.IgnoreProcessList.Remove(App.Name);
                IsIgnore = false;
                main.Toast("已取消忽略", Controls.Window.ToastType.Success);
            }

            appConfig.Save();
        }

        private async void DetailPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Date))
            {
                await LoadData();
            }

            if (e.PropertyName == nameof(Category))
            {
                if (App.Category == null && Category != null || Category != null && App.Category.Name != Category.Name)
                {
                    App.Category = Category.Data as CategoryModel;

                    var app = appData.GetApp(App.ID);
                    if (app != null)
                    {
                        app.CategoryID = App.Category.ID;
                        appData.UpdateApp(app);
                    }
                }

            }

            //  处理图表数据加载
            if (e.PropertyName == nameof(ChartDate))
            {
                LoadDayData();
            }
            if (e.PropertyName == nameof(TabbarSelectedIndex))
            {
                LoadChartData();

            }
            if (e.PropertyName == nameof(SelectedWeek))
            {
                LoadWeekData();
            }
            if (e.PropertyName == nameof(MonthDate))
            {
                LoadMonthlyData();
            }
            if (e.PropertyName == nameof(YearDate))
            {
                LoadYearData();
            }
        }

        /// <summary>
        /// 加载柱状图图表数据
        /// </summary>
        private void LoadChartData()
        {
            if (TabbarSelectedIndex == 0)
            {
                NameIndexStart = 0;

                LoadDayData();
            }
            else if (TabbarSelectedIndex == 1)
            {
                LoadWeekData();
            }
            else if (TabbarSelectedIndex == 2)
            {
                NameIndexStart = 1;

                LoadMonthlyData();
            }
            else if (TabbarSelectedIndex == 3)
            {
                LoadYearData();
            }
        }

        private void LoadCategorys(string categoryName)
        {
            var list = new List<SelectItemModel>();
            foreach (var item in categories.GetCategories())
            {
                var option = new SelectItemModel()
                {
                    Data = item,
                    Img = item.IconFile,
                    Name = item.Name,
                };
                list.Add(option);
                if (categoryName == option.Name)
                {
                    Category = option;
                }
            }

            Categorys = list;

        }

        private async Task LoadInfo()
        {
            await Task.Run(() =>
            {
                if (App != null)
                {

                    //  判断是否是忽略的进程
                    IsIgnore = config.Behavior.IgnoreProcessList.Contains(App.Name);

                    LoadCategorys(App.Category?.Name);

                    //  读取关联应用数据
                    var link = config.Links.Where(m => m.ProcessList.Contains(App.Name)).FirstOrDefault();
                    if (link != null)
                    {
                        LinkApps = appData.GetAllApps().Where(m => link.ProcessList.Contains(m.Name) && m.Name != App.Name).ToList();
                    }
                }
            }
            );
        }

        private async Task LoadData()
        {
            await Task.Run(() =>
            {
                if (App != null)
                {

                    var monthData = data.GetProcessMonthLogList(App.ID, Date);
                    int monthTotal = monthData.Sum(m => m.Time);
                    Total = Time.ToString(monthTotal);

                    DateTime start = new DateTime(Date.Year, Date.Month, 1);
                    DateTime end = new DateTime(Date.Year, Date.Month, DateTime.DaysInMonth(Date.Year, Date.Month));
                    var monthAllData = data.GetDateRangelogList(start, end);

                    LongDay = "暂无数据";
                    if (monthData.Count > 0)
                    {
                        var longDayData = monthData.OrderByDescending(m => m.Time).FirstOrDefault();
                        LongDay = longDayData.Date.ToString("最长一天是在 dd 号，使用了 " + Time.ToString(longDayData.Time));

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
                if (App != null)
                {
                    main.Toast("正在处理");
                    data.Clear(App.ID, Date);

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
                bindModel.Name = string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                bindModel.Value = item.Time;
                bindModel.Tag = Time.ToString(item.Time);
                bindModel.PopupText = item.AppModel.File;
                bindModel.Icon = item.AppModel.IconFile;
                bindModel.DateTime = item.Date;
                resData.Add(bindModel);
            }

            return resData;
        }

        #region 柱状图图表数据加载
        /// <summary>
        /// 加载天数据
        /// </summary>
        private void LoadDayData()
        {
            DataMaximum = 3600;
            Task.Run(() =>
            {
                var list = data.GetAppDayData(App.ID, ChartDate);

                var chartData = new List<ChartsDataModel>();

                foreach (var item in list)
                {

                    chartData.Add(new ChartsDataModel()
                    {
                        Name = App.Description,
                        Icon = App.IconFile,
                        Values = item.Values,
                    });
                }

                ChartData = chartData;
            });
        }

        /// <summary>
        /// 加载周数据
        /// </summary>
        private void LoadWeekData()
        {
            DataMaximum = 0;
            Task.Run(() =>
            {
                var weekDateArr = SelectedWeek.Name == "本周" ? Time.GetThisWeekDate() : Time.GetLastWeekDate();

                WeekDateStr = weekDateArr[0].ToString("yyyy年MM月dd日") + " 到 " + weekDateArr[1].ToString("yyyy年MM月dd日");

                var list = data.GetAppRangeData(App.ID, weekDateArr[0], weekDateArr[1]);

                var chartData = new List<ChartsDataModel>();

                string[] weekNames = { "周一", "周二", "周三", "周四", "周五", "周六", "周日", };
                foreach (var item in list)
                {

                    chartData.Add(new ChartsDataModel()
                    {

                        Name = App.Description,
                        Icon = App.IconFile,
                        Values = item.Values,
                        ColumnNames = weekNames
                    });
                }

                ChartData = chartData;
            });
        }

        /// <summary>
        /// 加载月数据
        /// </summary>
        private void LoadMonthlyData()
        {
            DataMaximum = 0;
            Task.Run(() =>
            {
                var dateArr = Time.GetMonthDate(MonthDate);

                var list = data.GetAppRangeData(App.ID, dateArr[0], dateArr[1]);

                var chartData = new List<ChartsDataModel>();

                foreach (var item in list)
                {

                    chartData.Add(new ChartsDataModel()
                    {

                        Name = App.Description,
                        Icon = App.IconFile,
                        Values = item.Values,
                    });
                }

                ChartData = chartData;

            });

        }

        /// <summary>
        /// 加载年份数据
        /// </summary>
        private void LoadYearData()
        {
            DataMaximum = 0;
            Task.Run(() =>
            {
                var list = data.GetAppYearData(App.ID, YearDate);

                var chartData = new List<ChartsDataModel>();

                string[] names = new string[12];
                for (int i = 0; i < 12; i++)
                {
                    names[i] = (i + 1) + "月";
                }

                foreach (var item in list)
                {

                    chartData.Add(new ChartsDataModel()
                    {
                        Name = App.Description,
                        Icon = App.IconFile,
                        Values = item.Values,
                        ColumnNames = names
                    });
                }

                ChartData = chartData;

            });
        }
        #endregion
    }
}
