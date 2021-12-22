using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Controls.Charts.Model;

namespace UI.Models
{
    public class DetailPageModel : UINotifyPropertyChanged
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
        private ChartsDataModel Process_;
        public ChartsDataModel Process { get { return Process_; } set { Process_ = value; OnPropertyChanged(); } }
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

        private string ProcessName_;
        public string ProcessName { get { return ProcessName_; } set { ProcessName_ = value; OnPropertyChanged(); } }

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
    }
}
