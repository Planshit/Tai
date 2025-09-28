using Core.Librarys;
using Core.Librarys.Image;
using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.Models;
using UI.Servicers;
using UI.Views;
using static UI.Controls.SettingPanel.SettingPanel;

namespace UI.ViewModels
{
    public class DataPageVM : DataPageModel
    {
        public Command ToDetailCommand { get; set; }
        private readonly IData data;
        private readonly MainViewModel main;
        private readonly IAppContextMenuServicer appContextMenuServicer;
        private readonly IAppConfig appConfig;
        private readonly IWebData _webData;
        private readonly IWebSiteContextMenuServicer _webSiteContextMenu;

        public DataPageVM(
            ILocalizationServicer localizationServicer,
            IData data,
            MainViewModel main,
            IAppContextMenuServicer appContextMenuServicer,
            IAppConfig appConfig,
            IWebData webData,
            IWebSiteContextMenuServicer webSiteContextMenu) : base(localizationServicer)
        {
            this.data = data;
            this.main = main;
            this.appContextMenuServicer = appContextMenuServicer;
            this.appConfig = appConfig;
            _webData = webData;
            _webSiteContextMenu = webSiteContextMenu;

            ToDetailCommand = new Command(new Action<object>(OnTodetailCommand));

            Init();
        }

        public override void Dispose()
        {
            PropertyChanged -= DataPageVM_PropertyChanged;

            base.Dispose();
        }
        private void Init()
        {
            //LoadData(DateTime.Now.Date);
            PropertyChanged += DataPageVM_PropertyChanged;

            TabbarData = new System.Collections.ObjectModel.ObservableCollection<string>()
            {
                Translated("time.daily"),
                Translated("time.monthly"),
                Translated("time.yearly")
            };

            TabbarSelectedIndex = 0;

            AppContextMenu = appContextMenuServicer.GetContextMenu();
        }

        private void OnTodetailCommand(object obj)
        {
            var data = obj as ChartsDataModel;

            if (data != null)
            {
                var model = data.Data as DailyLogModel;
                if (model != null && model.AppModel != null)
                {
                    main.Data = model.AppModel;
                    main.Uri = nameof(DetailPage);
                }
                else
                {
                    main.Data = data.Data as Core.Models.Db.WebSiteModel;
                    main.Uri = nameof(WebSiteDetailPage);
                }
            }
        }

        private void DataPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(DayDate))
            {
                LoadData(DayDate);
            }
            else if (e.PropertyName == nameof(MonthDate))
            {
                LoadData(MonthDate);
            }
            else if (e.PropertyName == nameof(YearDate))
            {
                LoadData(YearDate);
            }
            else if (e.PropertyName == nameof(TabbarSelectedIndex))
            {
                if (TabbarSelectedIndex == 0)
                {
                    if (DayDate == DateTime.MinValue)
                    {
                        DayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    }

                }
                else if (TabbarSelectedIndex == 1)
                {
                    if (MonthDate == DateTime.MinValue)
                    {
                        MonthDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    }
                }
                else if (TabbarSelectedIndex == 2)
                {
                    if (YearDate == DateTime.MinValue)
                    {
                        YearDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    }
                }
            }
            else if (e.PropertyName == nameof(ShowType))
            {
                LoadData(DayDate, 0);
                LoadData(MonthDate, 1);
                LoadData(YearDate, 2);
                if (ShowType.Id == 0)
                {
                    AppContextMenu = appContextMenuServicer.GetContextMenu();
                }
                else
                {
                    AppContextMenu = _webSiteContextMenu.GetContextMenu();
                }
            }
        }



        #region 读取数据




        private async void LoadData(DateTime date, int dataType_ = -1)
        {
            await Task.Run(() =>
            {
                DateTime dateStart = date, dateEnd = date;

                dataType_ = dataType_ == -1 ? TabbarSelectedIndex : dataType_;

                if (dataType_ == 1)
                {
                    dateStart = new DateTime(date.Year, date.Month, 1);
                    dateEnd = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                }
                else if (dataType_ == 0)
                {
                    dateStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                    dateEnd = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                }
                else if (dataType_ == 2)
                {
                    dateStart = new DateTime(date.Year, 1, 1, 0, 0, 0);
                    dateEnd = new DateTime(date.Year, 12, DateTime.DaysInMonth(date.Year, 12), 23, 59, 59);
                }

                List<ChartsDataModel> chartData = new List<ChartsDataModel>();
                if (ShowType.Id == 0)
                {
                    var result = data.GetDateRangelogList(dateStart, dateEnd);
                    chartData = MapToChartsData(result);
                }
                else
                {
                    var result = _webData.GetWebSiteLogList(dateStart, dateEnd);
                    chartData = MapToChartsWebData(result);
                }


                if (dataType_ == 0)
                {
                    Data = chartData;
                }
                else if (dataType_ == 1)
                {
                    MonthData = chartData;
                }
                else
                {
                    YearData = chartData;
                }

            });
        }

        #region 处理数据
        private List<ChartsDataModel> MapToChartsData(IEnumerable<Core.Models.DailyLogModel> list)
        {
            var resData = new List<ChartsDataModel>();
            try
            {
                var config = appConfig.GetConfig();

                foreach (var item in list)
                {
                    var bindModel = new ChartsDataModel();
                    bindModel.Data = item;
                    bindModel.Name = !string.IsNullOrEmpty(item.AppModel?.Alias) ? item.AppModel.Alias : string.IsNullOrEmpty(item.AppModel?.Description) ? item.AppModel.Name : item.AppModel.Description;
                    bindModel.Value = item.Time;
                    bindModel.Tag = Translated(Time.ToString(item.Time));
                    bindModel.PopupText = item.AppModel?.File;
                    bindModel.Icon = item.AppModel?.IconFile;
                    bindModel.BadgeList = new List<ChartBadgeModel>();
                    if (item.AppModel.Category != null)
                    {
                        bindModel.BadgeList.Add(new ChartBadgeModel()
                        {
                            Name = item.AppModel.Category.Name,
                            Color = item.AppModel.Category.Color,
                            Type = ChartBadgeType.Category
                        });
                    }
                    if (config.Behavior.IgnoreProcessList.Contains(item.AppModel.Name))
                    {
                        bindModel.BadgeList.Add(ChartBadgeModel.IgnoreBadge);
                    }
                    resData.Add(bindModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

            return resData;
        }

        private List<ChartsDataModel> MapToChartsWebData(IEnumerable<Core.Models.Db.WebSiteModel> list)
        {
            var resData = new List<ChartsDataModel>();
            try
            {
                var config = appConfig.GetConfig();

                foreach (var item in list)
                {
                    var bindModel = new ChartsDataModel();
                    bindModel.Data = item;
                    bindModel.Name = !string.IsNullOrEmpty(item.Alias) ? item.Alias : item.Title;
                    bindModel.Value = item.Duration;
                    bindModel.Tag = Translated(Time.ToString(item.Duration));
                    bindModel.PopupText = item.Domain;
                    bindModel.Icon = item.IconFile;
                    bindModel.BadgeList = new List<ChartBadgeModel>();
                    if (item.Category != null)
                    {
                        bindModel.BadgeList.Add(new ChartBadgeModel()
                        {
                            Name = item.Category.Name,
                            Color = item.Category.Color,
                            Type = ChartBadgeType.Category
                        });
                    }
                    if (config.Behavior.IgnoreURLList.Contains(item.Domain))
                    {
                        bindModel.BadgeList.Add(ChartBadgeModel.IgnoreBadge);
                    }
                    resData.Add(bindModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
            return resData;
        }
        #endregion
        #endregion
    }
}
