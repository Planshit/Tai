using Core.Models;
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
using System.Windows.Media;
using UI.Controls.Charts.Model;
using UI.ViewModels;

namespace UI.Servicers
{
    public class AppContextMenuServicer : IAppContextMenuServicer
    {

        private readonly MainViewModel main;
        private readonly ICategorys categorys;
        private readonly IAppData appData;
        private readonly IAppConfig appConfig;
        private readonly IThemeServicer theme;
        private readonly IUIServicer _uIServicer;
        private ContextMenu menu;
        private MenuItem setCategory;
        private MenuItem setLink;
        MenuItem block = new MenuItem();
        MenuItem _whiteList = new MenuItem();


        public AppContextMenuServicer(
            MainViewModel main,
            ICategorys categorys,
            IAppData appData,
            IAppConfig appConfig,
            IThemeServicer theme,
            IUIServicer uIServicer_)
        {
            this.main = main;
            this.categorys = categorys;
            this.appData = appData;
            this.appConfig = appConfig;
            this.theme = theme;
            this._uIServicer = uIServicer_;

        }

        public void Init()
        {
            CreateMenu();
            theme.OnThemeChanged += Theme_OnThemeChanged;
        }

        private void CreateMenu()
        {
            if (menu != null)
            {
                menu.ContextMenuOpening -= SetCategory_ContextMenuOpening;
            }
            menu = new ContextMenu();
            menu.Items.Clear();

            MenuItem run = new MenuItem();
            run.Header = "启动应用";
            run.Click += Run_Click;

            MenuItem openDir = new MenuItem();
            openDir.Header = "打开应用所在目录";
            openDir.Click += OpenDir_Click;

            setCategory = new MenuItem();
            setCategory.Header = "设置分类";

            MenuItem editAlias = new MenuItem();
            editAlias.Header = "编辑别名";
            editAlias.Click += EditAlias_ClickAsync;

            setLink = new MenuItem();
            setLink.Header = "添加关联";

            block.Header = "忽略此应用";
            block.Click += Block_Click;

            _whiteList.Header = "添加到白名单";
            _whiteList.Click += _whiteList_Click;

            menu.Items.Add(run);
            menu.Items.Add(new Separator());
            menu.Items.Add(setCategory);
            menu.Items.Add(setLink);
            menu.Items.Add(editAlias);
            menu.Items.Add(new Separator());

            menu.Items.Add(openDir);
            menu.Items.Add(block);
            menu.Items.Add(_whiteList);

            menu.ContextMenuOpening += SetCategory_ContextMenuOpening;
        }

        private async void EditAlias_ClickAsync(object sender, RoutedEventArgs e)
        {
            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }

            try
            {
                string input = await _uIServicer.ShowInputModalAsync("修改别名", "请输入别名", app.Alias, (val) =>
                 {
                     if (string.IsNullOrEmpty(val))
                     {
                         main.Error("请输入别名");
                         return false;
                     }
                     else if (val.Length > 10)
                     {
                         main.Error("别名最大长度为10位字符");
                         return false;
                     }
                     return true;
                 });

                //  开始更新别名
                var editApp = appData.GetApp(app.ID);
                editApp.Alias = input;
                appData.UpdateApp(editApp);
                data.Name = input;

                main.Success("别名已更新");
                Debug.WriteLine("输入内容：" + input);
            }
            catch
            {
                //  输入取消，无需处理异常
            }
        }

        private void _whiteList_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }
            var config = appConfig.GetConfig();
            if (config.Behavior.ProcessWhiteList.Contains(app.Name))
            {
                config.Behavior.ProcessWhiteList.Remove(app.Name);
                main.Toast($"已从白名单移除此应用 {app.Description}", Controls.Window.ToastType.Success);
            }
            else
            {
                config.Behavior.ProcessWhiteList.Add(app.Name);
                main.Toast($"已添加至白名单 {app.Description}", Controls.Window.ToastType.Success);
            }
        }

        private void Theme_OnThemeChanged(object sender, EventArgs e)
        {
            menu.UpdateDefaultStyle();
        }

        private void Block_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }
            var newBadgeList = new List<ChartBadgeModel>();
            if (data.BadgeList != null)
            {
                var categoryBadge = data.BadgeList.Where(m => m.Type != ChartBadgeType.Ignore).ToList();
                newBadgeList.AddRange(categoryBadge);
            }

            var config = appConfig.GetConfig();
            if (config.Behavior.IgnoreProcessList.Contains(app.Name))
            {
                config.Behavior.IgnoreProcessList.Remove(app.Name);
                main.Toast($"已取消忽略此应用 {app.Description}", Controls.Window.ToastType.Success);
            }
            else
            {
                config.Behavior.IgnoreProcessList.Add(app.Name);
                main.Toast($"已忽略此应用 {app.Description}", Controls.Window.ToastType.Success);

                newBadgeList.Add(ChartBadgeModel.IgnoreBadge);
            }
            data.BadgeList = newBadgeList;
        }

        private void SetCategory_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (menu.Tag == null)
            {
                return;
            }
            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }


            var config = appConfig.GetConfig();
            if (config.Behavior.IgnoreProcessList.Contains(app.Name))
            {
                block.Header = "取消忽略此应用";
            }
            else
            {
                block.Header = "忽略此应用";
            }

            if (config.Behavior.ProcessWhiteList.Contains(app.Name))
            {
                _whiteList.Header = "从白名单移除";
            }
            else
            {
                _whiteList.Header = "添加到白名单";
            }

            UpdateCategory();

            setLink.IsEnabled = config.Links.Count > 0;
            UpdateLinks();
        }

        private void UpdateCategory()
        {
            setCategory.Items.Clear();

            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }

            var categories = categorys.GetCategories();
            foreach (var category in categories)
            {
                var categoryMenu = new MenuItem();
                categoryMenu.Header = category.Name;
                categoryMenu.IsChecked = app.CategoryID == category.ID;
                categoryMenu.Click += (s, e) =>
                {
                    SetAppCategory(data, app.ID, category);
                };
                setCategory.Items.Add(categoryMenu);
            }

            setCategory.IsEnabled = setCategory.Items.Count > 0;

        }

        private void UpdateLinks()
        {
            setLink.Items.Clear();

            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }
            var config = appConfig.GetConfig();

            var links = config.Links;
            foreach (var link in links)
            {
                var categoryMenu = new MenuItem();
                categoryMenu.Header = link.Name;
                categoryMenu.Click += (s, e) =>
                {
                    SetLink(app, link.Name);
                };
                setLink.Items.Add(categoryMenu);
            }

        }
        private void SetLink(AppModel app, string linkName)
        {

            var config = appConfig.GetConfig();
            var links = config.Links;
            var link = links.Where(m => m.ProcessList.Contains(app.Name)).FirstOrDefault();
            if (link != null)
            {
                link.ProcessList.Remove(app.Name);
            }

            link = links.Where(m => m.Name == linkName).FirstOrDefault();
            if (link != null)
            {
                link.ProcessList.Add(app.Name);
                appConfig.Save();

                main.Toast("关联成功", Controls.Window.ToastType.Success);
            }
            else
            {
                main.Toast("关联配置不存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.IncidentTriangle);
            }
        }
        private void SetAppCategory(ChartsDataModel data, int appId, CategoryModel category)
        {
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

            var app = appData.GetApp(appId);
            app.CategoryID = category.ID;
            app.Category = category;
            appData.UpdateApp(app);

            main.Toast("操作已执行", Controls.Window.ToastType.Success);
        }



        private void OpenDir_Click(object sender, System.Windows.RoutedEventArgs e)
        {


            var data = menu.Tag as ChartsDataModel;

            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }

            if (File.Exists(app.File))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + app.File);
            }
            else
            {
                main.Toast("应用文件似乎不存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.IncidentTriangle);
            }
        }

        private void Run_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = menu.Tag as ChartsDataModel;

            var log = data.Data as DailyLogModel;
            var app = log != null ? log.AppModel : null;

            if (log == null)
            {
                app = (data.Data as HoursLogModel).AppModel;
            }

            if (File.Exists(app.File))
            {
                System.Diagnostics.Process.Start(app.File);
                main.Toast("操作已执行", Controls.Window.ToastType.Info);

            }
            else
            {
                main.Toast("应用文件似乎不存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.IncidentTriangle);
            }
        }

        public ContextMenu GetContextMenu()
        {
            return menu;
        }


    }
}
