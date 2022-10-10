using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        private ContextMenu menu;
        private MenuItem setCategory;
        MenuItem block = new MenuItem();


        public AppContextMenuServicer(MainViewModel main, ICategorys categorys, IAppData appData, IAppConfig appConfig)
        {
            this.main = main;
            this.categorys = categorys;
            this.appData = appData;
            this.appConfig = appConfig;

            menu = new ContextMenu();
        }

        public void Init()
        {
            menu.Items.Clear();

            MenuItem run = new MenuItem();
            run.Header = "启动应用";
            run.Click += Run_Click;

            MenuItem openDir = new MenuItem();
            openDir.Header = "打开应用所在目录";
            openDir.Click += OpenDir_Click;

            setCategory = new MenuItem();
            setCategory.Header = "设置分类";

            block.Header = "忽略此应用";
            block.Click += Block_Click;

            menu.Items.Add(run);
            menu.Items.Add(new Separator());
            menu.Items.Add(setCategory);
            menu.Items.Add(openDir);
            menu.Items.Add(new Separator());
            menu.Items.Add(block);

            menu.ContextMenuOpening += SetCategory_ContextMenuOpening;
        }

        private void Block_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log.AppModel;

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
            }
        }

        private void SetCategory_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log.AppModel;

            var config = appConfig.GetConfig();
            if (config.Behavior.IgnoreProcessList.Contains(app.Name))
            {
                block.Header = "取消忽略此应用";
            }
            else
            {
                block.Header = "忽略此应用";
            }

            UpdateCategory();
        }

        private void UpdateCategory()
        {
            setCategory.Items.Clear();

            var data = menu.Tag as ChartsDataModel;
            var log = data.Data as DailyLogModel;
            var app = log.AppModel;

            var categories = categorys.GetCategories();
            foreach (var category in categories)
            {
                var categoryMenu = new MenuItem();
                categoryMenu.Header = category.Name;
                categoryMenu.IsChecked = app.CategoryID == category.ID;
                categoryMenu.Click += (s, e) =>
                {
                    SetAppCategory(app.ID, category);
                };
                setCategory.Items.Add(categoryMenu);
            }

        }

        private void SetAppCategory(int appId, CategoryModel category)
        {
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

            var app = log.AppModel;

            if (File.Exists(app.File))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + app.File);
            }
            else
            {
                main.Toast("应用文件似乎不存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.Blocked);
            }
        }

        private void Run_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var data = menu.Tag as ChartsDataModel;

            var log = data.Data as DailyLogModel;

            var app = log.AppModel;

            if (File.Exists(app.File))
            {
                System.Diagnostics.Process.Start(app.File);
                main.Toast("操作已执行", Controls.Window.ToastType.Info);

            }
            else
            {
                main.Toast("应用文件似乎不存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.Blocked);
            }
        }

        public ContextMenu GetContextMenu()
        {
            return menu;
        }


    }
}
