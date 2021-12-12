using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Charts.Model;

namespace UI.Models
{
    public class IndexPageModel : UINotifyPropertyChanged
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

      
        private List<ChartsDataModel> WeekData_;
        /// <summary>
        /// week data
        /// </summary>
        public List<ChartsDataModel> WeekData
        {
            get { return WeekData_; }
            set { WeekData_ = value; OnPropertyChanged(); }
        }

        private bool IsLoading_;
        public bool IsLoading { get { return IsLoading_; } set { IsLoading_ = value; OnPropertyChanged(); } }
    }
}
