using Core.Librarys;
using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Command ToDetailCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public List<SelectItemModel> ChartDataModeOptions { get; set; }
        public ChartPageVM(IData data, ICategorys categorys, MainViewModel mainVM, IAppContextMenuServicer appContextMenuServicer, IInputServicer inputServicer)
        {
            this.data = data;
            this.categorys = categorys;
            this.mainVM = mainVM;
            this.appContextMenuServicer = appContextMenuServicer;
            this.inputServicer = inputServicer;


            ToDetailCommand = new Command(new Action<object>(OnTodetailCommand));
            RefreshCommand = new Command(new Action<object>(OnRefreshCommand));

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

            var chartDataModeOptions = new List<SelectItemModel>
            {
                new SelectItemModel()
                {
                    Id = 1,
                    Name = "默认视图",
                },
                new SelectItemModel()
                {
                    Id = 2,
                    Name = "汇总视图"
                },
                new SelectItemModel()
                {
                    Id = 3,
                    Name = "分类视图"
                }
            };

            ChartDataModeOptions = chartDataModeOptions;
            ChartDataMode = chartDataModeOptions[0];

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
            DataMaximum = 3600;
            Task.Run(() =>
            {

                var chartData = new List<ChartsDataModel>();
                var sumData = new List<ChartsDataModel>();

                var list = data.GetCategoryHoursData(Date);
                var nullCategory = new CategoryModel()
                {
                    ID = 0,
                    Name = "未分类",
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
                TotalHours = Time.ToHoursString(totalUse);
                LoadTopData();
            });
        }

        /// <summary>
        /// 加载周数据
        /// </summary>
        private void LoadWeekData()
        {
            ColumnSelectedIndex = -1;
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
                    Name = "未分类",
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
                TotalHours = Time.ToHoursString(totalUse);
            });


            LoadTopData();

        }

        /// <summary>
        /// 加载月数据
        /// </summary>
        private void LoadMonthlyData()
        {
            ColumnSelectedIndex = -1;
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
                    Name = "未分类",
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
                TotalHours = Time.ToHoursString(totalUse);
                LoadTopData();
            });

        }

        /// <summary>
        /// 加载年份数据
        /// </summary>
        private void LoadYearData()
        {
            ColumnSelectedIndex = -1;
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
                    Name = "未分类",
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
                TotalHours = Time.ToHoursString(totalUse);
                LoadTopData();
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

                AppCount = data.GetDateRangeAppCount(dateStart, dateEnd).ToString();
            });
        }

        private List<ChartsDataModel> MapToChartsData(IEnumerable<Core.Models.DailyLogModel> list)
        {
            var resData = new List<ChartsDataModel>();

            foreach (var item in list)
            {

                var bindModel = new ChartsDataModel();
                bindModel.Data = item;
                bindModel.Name = string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                bindModel.Value = item.Time;
                bindModel.Tag = Time.ToString(item.Time);
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
                        bindModel.Name = string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                        bindModel.Value = item.Time;
                        bindModel.Tag = Time.ToString(item.Time);
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
                        bindModel.Name = string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                        bindModel.Value = item.Time;
                        bindModel.Tag = Time.ToString(item.Time);
                        bindModel.PopupText = item.AppModel?.File;
                        bindModel.Icon = item.AppModel?.IconFile;
                        chartsDatas.Add(bindModel);
                    }
                }

                DayHoursData = chartsDatas;
            });
        }


    }
}
