using Core;
using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UI.ViewModels;

namespace UI.Servicers
{
    public class StatusBarIconServicer : IStatusBarIconServicer
    {
        //  状态栏图标
        private System.Windows.Forms.NotifyIcon _statusBarIcon;
        //  状态栏图标菜单
        private ContextMenu _contextMenu;

        /// <summary>
        /// 图标类型
        /// </summary>
        public enum IconType
        {
            /// <summary>
            /// 正常
            /// </summary>
            Normal,
            /// <summary>
            /// 繁忙中
            /// </summary>
            Busy
        }

        private readonly IAppObserver _appObserver;
        private readonly IThemeServicer _themeServicer;
        private readonly MainViewModel _mainVM;
        private readonly IAppConfig _appConfig;
        private readonly IUIServicer _uIServicer;
        private MainWindow _mainWindow;
        public StatusBarIconServicer(
            IAppObserver appObserver_,
            IThemeServicer themeServicer_,
            IAppConfig appConfig_,
            MainViewModel mainVM_,
            IUIServicer uIServicer_
            )
        {
            _appObserver = appObserver_;
            _themeServicer = themeServicer_;
            _appConfig = appConfig_;
            _mainVM = mainVM_;
            _uIServicer = uIServicer_;
        }
        public void Init()
        {
            InitMenu();

            _statusBarIcon = new System.Windows.Forms.NotifyIcon();
            _statusBarIcon.Visible = true;
            _statusBarIcon.MouseClick += _statusBarIcon_MouseClick;
            _statusBarIcon.MouseDoubleClick += _statusBarIcon_MouseDoubleClick;

            SetIcon(IconType.Busy);

            WatchStateAsync();

            _appObserver.OnAppActiveChanged += _appObserver_OnAppActiveChanged; ;
            _themeServicer.OnThemeChanged += _themeServicer_OnThemeChanged;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }


        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _statusBarIcon.Visible = false;
            Logger.Save(true);
        }

        private void _themeServicer_OnThemeChanged(object sender, EventArgs e)
        {
            _contextMenu.UpdateDefaultStyle();
        }

        private void _appObserver_OnAppActiveChanged(object sender, Core.Event.AppActiveChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _contextMenu.IsOpen = false;
            }));
        }

        /// <summary>
        /// 初始化状态栏菜单
        /// </summary>
        private void InitMenu()
        {
            _contextMenu = new ContextMenu();
            App.Current.Deactivated += (e, c) =>
            {
                _contextMenu.IsOpen = false;
            };
            var mainWindowMenuItem = new MenuItem();
            mainWindowMenuItem.Header = "主界面";
            mainWindowMenuItem.Click += MainWindowMenuItem_Click; ;
            var exitMenuItem = new MenuItem();
            exitMenuItem.Header = "退出";
            exitMenuItem.Click += ExitMenuItem_Click;

            _contextMenu.Items.Add(mainWindowMenuItem);
            _contextMenu.Items.Add(exitMenuItem);

        }

        /// <summary>
        /// 等待程序加载
        /// </summary>
        private async void WatchStateAsync()
        {
            await Task.Run(() =>
            {
                while (AppState.IsLoading)
                {
                    _statusBarIcon.Text = $"[{AppState.ProcessValue}%] Tai [{AppState.ActionText}]";
                }
                _statusBarIcon.Text = "Tai!";
                SetIcon();
            });
        }

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="iconType_">图标样式</param>
        private void SetIcon(IconType iconType_ = IconType.Normal)
        {
            try
            {
                string iconName = "tai32";
                switch (iconType_)
                {
                    case IconType.Busy:
                        iconName = "taibusy";
                        break;
                    default:
                        iconName = "tai32";
                        break;
                }
                Stream iconStream = Application.GetResourceStream(new Uri($"pack://application:,,,/Tai;component/Resources/Icons/{iconName}.ico")).Stream;
                _statusBarIcon.Icon = new Icon(iconStream);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Save(true);
            }
        }

        public void ShowMainWindow()
        {
            var config = _appConfig.GetConfig();
            if (config == null)
            {
                return;
            }

            if (_mainWindow == null || _mainWindow.IsWindowClosed)
            {
                _mainWindow = new MainWindow();
                _mainWindow.DataContext = _mainVM;
                _mainWindow.Loaded += _mainWindow_Loaded;
                _mainVM.LoadDefaultPage();
            }
            if (config.General.IsSaveWindowSize)
            {
                _mainWindow.Width = config.General.WindowWidth;
                _mainWindow.Height = config.General.WindowHeight;
            }
            _mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Show();
            _mainWindow.Activate();

            _uIServicer.InitWindow(_mainWindow);
        }

        private void _mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as MainWindow;

            _themeServicer.SetMainWindow(window);
            _themeServicer.UpdateWindowStyle();
        }

        private void ExitApp()
        {
            _statusBarIcon.Visible = false;

            Logger.Save(true);
            Application.Current.Shutdown();
        }
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExitApp();
        }

        private void MainWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowMainWindow();
        }

        private void _statusBarIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && !AppState.IsLoading)
            {
                //双击托盘图标显示主窗口
                ShowMainWindow();
            }
        }

        private void _statusBarIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && !AppState.IsLoading)
            {
                //右键单击弹出托盘菜单
                _contextMenu.IsOpen = true;
            }
        }
    }
}
