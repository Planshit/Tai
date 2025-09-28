using Core.Models.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Servicers;

namespace UI.Models
{
    public class SettingPageModel : LocalizedView
    {
        private object data;
        public object Data { get { return data; } set { data = value; OnPropertyChanged(); } }

        private ObservableCollection<string> TabbarData_;
        /// <summary>
        /// tabbar data
        /// </summary>
        public ObservableCollection<string> TabbarData { get { return TabbarData_; } set { TabbarData_ = value; OnPropertyChanged(); } }

        private int TabbarSelectedIndex_;
        /// <summary>
        /// tabbar selected item index
        /// </summary>
        public int TabbarSelectedIndex { get { return TabbarSelectedIndex_; } set { TabbarSelectedIndex_ = value; OnPropertyChanged(); } }

        private string version;
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Version { get { return version; } set { version = value; OnPropertyChanged(); } }

        private Visibility CheckUpdateBtnVisibility_ = Visibility.Visible;
        public Visibility CheckUpdateBtnVisibility { get { return CheckUpdateBtnVisibility_; } set { CheckUpdateBtnVisibility_ = value; OnPropertyChanged(); } }


        private DateTime DelDataStartMonthDate_;
        public DateTime DelDataStartMonthDate { get { return DelDataStartMonthDate_; } set { DelDataStartMonthDate_ = value; OnPropertyChanged(); } }

        private DateTime DelDataEndMonthDate_;
        public DateTime DelDataEndMonthDate { get { return DelDataEndMonthDate_; } set { DelDataEndMonthDate_ = value; OnPropertyChanged(); } }

        private DateTime ExportDataStartMonthDate_;
        public DateTime ExportDataStartMonthDate { get { return ExportDataStartMonthDate_; } set { ExportDataStartMonthDate_ = value; OnPropertyChanged(); } }

        private DateTime ExportDataEndMonthDate_;
        public DateTime ExportDataEndMonthDate { get { return ExportDataEndMonthDate_; } set { ExportDataEndMonthDate_ = value; OnPropertyChanged(); } }

        public SettingPageModel(ILocalizationServicer localization) : base(localization) {}
    }
}
