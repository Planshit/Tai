using Core.Models.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UI.Controls.Charts.Model;
using UI.Controls.Select;

namespace UI.Models
{
    public class WebSiteDetailPageModel : UINotifyPropertyChanged
    {
        public WebSiteModel WebSite_;
        /// <summary>
        /// 站点
        /// </summary>
        public WebSiteModel WebSite { get { return WebSite_; } set { WebSite_ = value; OnPropertyChanged(); } }

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
        public ObservableCollection<string> TabbarData { get { return TabbarData_; } set { TabbarData_ = value; OnPropertyChanged(); } }

        private int TabbarSelectedIndex_;
        public int TabbarSelectedIndex { get { return TabbarSelectedIndex_; } set { TabbarSelectedIndex_ = value; OnPropertyChanged(); } }
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

        private List<WebBrowseLogModel> WebPageData_;
        public List<WebBrowseLogModel> WebPageData { get { return WebPageData_; } set { WebPageData_ = value; OnPropertyChanged(); } }
        private WebBrowseLogModel WebPageSelectedItem_;
        public WebBrowseLogModel WebPageSelectedItem { get { return WebPageSelectedItem_; } set { WebPageSelectedItem_ = value; OnPropertyChanged(); } }

        private List<SelectItemModel> Categories_;
        /// <summary>
        /// 分类数据
        /// </summary>
        public List<SelectItemModel> Categories { get { return Categories_; } set { Categories_ = value; OnPropertyChanged(); } }

        private SelectItemModel Category_;
        /// <summary>
        /// 当前分类
        /// </summary>
        public SelectItemModel Category { get { return Category_; } set { Category_ = value; OnPropertyChanged(); } }

        private ContextMenu WebSiteContextMenu_;
        /// <summary>
        /// 网站右键菜单
        /// </summary>
        public ContextMenu WebSiteContextMenu { get { return WebSiteContextMenu_; } set { WebSiteContextMenu_ = value; OnPropertyChanged(); } }

        public bool IsIgnore_;
        /// <summary>
        /// 是否已被忽略
        /// </summary>
        public bool IsIgnore { get { return IsIgnore_; } set { IsIgnore_ = value; OnPropertyChanged(); } }
    }
}
