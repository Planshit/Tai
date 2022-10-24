using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        }
        public void Init()
        {
            LoadTheme(themeOptions[appConfig.GetConfig().General.Theme]);
        }
        public void LoadTheme(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                //  设置默认主题
                themeName = themeOptions[0];
            }

            if (themeName == this.themeName)
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

            UpdateWindowStyle();

            this.themeName = themeName;

            Debug.WriteLine("已加载主题：" + themeName);
        }



        public void SetMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
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
