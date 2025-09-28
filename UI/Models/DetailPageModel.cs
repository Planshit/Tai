using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UI.Controls.Charts.Model;
using UI.Controls.Select;
using UI.Servicers;

namespace UI.Models
{
    public class DetailPageModel : LocalizedView
    {

        private List<ChartsDataModel> Data_;
        /// <summary>
        /// data
        /// </summary>
        public List<ChartsDataModel> Data
        {
            get { return Data_; }
            set { Data_ = value; OnPropertyChanged(); }
        }
        private AppModel App_;
        public AppModel App { get { return App_; } set { App_ = value; OnPropertyChanged(); } }
        private bool IsLoading_;
        public bool IsLoading { get { return IsLoading_; } set { IsLoading_ = value; OnPropertyChanged(); } }

        private string Total_;
        public string Total { get { return Total_; } set { Total_ = value; OnPropertyChanged(); } }

        private string Ratio_;
        public string Ratio { get { return Ratio_; } set { Ratio_ = value; OnPropertyChanged(); } }

        private DateTime Date_;
        public DateTime Date { get { return Date_; } set { Date_ = value; OnPropertyChanged(); } }

        private string LongDay_;
        public string LongDay { get { return LongDay_; } set { LongDay_ = value; OnPropertyChanged(); } }


        private bool IsIgnore_;
        public bool IsIgnore
        {
            get { return IsIgnore_; }
            set
            {
                IsIgnore_ = value;
                if (IsIgnore)
                {
                    BlockBtnVisibility = Visibility.Collapsed;
                    CancelBlockBtnVisibility = Visibility.Visible;
                }
                else
                {
                    BlockBtnVisibility = Visibility.Visible;
                    CancelBlockBtnVisibility = Visibility.Collapsed;
                }
                OnPropertyChanged();
            }
        }

        private Visibility BlockBtnVisibility_ = Visibility.Collapsed;
        public Visibility BlockBtnVisibility { get { return BlockBtnVisibility_; } set { BlockBtnVisibility_ = value; OnPropertyChanged(); } }

        private Visibility CancelBlockBtnVisibility_ = Visibility.Collapsed;
        public Visibility CancelBlockBtnVisibility { get { return CancelBlockBtnVisibility_; } set { CancelBlockBtnVisibility_ = value; OnPropertyChanged(); } }

        private string TodayTime_;
        /// <summary>
        /// 今日使用时长
        /// </summary>
        public string TodayTime { get { return TodayTime_; } set { TodayTime_ = value; OnPropertyChanged(); } }

        private string Yesterday_;
        /// <summary>
        /// 相比昨日
        /// </summary>
        public string Yesterday { get { return Yesterday_; } set { Yesterday_ = value; OnPropertyChanged(); } }

        private List<SelectItemModel> Categorys_;
        /// <summary>
        /// 分类数据
        /// </summary>
        public List<SelectItemModel> Categorys { get { return Categorys_; } set { Categorys_ = value; OnPropertyChanged(); } }

        private SelectItemModel Category_;
        /// <summary>
        /// 当前分类
        /// </summary>
        public SelectItemModel Category { get { return Category_; } set { Category_ = value; OnPropertyChanged(); } }


        //  图表数据
        private List<ChartsDataModel> ChartData_;

        public List<ChartsDataModel> ChartData
        {
            get
            {
                return ChartData_;
            }
            set
            {
                ChartData_ = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> TabbarData_;
        /// <summary>
        /// tabbar data
        /// </summary>
        public ObservableCollection<string> TabbarData
        {
            get { return TabbarData_; }
            set { TabbarData_ = value; OnPropertyChanged(); }
        }

        private int TabbarSelectedIndex_;
        /// <summary>
        /// tabbar selected item index
        /// </summary>
        public int TabbarSelectedIndex
        {
            get { return TabbarSelectedIndex_; }
            set
            {
                TabbarSelectedIndex_ = value;
                OnPropertyChanged();
            }
        }
        private string WeekDateStr_;
        public string WeekDateStr { get { return WeekDateStr_; } set { WeekDateStr_ = value; OnPropertyChanged(); } }
        private DateTime ChartDate_;
        public DateTime ChartDate { get { return ChartDate_; } set { ChartDate_ = value; OnPropertyChanged(); } }

        private List<SelectItemModel> WeekOptions_;

        public List<SelectItemModel> WeekOptions { get { return WeekOptions_; } set { WeekOptions_ = value; OnPropertyChanged(); } }

        private SelectItemModel SelectedWeek_;

        public SelectItemModel SelectedWeek { get { return SelectedWeek_; } set { SelectedWeek_ = value; OnPropertyChanged(); } }

        private DateTime MonthDate_;
        public DateTime MonthDate { get { return MonthDate_; } set { MonthDate_ = value; OnPropertyChanged(); } }

        private DateTime YearDate_;
        public DateTime YearDate { get { return YearDate_; } set { YearDate_ = value; OnPropertyChanged(); } }

        private int NameIndexStart_ = 0;
        public int NameIndexStart { get { return NameIndexStart_; } set { NameIndexStart_ = value; OnPropertyChanged(); } }
        private double DataMaximum_ = 0;
        public double DataMaximum { get { return DataMaximum_; } set { DataMaximum_ = value; OnPropertyChanged(); } }

        private List<AppModel> LinkApps_;
        /// <summary>
        /// 关联的应用
        /// </summary>
        public List<AppModel> LinkApps { get { return LinkApps_; } set { LinkApps_ = value; OnPropertyChanged(); } }
        private bool IsRegexIgnore_;
        /// <summary>
        /// 是否已被正则忽略
        /// </summary>
        public bool IsRegexIgnore { get { return IsRegexIgnore_; } set { IsRegexIgnore_ = value; OnPropertyChanged(); } }
        private ContextMenu AppContextMenu_;
        /// <summary>
        /// 应用右键菜单
        /// </summary>
        public ContextMenu AppContextMenu { get { return AppContextMenu_; } set { AppContextMenu_ = value; OnPropertyChanged(); } }
        
        protected DetailPageModel(ILocalizationServicer localization) : base(localization) {}
    }
}
