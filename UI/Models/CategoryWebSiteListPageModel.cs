using Core.Models;
using Core.Models.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Controls.Select;
using UI.Models.CategoryAppList;
using UI.Servicers;

namespace UI.Models
{
    public class CategoryWebSiteListPageModel : ModelBase
    {
        public class OptionModel
        {
            public bool IsChecked { get; set; }
            public WebSiteModel WebSite { get; set; }
            public SelectItemModel OptionValue { get; set; } = new SelectItemModel();
        }
        private Visibility ChooseVisibility_ = Visibility.Hidden;
        public Visibility ChooseVisibility { get { return ChooseVisibility_; } set { ChooseVisibility_ = value; OnPropertyChanged(); } }

        private WebSiteModel SelectedItem_;
        public WebSiteModel SelectedItem { get { return SelectedItem_; } set { SelectedItem_ = value; OnPropertyChanged(); } }

        private string SearchInput_;
        public string SearchInput { get { return SearchInput_; } set { SearchInput_ = value; OnPropertyChanged(); } }

        private ObservableCollection<WebSiteModel> CategoryWebSiteList_;
        /// <summary>
        /// 分类站点列表
        /// </summary>
        public ObservableCollection<WebSiteModel> CategoryWebSiteList { get { return CategoryWebSiteList_; } set { CategoryWebSiteList_ = value; OnPropertyChanged(); } }

        private WebSiteCategoryModel Category_;
        /// <summary>
        /// 当前分类
        /// </summary>
        public WebSiteCategoryModel Category { get { return Category_; } set { Category_ = value; OnPropertyChanged(); } }

        private List<OptionModel> WebSiteOptionList_;
        /// <summary>
        /// 站点可选列表
        /// </summary>
        public List<OptionModel> WebSiteOptionList { get { return WebSiteOptionList_; } set { WebSiteOptionList_ = value; OnPropertyChanged(); } }

        protected CategoryWebSiteListPageModel(ILocalizationServicer localizationServicer) : base(localizationServicer) {}
    }
}
