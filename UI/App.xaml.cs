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
        //  保活窗口
        private HideWindow keepaliveWindow;

        public App()
        {
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
            Logger.Save(true);
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            //  记录崩溃错误
            Logger.Error("[程序崩溃异常] " + e.Exception.ToString());
            Logger.Save(true);

            //  显示崩溃弹窗
            string taiBugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "TaiBug.exe");
            ProcessHelper.Run(taiBugPath, new string[] { string.Empty });

            //  退出程序
            Shutdown();
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

        private void ConfigureServices(IServiceCollection services)
        {
            //  核心服务
            services.AddSingleton<IDatabase, Database>();
            services.AddSingleton<IAppManager, AppManager>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<IAppTimerServicer, AppTimerServicer>();
            services.AddSingleton<IAppObserver, AppObserver>();
            services.AddSingleton<IWebServer, WebServer>();
            services.AddSingleton<IMain, Main>();
            services.AddSingleton<IData, Data>();
            services.AddSingleton<IWebData, WebData>();
            services.AddSingleton<ISleepdiscover, Sleepdiscover>();
            services.AddSingleton<IAppConfig, AppConfig>();
            services.AddSingleton<IDateTimeObserver, DateTimeObserver>();
            services.AddSingleton<IAppData, AppData>();
            services.AddSingleton<ICategorys, Categorys>();
            services.AddSingleton<IWebFilter, WebFilter>();

            //  UI服务
            services.AddSingleton<ILocalizationServicer, LocalizationServicer>();
            services.AddSingleton<IUIServicer, UIServicer>();
            services.AddSingleton<IAppContextMenuServicer, AppContextMenuServicer>();
            services.AddSingleton<IThemeServicer, ThemeServicer>();
            services.AddSingleton<IMainServicer, MainServicer>();
            services.AddSingleton<IInputServicer, InputServicer>();
            services.AddSingleton<IWebSiteContextMenuServicer, WebSiteContextMenuServicer>();
            services.AddSingleton<IStatusBarIconServicer, StatusBarIconServicer>();

            //  主窗口
            services.AddSingleton<MainViewModel>();

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
            //  分类站点
            services.AddTransient<CategoryWebSiteListPage>();
            services.AddTransient<CategoryWebSiteListPageVM>();
            //  图表
            services.AddTransient<ChartPage>();
            services.AddTransient<ChartPageVM>();
            //  网站详情
            services.AddTransient<WebSiteDetailPage>();
            services.AddTransient<WebSiteDetailPageVM>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            //  阻止多开进程
            if (IsRuned())
            {
                Shutdown();
            }

            var main = serviceProvider.GetService<IMainServicer>();

            bool isSelfStart = false;
            if (e.Args.Length != 0)
            {
                if (e.Args[0].Equals("--selfStart"))
                {
                    isSelfStart = true;
                }
            }
            main.Start(isSelfStart);

            //  创建保活窗口
            keepaliveWindow = new HideWindow();
        }
    }
}
