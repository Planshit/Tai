using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Navigation.Models;

namespace UI.Models
{
    public class MainWindowModel : UINotifyPropertyChanged
    {
        private IServiceProvider ServiceProvider_;
        public IServiceProvider ServiceProvider
        {
            get { return ServiceProvider_; }
            set { ServiceProvider_ = value; OnPropertyChanged(); }
        }

        private string Uri_;
        public string Uri
        {
            get { return Uri_; }
            set { Uri_ = value; OnPropertyChanged(); }
        }

        private object Data_;
        /// <summary>
        /// 页面传递数据
        /// </summary>
        public object Data
        {
            get { return Data_; }
            set { Data_ = value; OnPropertyChanged(); }
        }

        public ObservableCollection<NavigationItemModel> Items { get; set; }

        private double NavigationWidth_ = 220;
        public double NavigationWidth
        {
            get { return NavigationWidth_; }
            set { NavigationWidth_ = value; OnPropertyChanged(); }
        }

        private bool IsShowNavigation_ = true;
        public bool IsShowNavigation
        {
            get { return IsShowNavigation_; }
            set { IsShowNavigation_ = value; OnPropertyChanged(); }
        }

        private string Title_;
        public string Title
        {
            get { return Title_; }
            set { Title_ = value; OnPropertyChanged(); }
        }
        private bool IsShowTitleBar_ = false;
        public bool IsShowTitleBar
        {
            get { return IsShowTitleBar_; }
            set { IsShowTitleBar_ = value; OnPropertyChanged(); }
        }


        private NavigationItemModel NavSelectedItem_;
        public NavigationItemModel NavSelectedItem
        {
            get { return NavSelectedItem_; }
            set { NavSelectedItem_ = value; OnPropertyChanged(); }
        }
    }
}
