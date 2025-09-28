using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UI.Controls.Charts.Model;
using UI.Controls.Select;
using UI.Servicers;

namespace UI.Models
{
    public class ChartPageModel : ModelBase
    {
        private List<ChartsDataModel> Data_;

        public List<ChartsDataModel> Data
        {
            get
            {
                return Data_;
            }
            set
            {
                Data_ = value;
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
        private DateTime Date_;
        public DateTime Date { get { return Date_; } set { Date_ = value; OnPropertyChanged(); } }

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

        private List<ChartsDataModel> TopData_;
        public List<ChartsDataModel> TopData
        {
            get { return TopData_; }
            set { TopData_ = value; OnPropertyChanged(); }
        }

        private double DataMaximum_ = 0;
        public double DataMaximum { get { return DataMaximum_; } set { DataMaximum_ = value; OnPropertyChanged(); } }

        private ContextMenu AppContextMenu_;
        public ContextMenu AppContextMenu { get { return AppContextMenu_; } set { AppContextMenu_ = value; OnPropertyChanged(); } }

        private string TotalHours_;
        public string TotalHours { get { return TotalHours_; } set { TotalHours_ = value; OnPropertyChanged(); } }

        private string TopHours_;
        public string TopHours { get { return TopHours_; } set { TopHours_ = value; OnPropertyChanged(); } }
        private string AppCount_;
        public string AppCount { get { return AppCount_; } set { AppCount_ = value; OnPropertyChanged(); } }

        private int ColumnSelectedIndex_ = -1;
        /// <summary>
        /// 柱状图选中列索引
        /// </summary>
        public int ColumnSelectedIndex { get { return ColumnSelectedIndex_; } set { ColumnSelectedIndex_ = value; OnPropertyChanged(); } }
        private List<ChartsDataModel> DayHoursData_;
        /// <summary>
        /// 指定时段app数据
        /// </summary>
        public List<ChartsDataModel> DayHoursData
        {
            get { return DayHoursData_; }
            set { DayHoursData_ = value; OnPropertyChanged(); }
        }
        private string DayHoursSelectedTime_;
        /// <summary>
        /// 选中时段
        /// </summary>
        public string DayHoursSelectedTime { get { return DayHoursSelectedTime_; } set { DayHoursSelectedTime_ = value; OnPropertyChanged(); } }
        private bool IsCanColumnSelect_ = true;
        /// <summary>
        /// 是否允许选择
        /// </summary>
        public bool IsCanColumnSelect { get { return IsCanColumnSelect_; } set { IsCanColumnSelect_ = value; OnPropertyChanged(); } }

        private SelectItemModel ChartDataMode_;
        /// <summary>
        /// 图表数据模式（1=分类/2=汇总）
        /// </summary>
        public SelectItemModel ChartDataMode { get { return ChartDataMode_; } set { ChartDataMode_ = value; OnPropertyChanged(); } }

        private List<ChartsDataModel> RadarData_;
        /// <summary>
        /// 雷达图数据
        /// </summary>
        public List<ChartsDataModel> RadarData { get { return RadarData_; } set { RadarData_ = value; OnPropertyChanged(); } }
        private bool IsChartStack_ = true;
        /// <summary>
        /// 是否以堆叠形式展示
        /// </summary>
        public bool IsChartStack { get { return IsChartStack_; } set { IsChartStack_ = value; OnPropertyChanged(); } }

        private Core.Models.AppModel Top1App_;
        /// <summary>
        /// 使用时间最长的应用
        /// </summary>
        public Core.Models.AppModel Top1App { get { return Top1App_; } set { Top1App_ = value; OnPropertyChanged(); } }

        private string DiffTotalTimeType_;
        /// <summary>
        /// 时长对比上一周期变化（0无变化，1增加，-1减少）
        /// </summary>
        public string DiffTotalTimeType { get { return DiffTotalTimeType_; } set { DiffTotalTimeType_ = value; OnPropertyChanged(); } }
        private string DiffAppCountType_;
        /// <summary>
        /// 应用量对比上一周期变化（0无变化，1增加，-1减少）
        /// </summary>
        public string DiffAppCountType { get { return DiffAppCountType_; } set { DiffAppCountType_ = value; OnPropertyChanged(); } }
        private string DiffTotalTimeValue_;
        /// <summary>
        /// 时长差异值
        /// </summary>
        public string DiffTotalTimeValue { get { return DiffTotalTimeValue_; } set { DiffTotalTimeValue_ = value; OnPropertyChanged(); } }
        private string DiffAppCountValue_;
        /// <summary>
        /// 应用量差异值
        /// </summary>
        public string DiffAppCountValue { get { return DiffAppCountValue_; } set { DiffAppCountValue_ = value; OnPropertyChanged(); } }

        //private SelectItemModel ShowType_;
        /// <summary>
        /// 展示数据类型（0=应用/1=网页）
        /// </summary>
        //public SelectItemModel ShowType { get { return ShowType_; } set { ShowType_ = value; OnPropertyChanged(); } }

        private List<ChartsDataModel> WebCategoriesPieData_;
        /// <summary>
        /// 网页分类统计饼图数据
        /// </summary>
        public List<ChartsDataModel> WebCategoriesPieData { get { return WebCategoriesPieData_; } set { WebCategoriesPieData_ = value; OnPropertyChanged(); } }

        private List<ChartsDataModel> WebBrowseStatisticsData_;
        /// <summary>
        /// 网页浏览时长统计数据（柱状图）
        /// </summary>
        public List<ChartsDataModel> WebBrowseStatisticsData { get { return WebBrowseStatisticsData_; } set { WebBrowseStatisticsData_ = value; OnPropertyChanged(); } }

        #region 网页数据
        private string WebTotalTimeText_;
        /// <summary>
        /// 网页浏览总时长
        /// </summary>
        public string WebTotalTimeText { get { return WebTotalTimeText_; } set { WebTotalTimeText_ = value; OnPropertyChanged(); } }
        private double WebTotalTime_;
        /// <summary>
        /// 网页浏览总时长
        /// </summary>
        public double WebTotalTime { get { return WebTotalTime_; } set { WebTotalTime_ = value; OnPropertyChanged(); } }
        private double WebSiteCount_;
        /// <summary>
        /// 站点浏览数量
        /// </summary>
        public double WebSiteCount { get { return WebSiteCount_; } set { WebSiteCount_ = value; OnPropertyChanged(); } }
        private double WebPageCount_;
        /// <summary>
        /// 网页浏览数量
        /// </summary>
        public double WebPageCount { get { return WebPageCount_; } set { WebPageCount_ = value; OnPropertyChanged(); } }

        private double LastWebTotalTime_;
        /// <summary>
        /// 上一个周期网页浏览总时长
        /// </summary>
        public double LastWebTotalTime { get { return LastWebTotalTime_; } set { LastWebTotalTime_ = value; OnPropertyChanged(); } }
        private double LastWebSiteCount_;
        /// <summary>
        /// 上一个周期站点浏览数量
        /// </summary>
        public double LastWebSiteCount { get { return LastWebSiteCount_; } set { LastWebSiteCount_ = value; OnPropertyChanged(); } }
        private double LastWebPageCount_;
        /// <summary>
        /// 上一个周期网页浏览数量
        /// </summary>
        public double LastWebPageCount { get { return LastWebPageCount_; } set { LastWebPageCount_ = value; OnPropertyChanged(); } }

        private List<ChartsDataModel> WebSitesTopData_;
        /// <summary>
        /// 网页站点最为频繁
        /// </summary>
        public List<ChartsDataModel> WebSitesTopData { get { return WebSitesTopData_; } set { WebSitesTopData_ = value; OnPropertyChanged(); } }
        private int WebColSelectedIndex_ = -1;
        /// <summary>
        /// 网页浏览数据柱状图选中列索引
        /// </summary>
        public int WebColSelectedIndex { get { return WebColSelectedIndex_; } set { WebColSelectedIndex_ = value; OnPropertyChanged(); } }

        private List<ChartsDataModel> WebSitesColSelectedData_;
        /// <summary>
        /// 网页浏览数据柱状图选中列数据
        /// </summary>
        public List<ChartsDataModel> WebSitesColSelectedData { get { return WebSitesColSelectedData_; } set { WebSitesColSelectedData_ = value; OnPropertyChanged(); } }

        private string WebSitesColSelectedTimeText_;
        /// <summary>
        /// 网页浏览数据柱状图选择列时间
        /// </summary>
        public string WebSitesColSelectedTimeText { get { return WebSitesColSelectedTimeText_; } set { WebSitesColSelectedTimeText_ = value; OnPropertyChanged(); } }
        private ContextMenu WebSiteContextMenu_;
        /// <summary>
        /// 网站右键菜单
        /// </summary>
        public ContextMenu WebSiteContextMenu { get { return WebSiteContextMenu_; } set { WebSiteContextMenu_ = value; OnPropertyChanged(); } }
        #endregion

        protected ChartPageModel(ILocalizationServicer localizationServicer) : base(localizationServicer) {}
    }
}
