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
    }
}
