using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Models.CategoryAppList;
using UI.Servicers;

namespace UI.Models
{
    public class CategoryAppListPageModel : ModelBase
    {
        private UI.Models.Category.CategoryModel Category_;
        public UI.Models.Category.CategoryModel Category { get { return Category_; } set { Category_ = value; OnPropertyChanged(); } }
        private List<AppModel> Data_;

        public List<AppModel> Data { get { return Data_; } set { Data_ = value; OnPropertyChanged(); } }

        private List<ChooseAppModel> AppList_;
        public List<ChooseAppModel> AppList { get { return AppList_; } set { AppList_ = value; OnPropertyChanged(); } }

        //private List<AppModel> ChooseAppList_;
        //public List<AppModel> ChooseAppList { get { return ChooseAppList_; } set { ChooseAppList_ = value; OnPropertyChanged(); } }

        private Visibility ChooseVisibility_ = Visibility.Hidden;
        public Visibility ChooseVisibility { get { return ChooseVisibility_; } set { ChooseVisibility_ = value; OnPropertyChanged(); } }

        private AppModel SelectedItem_;
        public AppModel SelectedItem { get { return SelectedItem_; } set { SelectedItem_ = value; OnPropertyChanged(); } }

        private string SearchInput_;
        public string SearchInput { get { return SearchInput_; } set { SearchInput_ = value; OnPropertyChanged(); } }

        protected CategoryAppListPageModel(ILocalizationServicer localizationServicer) : base(localizationServicer) {}
    }
}
