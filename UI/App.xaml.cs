using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;
using UI.Controls.Window;
using UI.ViewModels;
using UI.Views;

namespace UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;

        //  状态栏图标
        private System.Windows.Forms.NotifyIcon statusBarIcon;

        //  保活窗口
        private HideWindow keepaliveWindow;

        //  状态栏图标菜单
        private ContextMenu contextMenu;
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();

        }

        #region 状态栏图标
        /// <summary>
        /// 初始化状态栏图标
        /// </summary>
        private void InitStatusBarIcon()
        {
            CreateStatusBarIconMenu();

            statusBarIcon = new System.Windows.Forms.NotifyIcon();

            statusBarIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(
             System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);

            statusBarIcon.Visible = true;
            statusBarIcon.MouseClick += NotifyIcon_MouseClick;
            statusBarIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        }
        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //右键单击弹出托盘菜单
                contextMenu.IsOpen = true;
                //激活主窗口，用于处理关闭托盘菜单
                App.Current.MainWindow.Activate();
            }
        }
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //双击托盘图标显示主窗口
                ShowMainWindow();
            }
        }

        /// <summary>
        /// 创建状态栏图标菜单
        /// </summary>
        private void CreateStatusBarIconMenu()
        {
            contextMenu = new ContextMenu();
            App.Current.Deactivated += (e, c) =>
            {
                contextMenu.IsOpen = false;
            };
            var mainWindowMenuItem = new MenuItem();
            mainWindowMenuItem.Header = "主界面";
            mainWindowMenuItem.Click += MainWindowMenuItem_Click;

            contextMenu.Items.Add(mainWindowMenuItem);
        }

        private void MainWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }
        #endregion

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IObserver, Observer>();
            services.AddSingleton<IMain, Main>();

            services.AddSingleton<MainViewModel>();
            services.AddTransient<MainWindow>();

            services.AddTransient<IndexPage>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var main = serviceProvider.GetService<IMain>();
            main.Run();

            //  创建保活窗口
            keepaliveWindow = new HideWindow();
            keepaliveWindow.Title = "Tai Keepalive";
            keepaliveWindow.ShowInTaskbar = false;
            keepaliveWindow.WindowStyle = WindowStyle.None;
            keepaliveWindow.OverridesDefaultStyle = true;
            keepaliveWindow.Show();

            InitStatusBarIcon();
        }

        private void ShowMainWindow()
        {
            var mainWindow = serviceProvider.GetService<MainWindow>();
            var dataContext = serviceProvider.GetService<MainViewModel>();
            mainWindow.DataContext = dataContext;
            dataContext.SelectGroup(-1);
            ////dataContext.Uri = nameof(IndexPage);
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.Show();
            mainWindow.Activate();
        }
    }
}
