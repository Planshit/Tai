using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UI.ViewModels;

namespace UI.Servicers
{
    public class ThemeServicer : IThemeServicer
    {
        /// <summary>
        /// 当前主题名称
        /// </summary>
        private string themeName;
        private MainWindow mainWindow;
        private Collection<ResourceDictionary> MergedDictionaries;
        private readonly string[] themeOptions = { "Light", "Dark" };
        private readonly IAppConfig appConfig;
        public ThemeServicer(IAppConfig appConfig)
        {
            this.appConfig = appConfig;

            appConfig.ConfigChanged += AppConfig_ConfigChanged;

            MergedDictionaries = Application.Current.Resources.MergedDictionaries;
        }

        private void AppConfig_ConfigChanged(Core.Models.Config.ConfigModel oldConfig, Core.Models.Config.ConfigModel newConfig)
        {
            if (oldConfig.General.Theme != newConfig.General.Theme)
            {
                LoadTheme(themeOptions[newConfig.General.Theme]);
            }

            if (oldConfig.General.ThemeColor != newConfig.General.ThemeColor)
            {
                LoadTheme(themeOptions[newConfig.General.Theme], true);
            }
        }
        public void Init()
        {
            LoadTheme(themeOptions[appConfig.GetConfig().General.Theme]);
        }
        public void LoadTheme(string themeName, bool isRefresh = false)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                //  设置默认主题
                themeName = themeOptions[0];
            }

            if (themeName == this.themeName && !isRefresh)
            {
                return;
            }

            var configDict = GetResourceDictionary($"pack://application:,,,/Tai;component/Resources/Themes/{themeName}/Config.xaml");
            var controlDict = GetResourceDictionary($"pack://application:,,,/Tai;component/Resources/Themes/{themeName}/Style.xaml");


            //清除旧主题
            var oldStyles = MergedDictionaries.Where(m => m.Source.OriginalString.Contains(themeName)).ToList();
            if (oldStyles != null)
            {
                foreach (var oldStyle in oldStyles)
                {
                    MergedDictionaries.Remove(oldStyle);
                }
            }
            if (configDict != null)
            {
                MergedDictionaries.Add(configDict);
            }
            if (controlDict != null)
            {
                MergedDictionaries.Add(controlDict);
            }
            this.themeName = themeName;

            UpdateWindowStyle();
            UpdateThemeColor();

            Debug.WriteLine("已加载主题：" + themeName);
        }

        /// <summary>
        /// 刷新主题颜色
        /// </summary>
        private void UpdateThemeColor()
        {
            var config = appConfig.GetConfig();
            if (string.IsNullOrEmpty(config.General.ThemeColor))
            {
                return;
            }
            Application.Current.Resources["ThemeColor"] = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.General.ThemeColor);
            Application.Current.Resources["ThemeBrush"] = UI.Base.Color.Colors.GetFromString(config.General.ThemeColor);
        }

        public void SetMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.SizeChanged += MainWindow_SizeChanged;

            var config = appConfig.GetConfig();
            if (config.General.IsSaveWindowSize)
            {
                mainWindow.Width = config.General.WindowWidth;
                mainWindow.Height = config.General.WindowHeight;
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var config = appConfig.GetConfig();
            if (config.General.IsSaveWindowSize)
            {
                config.General.WindowWidth = mainWindow.ActualWidth;
                config.General.WindowHeight = mainWindow.ActualHeight;
                appConfig.Save();
            }
        }

        public void UpdateWindowStyle()
        {
            if (mainWindow != null)
            {
                var themeStyle = Application.Current.Resources["WindowStyle"] as Style;
                if (themeStyle != null)
                {
                    mainWindow.Style = themeStyle;
                }
            }
        }

        private ResourceDictionary GetResourceDictionary(string uri)
        {
            try
            {
                return new ResourceDictionary { Source = new Uri(uri, UriKind.RelativeOrAbsolute) };
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return null;
            }
        }
    }
}
