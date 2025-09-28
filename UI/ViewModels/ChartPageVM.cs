using Core.Librarys;
using Core.Models;
using Core.Models.Db;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.Controls.Select;
using UI.Models;
using UI.Servicers;
using UI.Views;

namespace UI.ViewModels
{
    public class ChartPageVM : ChartPageModel
    {
        private readonly ICategorys categorys;
        private readonly IData data;
        private readonly MainViewModel mainVM;
        private readonly IAppContextMenuServicer appContextMenuServicer;
        private readonly IInputServicer inputServicer;
        private readonly IWebData _webData;
        private readonly IWebSiteContextMenuServicer _webSiteContextMenu;

        private double totalTime_ = 0;
        private int appCount_ = 0;
        public Command ToDetailCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public List<SelectItemModel> ChartDataModeOptions { get; set; }

        public ChartPageVM(
            ILocalizationServicer localizationServicer,
            IData data,
            ICategorys categorys,
            MainViewModel mainVM,
            IAppContextMenuServicer appContextMenuServicer,
            IInputServicer inputServicer,
            IWebData webData_,
            IWebSiteContextMenuServicer webSiteContextMenu_) : base(localizationServicer)
        {
            this.data = data;
            this.categorys = categorys;
            this.mainVM = mainVM;
            this.appContextMenuServicer = appContextMenuServicer;
            this.inputServicer = inputServicer;
            _webData = webData_;
            _webSiteContextMenu = webSiteContextMenu_;

            ToDetailCommand = new Command(new Action<object>(OnTodetailCommand));
            RefreshCommand = new Command(new Action<object>(OnRefreshCommand));

            TabbarData = new System.Collections.ObjectModel.ObservableCollection<string>()
            {
                Translated("time.daily"),
                Translated("time.weekly"),
                Translated("time.monthly"),
                Translated("time.yearly")
            };

            var weekOptions = new List<SelectItemModel>();
            weekOptions.Add(new SelectItemModel()
            {
                Name = Translated("page.chart.thisWeek")
            });
            weekOptions.Add(new SelectItemModel()
            {
                Name = Translated("page.chart.lastWeek")
            });

            var chartDataModeOptions = new List<SelectItemModel>
            {
                new SelectItemModel()
                {
                    Id = 1,
                    Name = Translated("page.chart.view.default")
                },
                new SelectItemModel()
                {
                    Id = 2,
                    Name = Translated("page.chart.view.summary")
                },
                new SelectItemModel()
                {
                    Id = 3,
                    Name = Translated("page.chart.view.categories")
                }
            };

            ChartDataModeOptions = chartDataModeOptions;
            ChartDataMode = chartDataModeOptions[0];
            //ShowType = ShowTypeOptions[0];
            WeekOptions = weekOptions;
            SelectedWeek = weekOptions[0];
            MonthDate = DateTime.Now;
            TabbarSelectedIndex = 0;
            Date = DateTime.Now;
            YearDate = DateTime.Now;

            LoadDayData();

            PropertyChanged += ChartPageVM_PropertyChanged;
            inputServicer.OnKeyUpInput += InputServicer_OnKeyUpInput;
            AppContextMenu = appContextMenuServicer.GetContextMenu();
            WebSiteContextMenu = _webSiteContextMenu.GetContextMenu();
        }


        public override void Dispose()
        {
            base.Dispose();
            PropertyChanged -= ChartPageVM_PropertyChanged;
            Data = null;
            TopData = null;
        }
        private void OnTodetailCommand(object obj)
        {
            var data = obj as ChartsDataModel;

            if (data != null)
            {
                if (data.Data is WebSiteModel)
                {
                    mainVM.Data = data.Data;
                    mainVM.Uri = nameof(WebSiteDetailPage);
                }
                else
                {
                    var model = data.Data as DailyLogModel;
                    var app = model != null ? model.AppModel : null;

                    if (model == null)
                    {
                        app = (data.Data as HoursLogModel).AppModel;
                    }

                    if (app != null)
                    {
                        mainVM.Data = app;
                        mainVM.Uri = nameof(DetailPage);
                    }
                }
            }
        }
        private void OnRefreshCommand(object obj)
        {
            LoadData();
        }

        private void InputServicer_OnKeyUpInput(object sender, System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.F5)
            {
                LoadData();
            }
        }


        private void ChartPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Date))
            {
                LoadDayData();
            }
            if (e.PropertyName == nameof(TabbarSelectedIndex))
            {
                IsCanColumnSelect = true;
                LoadData();
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
            if (e.PropertyName == nameof(ColumnSelectedIndex))
            {
                LoadSelectedColData();
            }
            if (e.PropertyName == nameof(ChartDataMode))
            {
                if (ChartDataMode.Id == 1)
                {
                    IsChartStack = true;
                }
                else
                {
                    IsChartStack = false;
                }
                LoadData();
            }
            if (e.PropertyName == nameof(WebColSelectedIndex))
            {
                LoadWebSitesColSelectedData();
            }
        }


        private void LoadData()
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

            LoadSelectedColData();
        }

        /// <summary>
        /// 加载天数据
        /// </summary>
        private void LoadDayData()
        {
            ColumnSelectedIndex = -1;
            WebColSelectedIndex = -1;

            DataMaximum = 3600;
            Task.Run(() =>
            {
                //  应用数据
                var chartData = new List<ChartsDataModel>();
                var sumData = new List<ChartsDataModel>();

                var list = data.GetCategoryHoursData(Date);
                var nullCategory = new CategoryModel()
                {
                    ID = 0,
                    Name = Translated("page.chart.uncategorized"),
                    IconFile = "pack://application:,,,/Tai;component/Resources/Icons/tai32.ico"
                };
                foreach (var item in list)
                {
                    var category = categorys.GetCategory(item.CategoryID);
                    if (item.CategoryID == 0)
                    {
                        category = nullCategory;
                    }
                    if (category != null)
                    {
                        var dataItem = new ChartsDataModel()
                        {

                            Name = category.Name,
                            Icon = category.IconFile,
                            Values = item.Values,
                            Color = category.Color
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }
                if (ChartDataMode.Id == 1 || ChartDataMode.Id == 3)
                {
                    Data = chartData;
                }
                else
                {
                    //  汇总
                    var values = data.GetRangeTotalData(Date, Date);


                    var dataItem = new ChartsDataModel()
                    {
                        Values = values,
                    };

                    sumData.Add(dataItem);
                    Data = sumData;
                }

                RadarData = chartData;

                double totalUse = Data.Sum(m => m.Values.Sum());
                totalTime_ = totalUse;
                TotalHours = Time.ToHoursString(totalUse);
                LoadTopData();

                //  网页数据
                LoadWebData(Date, Date);
            });
        }

        /// <summary>
        /// 加载周数据
        /// </summary>
        private void LoadWeekData()
        {
            ColumnSelectedIndex = -1;
            WebColSelectedIndex = -1;

            DataMaximum = 0;
            Task.Run(() =>
            {
                var weekDateArr = SelectedWeek.Name == "本周" ? Time.GetThisWeekDate() : Time.GetLastWeekDate();
                WeekDateStr = weekDateArr[0].ToString("yyyy年MM月dd日") + " 到 " + weekDateArr[1].ToString("yyyy年MM月dd日");
                string[] weekNames = { "周一", "周二", "周三", "周四", "周五", "周六", "周日", };
                var chartData = new List<ChartsDataModel>();
                var sumData = new List<ChartsDataModel>();

                var list = data.GetCategoryRangeData(weekDateArr[0], weekDateArr[1]);
                var nullCategory = new CategoryModel()
                {
                    ID = 0,
                    Name = Translated("page.chart.uncategorized"),
                    IconFile = "pack://application:,,,/Tai;component/Resources/Icons/tai32.ico"

                };

                foreach (var item in list)
                {
                    var category = categorys.GetCategory(item.CategoryID);
                    if (item.CategoryID == 0)
                    {
                        category = nullCategory;
                    }
                    if (category != null)
                    {
                        var dataItem = new ChartsDataModel()
                        {

                            Name = category.Name,
                            Icon = category.IconFile,
                            Values = item.Values,
                            ColumnNames = weekNames,
                            Color = category.Color,
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }

                if (ChartDataMode.Id == 1 || ChartDataMode.Id == 3)
                {
                    Data = chartData;
                }
                else
                {
                    //  汇总
                    var values = data.GetRangeTotalData(weekDateArr[0], weekDateArr[1]);


                    var dataItem = new ChartsDataModel()
                    {
                        Values = values,
                        ColumnNames = weekNames,
                    };

                    sumData.Add(dataItem);
                    Data = sumData;
                }

                RadarData = chartData;
                double totalUse = Data.Sum(m => m.Values.Sum());
                totalTime_ = totalUse;
                TotalHours = Time.ToHoursString(totalUse);

                //  网页数据
                LoadWebData(weekDateArr[0], weekDateArr[1]);
            });


            LoadTopData();

        }

        /// <summary>
        /// 加载月数据
        /// </summary>
        private void LoadMonthlyData()
        {
            ColumnSelectedIndex = -1;
            WebColSelectedIndex = -1;
            DataMaximum = 0;
            Task.Run(() =>
            {
                var chartData = new List<ChartsDataModel>();
                var sumData = new List<ChartsDataModel>();
                var dateArr = Time.GetMonthDate(MonthDate);

                var list = data.GetCategoryRangeData(dateArr[0], dateArr[1]);

                var nullCategory = new CategoryModel()
                {
                    ID = 0,
                    Name = Translated("page.chart.uncategorized"),
                    IconFile = "pack://application:,,,/Tai;component/Resources/Icons/tai32.ico"
                };

                foreach (var item in list)
                {
                    var category = categorys.GetCategory(item.CategoryID);
                    if (item.CategoryID == 0)
                    {
                        category = nullCategory;
                    }
                    if (category != null)
                    {
                        var dataItem = new ChartsDataModel()
                        {

                            Name = category.Name,
                            Icon = category.IconFile,
                            Values = item.Values,
                            Color = category.Color
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }
                if (ChartDataMode.Id == 1 || ChartDataMode.Id == 3)
                {
                    Data = chartData;
                }
                else
                {
                    //  汇总
                    var values = data.GetRangeTotalData(dateArr[0], dateArr[1]);

                    var dataItem = new ChartsDataModel()
                    {
                        Values = values,
                    };
                    sumData.Add(dataItem);
                    Data = sumData;
                }

                RadarData = chartData;
                double totalUse = Data.Sum(m => m.Values.Sum());
                totalTime_ = totalUse;
                TotalHours = Time.ToHoursString(totalUse);
                LoadTopData();

                //  网页数据
                LoadWebData(dateArr[0], dateArr[1]);
            });

        }

        /// <summary>
        /// 加载年份数据
        /// </summary>
        private void LoadYearData()
        {
            ColumnSelectedIndex = -1;
            WebColSelectedIndex = -1;

            DataMaximum = 0;
            Task.Run(() =>
            {
                var chartData = new List<ChartsDataModel>();
                var sumData = new List<ChartsDataModel>();
                string[] names = new string[12];
                for (int i = 0; i < 12; i++)
                {
                    names[i] = (i + 1) + "月";
                }

                var list = data.GetCategoryYearData(YearDate);
                var nullCategory = new CategoryModel()
                {
                    ID = 0,
                    Name = Translated("page.chart.uncategorized"),
                    IconFile = "pack://application:,,,/Tai;component/Resources/Icons/tai32.ico"
                };

                foreach (var item in list)
                {
                    var category = categorys.GetCategory(item.CategoryID);
                    if (item.CategoryID == 0)
                    {
                        category = nullCategory;
                    }
                    if (category != null)
                    {
                        var dataItem = new ChartsDataModel()
                        {

                            Name = category.Name,
                            Icon = category.IconFile,
                            Values = item.Values,
                            ColumnNames = names,
                            Color = category.Color
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }




                if (ChartDataMode.Id == 1 || ChartDataMode.Id == 3)
                {
                    Data = chartData;
                }
                else
                {
                    //  汇总
                    var values = data.GetMonthTotalData(YearDate);
                    var dataItem = new ChartsDataModel()
                    {
                        Values = values,
                        ColumnNames = names,
                    };
                    sumData.Add(dataItem);

                    Data = sumData;
                }

                RadarData = chartData;

                double totalUse = Data.Sum(m => m.Values.Sum());
                totalTime_ = totalUse;
                TotalHours = Time.ToHoursString(totalUse);
                LoadTopData();

                //  网页数据
                var dateArr = Time.GetYearDate(YearDate);
                LoadWebData(dateArr[0], dateArr[1]);
            });
        }

        private void LoadTopData()
        {
            Task.Run(() =>
            {
                var dateStart = Date.Date;
                var dateEnd = Date.Date;
                if (TabbarSelectedIndex == 1)
                {
                    //  周
                    var weekDateArr = SelectedWeek.Name == "本周" ? Time.GetThisWeekDate() : Time.GetLastWeekDate();

                    dateStart = weekDateArr[0];
                    dateEnd = weekDateArr[1];

                }
                else if (TabbarSelectedIndex == 2)
                {
                    //  月
                    var dateArr = Time.GetMonthDate(MonthDate);
                    dateStart = dateArr[0];
                    dateEnd = dateArr[1];
                }
                else if (TabbarSelectedIndex == 3)
                {
                    //  年
                    var dateArr = Time.GetYearDate(YearDate);
                    dateStart = dateArr[0];
                    dateEnd = dateArr[1];
                }
                var list = data.GetDateRangelogList(dateStart, dateEnd, 5);

                TopData = MapToChartsData(list);

                TopHours = TopData.Count > 0 ? Time.ToHoursString(TopData[0].Value) : "0";

                appCount_ = data.GetDateRangeAppCount(dateStart, dateEnd);
                AppCount = appCount_.ToString();

                Top1App = null;

                if (TopData.Count > 0)
                {
                    var model = TopData[0].Data as DailyLogModel;
                    Top1App = model != null ? model.AppModel : null;
                }

                LoadDiffData();
            });
        }

        private void LoadDiffData()
        {
            var dateStart = Date.Date.AddDays(-1);
            var dateEnd = Date.Date.AddDays(-1);
            if (TabbarSelectedIndex == 1)
            {
                //  周
                var weekDateArr = Time.GetLastWeekDate();
                dateStart = weekDateArr[0];
                dateEnd = weekDateArr[1];
            }
            else if (TabbarSelectedIndex == 2)
            {
                //  月
                var dateArr = Time.GetMonthDate(MonthDate.AddMonths(-1));
                dateStart = dateArr[0];
                dateEnd = dateArr[1];
            }
            else if (TabbarSelectedIndex == 3)
            {
                //  年
                var dateArr = Time.GetYearDate(YearDate.AddYears(-1));
                dateStart = dateArr[0];
                dateEnd = dateArr[1];
            }

            //  应用量
            int lastAppCount = data.GetDateRangeAppCount(dateStart, dateEnd);
            //  使用总时长
            var lastTotalTime = data.GetRangeTotalData(dateStart, dateEnd).Sum();

            double diffTotalTime = ((totalTime_ - lastTotalTime) / lastTotalTime) * 100;
            if (totalTime_ > 0 && lastTotalTime == 0)
            {
                diffTotalTime = 100;
            }
            else if (totalTime_ == 0 && lastTotalTime == 0)
            {
                diffTotalTime = 0;
            }
            if (diffTotalTime > 0)
            {
                DiffTotalTimeType = "1";
            }
            else if (diffTotalTime < 0)
            {
                DiffTotalTimeType = "-1";
            }
            else
            {
                DiffTotalTimeType = "0";
            }
            DiffTotalTimeValue = DiffTotalTimeType == "0" ? string.Empty : diffTotalTime == 100 ? "100%" : Math.Abs(diffTotalTime).ToString("f2") + "%";

            int diffAppCount = appCount_ - lastAppCount;
            if (diffAppCount > 0)
            {
                DiffAppCountType = "1";
            }
            else if (diffAppCount < 0)
            {
                DiffAppCountType = "-1";
            }
            else
            {
                DiffAppCountType = "0";
            }
            DiffAppCountValue = DiffAppCountType == "0" ? string.Empty : Math.Abs(diffAppCount).ToString();

            LastWebTotalTime = _webData.GetBrowseDurationTotal(dateStart, dateEnd);
            LastWebSiteCount = _webData.GetBrowseSitesTotal(dateStart, dateEnd);
            LastWebPageCount = _webData.GetBrowsePagesTotal(dateStart, dateEnd);
        }

        private List<ChartsDataModel> MapToChartsData(IEnumerable<Core.Models.DailyLogModel> list)
        {
            var resData = new List<ChartsDataModel>();

            foreach (var item in list)
            {

                var bindModel = new ChartsDataModel();
                bindModel.Data = item;
                bindModel.Name = !string.IsNullOrEmpty(item.AppModel?.Alias) ? item.AppModel.Alias : string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                bindModel.Value = item.Time;
                bindModel.Tag = Translated(Time.ToString(item.Time));
                bindModel.PopupText = item.AppModel?.File;
                bindModel.Icon = item.AppModel?.IconFile;
                resData.Add(bindModel);
            }

            return resData;
        }

        private void LoadSelectedColData()
        {
            if (ColumnSelectedIndex < 0)
            {
                DayHoursSelectedTime = String.Empty;
                return;
            }
            Task.Run(() =>
            {
                IEnumerable<HoursLogModel> hoursModelList = new List<HoursLogModel>();
                IEnumerable<DailyLogModel> daysModelList = new List<DailyLogModel>();

                var chartsDatas = new List<ChartsDataModel>();

                if (TabbarSelectedIndex == 0)
                {
                    //  天
                    var time = new DateTime(Date.Year, Date.Month, Date.Day, ColumnSelectedIndex, 0, 0);
                    DayHoursSelectedTime = time.ToString("yyyy年MM月dd日 HH点");
                    hoursModelList = data.GetTimeRangelogList(time);
                }
                else if (TabbarSelectedIndex == 1)
                {
                    //  周
                    var weekDateArr = SelectedWeek.Name == "本周" ? Time.GetThisWeekDate() : Time.GetLastWeekDate();
                    var time = weekDateArr[0].AddDays(ColumnSelectedIndex);
                    DayHoursSelectedTime = time.ToString("yyyy年MM月dd日");
                    daysModelList = data.GetDateRangelogList(time, time);
                }
                else if (TabbarSelectedIndex == 2)
                {
                    //  月
                    var dateArr = Time.GetMonthDate(MonthDate);
                    var time = dateArr[0].AddDays(ColumnSelectedIndex);
                    DayHoursSelectedTime = time.ToString("yyyy年MM月dd日");
                    daysModelList = data.GetDateRangelogList(time, time);
                }
                else if (TabbarSelectedIndex == 3)
                {
                    //  年
                    var dateStart = new DateTime(YearDate.Year, ColumnSelectedIndex + 1, 1);
                    var dateEnd = new DateTime(dateStart.Year, dateStart.Month, DateTime.DaysInMonth(dateStart.Year, dateStart.Month), 23, 59, 59);

                    DayHoursSelectedTime = dateStart.ToString("yyyy年MM月");
                    daysModelList = data.GetDateRangelogList(dateStart, dateEnd);
                }

                if (TabbarSelectedIndex == 0)
                {
                    foreach (var item in hoursModelList)
                    {
                        var bindModel = new ChartsDataModel();
                        bindModel.Data = item;
                        bindModel.Name = !string.IsNullOrEmpty(item.AppModel?.Alias) ? item.AppModel.Alias : string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                        bindModel.Value = item.Time;
                        bindModel.Tag = Translated(Time.ToString(item.Time));
                        bindModel.PopupText = item.AppModel?.File;
                        bindModel.Icon = item.AppModel?.IconFile;
                        chartsDatas.Add(bindModel);
                    }
                }
                else
                {
                    foreach (var item in daysModelList)
                    {
                        var bindModel = new ChartsDataModel();
                        bindModel.Data = item;
                        bindModel.Name = !string.IsNullOrEmpty(item.AppModel?.Alias) ? item.AppModel.Alias : string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                        bindModel.Value = item.Time;
                        bindModel.Tag = Translated(Time.ToString(item.Time));
                        bindModel.PopupText = item.AppModel?.File;
                        bindModel.Icon = item.AppModel?.IconFile;
                        chartsDatas.Add(bindModel);
                    }
                }

                DayHoursData = chartsDatas;
            });
        }


        #region 网页数据
        private void LoadWebData(DateTime start_, DateTime end_)
        {
            LoadCategoriesStatistics(start_, end_);
            LoadWebBrowseDataStatistics(start_, end_);
            LoadWebSitesTopData(start_, end_);
            WebTotalTime = _webData.GetBrowseDurationTotal(start_, end_);
            WebSiteCount = _webData.GetBrowseSitesTotal(start_, end_);
            WebPageCount = _webData.GetBrowsePagesTotal(start_, end_);
            WebTotalTimeText = Time.ToHoursString(WebTotalTime);
        }

        /// <summary>
        /// 分类饼图
        /// </summary>
        /// <param name="start_"></param>
        /// <param name="end_"></param>
        private void LoadCategoriesStatistics(DateTime start_, DateTime end_)
        {
            Task.Run(() =>
            {
                var chartsDatas = new List<ChartsDataModel>();
                var data = _webData.GetCategoriesStatistics(start_, end_);
                foreach (var item in data)
                {
                    var category = _webData.GetWebSiteCategory(item.ID);
                    var bindModel = new ChartsDataModel();
                    bindModel.Name = item.ID == 0 ? Translated("page.chart.uncategorized") : item.Name;
                    bindModel.Value = item.Value;
                    bindModel.Data = item;
                    bindModel.Color = item.ID == 0 ? "#ccc" : category.Color;
                    bindModel.PopupText = bindModel.Name + " " + Translated(Time.ToString((int)item.Value));
                    bindModel.Icon = item.ID == 0 ? "" : category.IconFile;
                    chartsDatas.Add(bindModel);
                }
                WebCategoriesPieData = chartsDatas.OrderByDescending(m => m.Value).ToList();

            });
        }

        private void LoadWebBrowseDataStatistics(DateTime start_, DateTime end_)
        {
            Task.Run(() =>
            {
                var chartData = new List<ChartsDataModel>();
                var data = _webData.GetBrowseDataByCategoryStatistics(start_, end_);
                //  转换为图表格式数据
                var emptyCategory = new WebSiteCategoryModel()
                {
                    ID = 0,
                    Name = Translated("page.chart.uncategorized"),
                    IconFile = "pack://application:,,,/Tai;component/Resources/Icons/tai32.ico"
                };

                string[] colNames = { };

                if (TabbarSelectedIndex == 1)
                {
                    colNames = new string[] { "周一", "周二", "周三", "周四", "周五", "周六", "周日", };
                }
                else if (TabbarSelectedIndex == 3)
                {
                    colNames = new string[12];
                    for (int i = 0; i < 12; i++)
                    {
                        colNames[i] = (i + 1) + "月";
                    }
                }

                foreach (var item in data)
                {
                    var category = _webData.GetWebSiteCategory(item.CategoryID);
                    if (item.CategoryID == 0)
                    {
                        category = emptyCategory;
                    }
                    if (category != null)
                    {
                        var dataItem = new ChartsDataModel()
                        {

                            Name = category.Name,
                            Icon = category.IconFile,
                            Values = item.Values,
                            Color = category.Color,
                            ColumnNames = colNames
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }

                //var chartsDatas = new List<ChartsDataModel>();
                //var bindModel = new ChartsDataModel();
                //bindModel.Values = data.Select(m => m.Value).ToArray();
                //bindModel.Color = StateData.ThemeColor;
                //chartsDatas.Add(bindModel);
                WebBrowseStatisticsData = chartData;
            });
        }

        private void LoadWebSitesTopData(DateTime start_, DateTime end_)
        {
            Task.Run(() =>
            {
                var data = _webData.GetDateRangeWebSiteList(start_, end_, 10);
                WebSitesTopData = MapToChartData(data);
            });
        }
        private void LoadWebSitesColSelectedData()
        {
            if (WebColSelectedIndex < 0)
            {
                WebSitesColSelectedTimeText = String.Empty;
                WebSitesColSelectedData = new List<ChartsDataModel>();
                return;
            }
            Task.Run(() =>
            {

                var chartData = new List<ChartsDataModel>();
                bool isTime = false;
                DateTime startTime = DateTime.Now, endTime = DateTime.Now;
                if (TabbarSelectedIndex == 0)
                {
                    //  天
                    var time = new DateTime(Date.Year, Date.Month, Date.Day, WebColSelectedIndex, 0, 0);
                    WebSitesColSelectedTimeText = time.ToString("yyyy年MM月dd日 HH点");
                    isTime = true;
                    startTime = endTime = time;
                }
                else if (TabbarSelectedIndex == 1)
                {
                    //  周
                    var weekDateArr = SelectedWeek.Name == "本周" ? Time.GetThisWeekDate() : Time.GetLastWeekDate();
                    var time = weekDateArr[0].AddDays(WebColSelectedIndex);
                    WebSitesColSelectedTimeText = time.ToString("yyyy年MM月dd日");
                    startTime = endTime = time;
                }
                else if (TabbarSelectedIndex == 2)
                {
                    //  月
                    var dateArr = Time.GetMonthDate(MonthDate);
                    var time = dateArr[0].AddDays(WebColSelectedIndex);
                    WebSitesColSelectedTimeText = time.ToString("yyyy年MM月dd日");
                    startTime = endTime = time;
                }
                else if (TabbarSelectedIndex == 3)
                {
                    //  年
                    var dateStart = new DateTime(YearDate.Year, WebColSelectedIndex + 1, 1);
                    var dateEnd = new DateTime(dateStart.Year, dateStart.Month, DateTime.DaysInMonth(dateStart.Year, dateStart.Month), 23, 59, 59);

                    WebSitesColSelectedTimeText = dateStart.ToString("yyyy年MM月");

                    startTime = dateStart;
                    endTime = dateEnd;

                }


                chartData = MapToChartData(_webData.GetDateRangeWebSiteList(startTime, endTime, 0, -1, isTime));

                WebSitesColSelectedData = chartData;
            });
        }
        private List<ChartsDataModel> MapToChartData(IEnumerable<Core.Models.Db.WebSiteModel> list)
        {
            var resData = new List<ChartsDataModel>();

            foreach (var item in list)
            {
                var bindModel = new ChartsDataModel();
                bindModel.Data = item;
                bindModel.Name = !string.IsNullOrEmpty(item.Alias) ? item.Alias : item.Title;
                bindModel.Value = item.Duration;
                bindModel.Tag = Translated(Time.ToString(item.Duration));
                bindModel.PopupText = item.Domain;
                bindModel.Icon = item.IconFile;
                resData.Add(bindModel);
            }

            return resData;
        }
        #endregion
    }
}
