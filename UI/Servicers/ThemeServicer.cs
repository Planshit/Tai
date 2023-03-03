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

        public event EventHandler OnThemeChanged;

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
                OnThemeChanged?.Invoke(this, EventArgs.Empty);
            }

            if (oldConfig.General.ThemeColor != newConfig.General.ThemeColor)
            {
                LoadTheme(themeOptions[newConfig.General.Theme], true);
                OnThemeChanged?.Invoke(this, EventArgs.Empty);
            }

            if (oldConfig.General.IsSaveWindowSize != newConfig.General.IsSaveWindowSize)
            {
                HandleWindowSizeChangedEvent();
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

            var themeDict = GetResourceDictionary($"pack://application:,,,/Tai;component/Resources/Themes/{themeName}.xaml");
            var controlDict = GetResourceDictionary($"pack://application:,,,/Tai;component/Themes/Generic.xaml");
            if (themeDict == null || controlDict == null)
            {
                return;
            }

            //Collection<ResourceDictionary> resourceDictionaries = Application.Current.Resources.MergedDictionaries;
            //for (int i = 0; i < resourceDictionaries.Count; i++)
            //{
            //    ResourceDictionary resourceDictionary = resourceDictionaries[i];
            //    //if (resourceDictionary.FindName(""))
            //    //{
            //    if (resourceDictionary.Contains("ThemeName"))
            //    {
            //        foreach(var key in resourceDictionary.Keys)
            //        {
            //            resourceDictionary[key] = themeDict[key];
            //        }
            //        //resourceDictionary["ThemeColor"] = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#ff0000");
            //    }
            //    //}
            //}
            //清除旧主题
            if (this.themeName != null)
            {
                var oldStyles = MergedDictionaries.Where(m => m.Source.OriginalString.Contains(this.themeName) || m.Source.OriginalString.Contains("Generic")).ToList();
                if (oldStyles != null)
                {
                    foreach (var oldStyle in oldStyles)
                    {
                        MergedDictionaries.Remove(oldStyle);
                    }
                }
            }

            MergedDictionaries.Add(themeDict);
            MergedDictionaries.Add(controlDict);

            //if (configDict != null)
            //{
            //    //MergedDictionaries.Add(configDict);
            //}
            //if (controlDict != null)
            //{
            //    MergedDictionaries.Add(controlDict);
            //}
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
                StateData.ThemeColor = ((System.Windows.Media.Color)Application.Current.Resources["ThemeColor"]).ToString();
                return;
            }

            StateData.ThemeColor = config.General.ThemeColor;
            Application.Current.Resources["ThemeColor"] = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(config.General.ThemeColor);
            Application.Current.Resources["ThemeBrush"] = UI.Base.Color.Colors.GetFromString(config.General.ThemeColor);
        }

        public void SetMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            HandleWindowSizeChangedEvent();
        }

        private void HandleWindowSizeChangedEvent()
        {
            if (mainWindow == null || mainWindow.IsWindowClosed) return;

            mainWindow.SizeChanged -= MainWindow_SizeChanged;

            var config = appConfig.GetConfig();
            if (config.General.IsSaveWindowSize)
            {
                //  保存窗口大小信息
                mainWindow.SizeChanged += MainWindow_SizeChanged;
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var config = appConfig.GetConfig();
            config.General.WindowWidth = mainWindow.ActualWidth;
            config.General.WindowHeight = mainWindow.ActualHeight;
            appConfig.Save();
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
