using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Charts.Model;
using UI.Controls.DatePickerBar;

namespace UI.Models
{
    public class DataPageModel : UINotifyPropertyChanged
    {
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

        private List<ChartsDataModel> Data_;
        /// <summary>
        ///  data
        /// </summary>
        public List<ChartsDataModel> Data
        {
            get { return Data_; }
            set { Data_ = value; OnPropertyChanged(); }
        }

        private DateTime DayDate_;
        /// <summary>
        ///  date
        /// </summary>
        public DateTime DayDate
        {
            get { return DayDate_; }
            set { DayDate_ = value; OnPropertyChanged(); }
        }

        private DateTime MonthDate_;
        /// <summary>
        ///  date
        /// </summary>
        public DateTime MonthDate
        {
            get { return MonthDate_; }
            set { MonthDate_ = value; OnPropertyChanged(); }
        }
    }
}
