using Core.Librarys;
using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
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

        public Command ToDetailCommand { get; set; }

        public ChartPageVM(IData data, ICategorys categorys, MainViewModel mainVM, IAppContextMenuServicer appContextMenuServicer)
        {
            this.data = data;
            this.categorys = categorys;
            this.mainVM = mainVM;
            this.appContextMenuServicer = appContextMenuServicer;


            ToDetailCommand = new Command(new Action<object>(OnTodetailCommand));

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

            LoadDayData();

            PropertyChanged += ChartPageVM_PropertyChanged;

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
                if (model != null && model.AppModel != null)
                {
                    mainVM.Data = model.AppModel;
                    mainVM.Uri = nameof(DetailPage);
                }

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
        /// 加载天数据
        /// </summary>
        private void LoadDayData()
        {
            DataMaximum = 3600;
            Task.Run(() =>
            {


                var list = data.GetCategoryHoursData(Date);

                var chartData = new List<ChartsDataModel>();

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

                Data = chartData;

                LoadTopData();
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

                var list = data.GetCategoryRangeData(weekDateArr[0], weekDateArr[1]);

                var chartData = new List<ChartsDataModel>();

                var nullCategory = new CategoryModel()
                {
                    ID = 0,
                    Name = "未分类",
                    IconFile = "pack://application:,,,/Tai;component/Resources/Icons/tai32.ico"

                };

                string[] weekNames = { "周一", "周二", "周三", "周四", "周五", "周六", "周日", };
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
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }

                Data = chartData;
            });


            LoadTopData();

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

                var list = data.GetCategoryRangeData(dateArr[0], dateArr[1]);

                var chartData = new List<ChartsDataModel>();

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
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }

                Data = chartData;

                LoadTopData();
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
                var list = data.GetCategoryYearData(YearDate);

                var chartData = new List<ChartsDataModel>();

                string[] names = new string[12];
                for (int i = 0; i < 12; i++)
                {
                    names[i] = (i + 1) + "月";
                }

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
                        };
                        if (category.ID == 0)
                        {
                            dataItem.Color = "#E5F7F6F2";
                        }
                        chartData.Add(dataItem);
                    }
                }

                Data = chartData;

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
                var list = data.GetDateRangelogList(dateStart, dateEnd, 10);

                TopData = MapToChartsData(list);
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
    }
}
