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

        private DateTime Date_;
        /// <summary>
        ///  date
        /// </summary>
        public DateTime Date
        {
            get { return Date_; }
            set { Date_ = value; OnPropertyChanged(); }
        }

        private DatePickerShowType DatePickerType_;
        public DatePickerShowType DatePickerType { get { return DatePickerType_; } set { DatePickerType_ = value; OnPropertyChanged(); } }
    }
}
