using Core.Models;
using Core.Models.Db;
using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls;
using UI.Models;
using UI.Models.Category;
using UI.Models.CategoryAppList;
using UI.Servicers;
using UI.Views;

namespace UI.ViewModels
{
    public class CategoryWebSiteListPageVM : CategoryWebSiteListPageModel
    {
        private readonly MainViewModel _mainVM;
        private readonly IWebData _webData;
        public Command ShowChooseCommand { get; set; }
        public Command ChoosedCommand { get; set; }
        public Command GotoDetailCommand { get; set; }
        public Command SearchCommand { get; set; }
        public Command ChooseCloseCommand { get; set; }
        public Command DelCommand { get; set; }
        private List<OptionModel> _webSiteOptionsTemp;
        public CategoryWebSiteListPageVM(
            ILocalizationServicer localizationServicer,
            MainViewModel mainVM_,
            IWebData webData_) : base(localizationServicer)
        {
            _mainVM = mainVM_;
            _webData = webData_;
            Category = mainVM_.Data as WebSiteCategoryModel;

            ShowChooseCommand = new Command(new Action<object>(OnShowChoose));
            ChoosedCommand = new Command(new Action<object>(OnChoosed));
            GotoDetailCommand = new Command(new Action<object>(OnGotoDetail));
            SearchCommand = new Command(new Action<object>(OnSearch));
            ChooseCloseCommand = new Command(new Action<object>(OnChooseClose));
            DelCommand = new Command(new Action<object>(OnDel));

            LoadWebsiteList();
        }

        private void LoadWebsiteList()
        {
            if (Category == null)
            {
                _mainVM.Toast("参数错误", Controls.Window.ToastType.Error, Controls.Base.IconTypes.Error);
                return;
            }
            Task.Run(() =>
            {
                var list = _webData.GetWebSites(Category.ID);
                CategoryWebSiteList = new System.Collections.ObjectModel.ObservableCollection<Core.Models.Db.WebSiteModel>(list);

                LoadWebSiteOptionList();
            });
        }

        /// <summary>
        /// 加载可选站点列表
        /// </summary>
        private void LoadWebSiteOptionList()
        {
            var list = _webData.GetUnSetCategoryWebSites();
            list = list.Concat(CategoryWebSiteList).OrderBy(m => m.Title).ToList();

            var optionList = new List<OptionModel>();
            foreach (var site in list)
            {
                optionList.Add(new OptionModel()
                {
                    IsChecked = site.CategoryID == Category.ID,
                    OptionValue = new Controls.Select.SelectItemModel
                    {
                        Name = $"{site.Title} - {site.Domain}",
                        Img = site.IconFile
                    },
                    WebSite = site
                });
            }
            WebSiteOptionList = optionList;
            _webSiteOptionsTemp = new List<OptionModel>(WebSiteOptionList);
        }
        private void OnDel(object obj)
        {
            if (SelectedItem != null)
            {
                _webData.UpdateWebSitesCategory(new int[] { SelectedItem.ID }, 0);
                CategoryWebSiteList.Remove(SelectedItem);
                if (CategoryWebSiteList.Count == 0)
                {
                    CategoryWebSiteList = new System.Collections.ObjectModel.ObservableCollection<WebSiteModel>();
                }

                Task.Run(() =>
                {
                    LoadWebSiteOptionList();
                });
            }
        }

        private void OnChooseClose(object obj)
        {
            ChooseVisibility = System.Windows.Visibility.Collapsed;
            SearchInput = "";
        }

        private void OnSearch(object obj)
        {
            string keyword = obj.ToString();
            if (string.IsNullOrEmpty(keyword))
            {
                WebSiteOptionList = new List<OptionModel>(_webSiteOptionsTemp);
            }
            else
            {
                WebSiteOptionList = _webSiteOptionsTemp.Where(m => m.WebSite.Title.Contains(keyword) || m.WebSite.Domain.Contains(keyword)).ToList();
            }
        }



        private void OnGotoDetail(object obj)
        {
            if (SelectedItem != null)
            {
                _mainVM.Data = SelectedItem;
                _mainVM.Uri = nameof(WebSiteDetailPage);
            }
        }

        private void OnChoosed(object obj)
        {
            ChooseVisibility = System.Windows.Visibility.Collapsed;
            SearchInput = "";
            _webSiteOptionsTemp = new List<OptionModel>(WebSiteOptionList);
            UpdateCategory();
        }

        private void UpdateCategory()
        {
            var removeSiteList = WebSiteOptionList.Where(m => m.IsChecked == false && CategoryWebSiteList.Where(s => s.ID == m.WebSite.ID).Any()).Select(m => m.WebSite.ID).ToList();
            var addSiteList = WebSiteOptionList.Where(m => m.IsChecked == true && !CategoryWebSiteList.Where(s => s.ID == m.WebSite.ID).Any()).Select(m => m.WebSite).ToList();

            foreach (var id in removeSiteList)
            {
                var item = CategoryWebSiteList.Where(m => m.ID == id).FirstOrDefault();
                if (item != null)
                {
                    CategoryWebSiteList.Remove(item);
                }
            }

            if (CategoryWebSiteList.Count == 0)
            {
                CategoryWebSiteList = new System.Collections.ObjectModel.ObservableCollection<WebSiteModel>(addSiteList);
            }
            else
            {
                foreach (var item in addSiteList)
                {
                    CategoryWebSiteList.Add(item);
                }
            }
            //  从分类中移除
            if (removeSiteList.Count > 0)
            {
                _webData.UpdateWebSitesCategory(removeSiteList.ToArray(), 0);
            }

            //  添加到分类
            if (addSiteList.Count > 0)
            {
                _webData.UpdateWebSitesCategory(addSiteList.Select(m => m.ID).ToArray(), Category.ID);
            }
        }

        private void OnShowChoose(object obj)
        {
            ChooseVisibility = System.Windows.Visibility.Visible;
        }



    }
}
