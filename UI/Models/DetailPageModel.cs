using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Controls.Charts.Model;
using UI.Controls.Select;

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
    }
}
