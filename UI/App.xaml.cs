using Core;
using Core.Librarys;
using Core.Librarys.SQLite;
using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;
using UI.Controls.Window;
using UI.Servicers;
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
        private System.Threading.Mutex mutex;

        //  状态栏图标
        private System.Windows.Forms.NotifyIcon statusBarIcon;

        //  保活窗口
        private HideWindow keepaliveWindow;

        //  状态栏图标菜单
        private ContextMenu contextMenu;

        private DefaultWindow mainWindow;

        private IAppData appData;

        public App()
        {
            InitStatusBarIcon();

            WatchStatus();

            DispatcherUnhandledException += App_DispatcherUnhandledException;

#if DEBUG
            DispatcherUnhandledException -= App_DispatcherUnhandledException;

#endif

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);


            //appData.SaveAppChanges();

            Logger.Save(true);

            statusBarIcon.Visible = false;
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //appData.SaveAppChanges();

            //  记录崩溃错误
            Logger.Error("[程序崩溃异常] " + e.Exception.ToString());

            Logger.Save(true);

            e.Handled = true;

            Shutdown();

            //  显示崩溃弹窗
            string taiBugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "TaiBug.exe");
            ProcessHelper.Run(taiBugPath, new string[] { string.Empty });
        }

        #region 获取当前程序是否已运行
        /// <summary>
        /// 获取当前程序是否已运行
        /// </summary>
        private bool IsRuned()
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name, out ret);
            if (!ret)
            {
#if !DEBUG
                return true;

#endif
            }
            return false;
        }
        #endregion

        #region 状态栏图标
        /// <summary>
        /// 初始化状态栏图标
        /// </summary>
        private void InitStatusBarIcon()
        {
            CreateStatusBarIconMenu();

            statusBarIcon = new System.Windows.Forms.NotifyIcon();
            //statusBarIcon.Text = "Tai!";
            Stream iconStream = GetResourceStream(new Uri("pack://application:,,,/Tai;component/Resources/Icons/taibusy.ico")).Stream;
            statusBarIcon.Icon = new Icon(iconStream);
            //statusBarIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);

            statusBarIcon.Visible = true;
            statusBarIcon.MouseClick += NotifyIcon_MouseClick;
            statusBarIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        }
        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && !AppState.IsLoading)
            {
                //右键单击弹出托盘菜单
                contextMenu.IsOpen = true;
            }
        }
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && !AppState.IsLoading)
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
            var exitMenuItem = new MenuItem();
            exitMenuItem.Header = "退出";
            exitMenuItem.Click += ExitMenuItem_Click; ;

            contextMenu.Items.Add(mainWindowMenuItem);
            contextMenu.Items.Add(exitMenuItem);

        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void MainWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }
        #endregion

        private void ConfigureServices(IServiceCollection services)
        {
            //  核心服务
            services.AddSingleton<IObserver, Observer>();
            services.AddSingleton<IMain, Main>();
            services.AddSingleton<IData, Data>();
            services.AddSingleton<ISleepdiscover, Sleepdiscover>();
            services.AddSingleton<IAppConfig, AppConfig>();
            services.AddSingleton<IDateTimeObserver, DateTimeObserver>();
            services.AddSingleton<IAppData, AppData>();
            services.AddSingleton<ICategorys, Categorys>();

            //  UI服务
            services.AddSingleton<IAppContextMenuServicer, AppContextMenuServicer>();
            services.AddSingleton<IThemeServicer, ThemeServicer>();
            services.AddSingleton<IMainServicer, MainServicer>();
            services.AddSingleton<IInputServicer, InputServicer>();

            //  主窗口
            services.AddSingleton<MainViewModel>();
            services.AddTransient<MainWindow>();

            //  首页
            services.AddTransient<IndexPage>();
            services.AddTransient<IndexPageVM>();

            //  数据页
            services.AddTransient<DataPage>();
            services.AddTransient<DataPageVM>();

            //  设置页
            services.AddTransient<SettingPage>();
            services.AddTransient<SettingPageVM>();

            //  详情页
            services.AddTransient<DetailPage>();
            services.AddTransient<DetailPageVM>();

            //  分类
            services.AddTransient<CategoryPage>();
            services.AddTransient<CategoryPageVM>();

            //  分类app
            services.AddTransient<CategoryAppListPage>();
            services.AddTransient<CategoryAppListPageVM>();

            //  图表
            services.AddTransient<ChartPage>();
            services.AddTransient<ChartPageVM>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            //  阻止多开进程
            if (IsRuned())
            {
                Shutdown();
            }

            var main = serviceProvider.GetService<IMainServicer>();
            main.Start();

            var observer = serviceProvider.GetService<IObserver>();
            observer.OnAppActive += (p, d, f) =>
            {
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                {
                    contextMenu.IsOpen = false;
                }));
            };

            var theme = serviceProvider.GetService<IThemeServicer>();
            theme.OnThemeChanged += (a, c) =>
            {
                contextMenu.UpdateDefaultStyle();
            };
            //  创建保活窗口
            keepaliveWindow = new HideWindow();
        }



        private void ShowMainWindow()
        {
            var dataContext = serviceProvider.GetService<MainViewModel>();


            if (mainWindow == null || mainWindow.IsWindowClosed)
            {
                mainWindow = serviceProvider.GetService<MainWindow>();

                mainWindow.DataContext = dataContext;
                dataContext.LoadDefaultPage();
            }

            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Show();
            mainWindow.Activate();
        }

        private async void WatchStatus()
        {
            await Task.Run(() =>
             {
                 while (AppState.IsLoading)
                 {
                     statusBarIcon.Text = $"[{AppState.ProcessValue}%] Tai [{AppState.ActionText}]";
                 }
                 statusBarIcon.Text = "Tai!";
                 Stream iconStream = GetResourceStream(new Uri("pack://application:,,,/Tai;component/Resources/Icons/tai32.ico")).Stream;
                 statusBarIcon.Icon = new Icon(iconStream);
             });
        }
    }
}
