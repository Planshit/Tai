using Core.Models;
using Core.Models.Db;
using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UI.Controls.Charts.Model;
using UI.ViewModels;

namespace UI.Servicers
{
    public class WebSiteContextMenuServicer : IWebSiteContextMenuServicer
    {

        private readonly MainViewModel _main;
        private readonly IAppConfig _appConfig;
        private readonly IThemeServicer _theme;
        private readonly IWebData _webData;
        private readonly IUIServicer _uIServicer;

        private ContextMenu _menu;
        private MenuItem _setCategory;
        private MenuItem _block;
        private MenuItem _site;


        public WebSiteContextMenuServicer(
            MainViewModel main_,
            IAppConfig appConfig_,
            IThemeServicer theme_,
            IWebData webData_,
             IUIServicer uIServicer_)
        {
            _main = main_;
            _appConfig = appConfig_;
            _theme = theme_;
            _webData = webData_;
            _uIServicer = uIServicer_;
        }

        public void Init()
        {
            CreateMenu();
            _theme.OnThemeChanged += Theme_OnThemeChanged;
        }

        private void CreateMenu()
        {
            if (_menu != null)
            {
                _menu.ContextMenuOpening -= _menu_ContextMenuOpening;
            }
            _menu = new ContextMenu();
            _menu.Items.Clear();

            MenuItem open = new MenuItem();
            open.Header = "在浏览器打开网站";
            open.Click += Open_Click; ;


            _setCategory = new MenuItem();
            _setCategory.Header = "设置分类";

            MenuItem editAlias = new MenuItem();
            editAlias.Header = "编辑别名";
            editAlias.Click += EditAlias_ClickAsync;

            _block = new MenuItem();
            _block.Header = "忽略此网站";
            _block.Click += Block_Click;

            _site = new MenuItem();
            _site.IsEnabled = false;

            _menu.Items.Add(_site);
            _menu.Items.Add(open);
            _menu.Items.Add(new Separator());
            _menu.Items.Add(_setCategory);
            _menu.Items.Add(editAlias);
            _menu.Items.Add(new Separator());
            _menu.Items.Add(_block);
            _menu.ContextMenuOpening += _menu_ContextMenuOpening;
        }

        private async void EditAlias_ClickAsync(object sender, RoutedEventArgs e)
        {
            var data = _menu.Tag as ChartsDataModel;
            var site = data.Data as WebSiteModel;

            try
            {
                string input = await _uIServicer.ShowInputModalAsync("修改别名", "请输入别名", site.Alias, (val) =>
                {
                    if (string.IsNullOrEmpty(val))
                    {
                        _main.Error("请输入别名");
                        return false;
                    }
                    else if (val.Length > 10)
                    {
                        _main.Error("别名最大长度为10位字符");
                        return false;
                    }
                    return true;
                });

                //  开始更新别名

                data.Name = input;
                site.Alias = input;

                _webData.Update(site);

                _main.Success("别名已更新");
                Debug.WriteLine("输入内容：" + input);
            }
            catch
            {
                //  输入取消，无需处理异常
            }
        }

        private void _menu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_menu.Tag == null)
            {
                return;
            }
            var data = _menu.Tag as ChartsDataModel;
            var site = data.Data as WebSiteModel;
            _site.Header = site.Title;

            var config = _appConfig.GetConfig();
            if (config.Behavior.IgnoreURLList.Contains(site.Domain))
            {
                _block.Header = "取消忽略此站点";
            }
            else
            {
                _block.Header = "忽略此站点";
            }

            UpdateCategoryMenu();
        }

        private void Open_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = _menu.Tag as ChartsDataModel;
            var site = data.Data as WebSiteModel;
            if (!string.IsNullOrEmpty(site.Domain))
            {
                Process.Start($"http://{site.Domain}");
                _main.Info("操作已执行");
            }
        }

        private void Theme_OnThemeChanged(object sender, EventArgs e)
        {
            _menu.UpdateDefaultStyle();
        }

        private void Block_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = _menu.Tag as ChartsDataModel;
            var site = data.Data as WebSiteModel;
            if (site == null) { return; }

            var newBadgeList = new List<ChartBadgeModel>();
            if (data.BadgeList != null)
            {
                var categoryBadge = data.BadgeList.Where(m => m.Type != ChartBadgeType.Ignore).ToList();
                newBadgeList.AddRange(categoryBadge);
            }

            var config = _appConfig.GetConfig();
            if (config.Behavior.IgnoreURLList.Contains(site.Domain))
            {
                config.Behavior.IgnoreURLList.Remove(site.Domain);
                _main.Toast($"已取消忽略此域名 {site.Domain}", Controls.Window.ToastType.Success);
            }
            else
            {
                config.Behavior.IgnoreURLList.Add(site.Domain);
                _main.Toast($"已忽略此域名 {site.Domain}", Controls.Window.ToastType.Success);

                newBadgeList.Add(ChartBadgeModel.IgnoreBadge);
            }
            data.BadgeList = newBadgeList;
        }



        private void UpdateCategoryMenu()
        {
            _setCategory.Items.Clear();

            var data = _menu.Tag as ChartsDataModel;
            var site = data.Data as WebSiteModel;
            var categories = _webData.GetWebSiteCategories();
            foreach (var category in categories)
            {
                var categoryMenu = new MenuItem();
                categoryMenu.Header = category.Name;
                categoryMenu.IsChecked = site.CategoryID == category.ID;
                categoryMenu.Click += (s, e) =>
                {
                    UpdateSiteCategory(data, category.ID);
                };
                _setCategory.Items.Add(categoryMenu);
            }
        }



        private void UpdateSiteCategory(ChartsDataModel data, int categoryId_)
        {
            Task.Run(() =>
            {
                var category = _webData.GetWebSiteCategory(categoryId_);
                if (category != null)
                {
                    WebSiteModel site_ = data.Data as WebSiteModel;
                    _webData.UpdateWebSitesCategory(new int[] { site_.ID }, categoryId_);
                    site_.CategoryID = categoryId_;
                    site_.Category = category;

                    var newBadgeList = new List<ChartBadgeModel>();
                    if (data.BadgeList != null)
                    {
                        var otherBadge = data.BadgeList.Where(m => m.Type != ChartBadgeType.Category).ToList();
                        newBadgeList.AddRange(otherBadge);
                    }
                    newBadgeList.Add(new ChartBadgeModel()
                    {
                        Name = category.Name,
                        Color = category.Color,
                        Type = ChartBadgeType.Category
                    });
                    data.BadgeList = newBadgeList;
                }
            });
        }





        public ContextMenu GetContextMenu()
        {
            return _menu;
        }


    }
}
