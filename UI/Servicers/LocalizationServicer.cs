using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows;
using UI.Properties;

namespace UI.Servicers
{
    public class LocalizationServicer : ILocalizationServicer
    {
        private ResourceManager resourceManager;
        private CultureInfo currentCulture;
        
        public event EventHandler CultureChanged;
        
        public CultureInfo CurrentCulture => currentCulture;
        
        public LocalizationServicer()
        {
            resourceManager = new ResourceManager("UI.Properties.Resources", typeof(Resources).Assembly);
            currentCulture = Thread.CurrentThread.CurrentUICulture;
        }
        
        public string GetString(string key)
        {
            try
            {
                return resourceManager.GetString(key, currentCulture) ?? key;
            }
            catch
            {
                return key;
            }
        }
        
        public string GetString(string key, params object[] args)
        {
            try
            {
                string format = resourceManager.GetString(key, currentCulture) ?? key;
                return string.Format(format, args);
            }
            catch
            {
                return key;
            }
        }
        
        public void SetCulture(string cultureName)
        {
            try
            {
                var newCulture = new CultureInfo(cultureName);
                if (currentCulture.Name != newCulture.Name)
                {
                    currentCulture = newCulture;
                    Thread.CurrentThread.CurrentUICulture = newCulture;
                    Thread.CurrentThread.CurrentCulture = newCulture;
                    
                    // 更新应用程序级别的文化设置
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CultureChanged?.Invoke(this, EventArgs.Empty);
                    });
                }
            }
            catch (Exception ex)
            {
                // 如果设置文化失败，记录错误但不抛出异常
                System.Diagnostics.Debug.WriteLine($"Failed to set culture: {ex.Message}");
            }
        }
        
        public CultureInfo[] GetSupportedCultures()
        {
            var cultures = new List<CultureInfo>();
            
            // 添加默认文化（中文）
            cultures.Add(new CultureInfo("zh-CN"));
            
            // 添加英文
            cultures.Add(new CultureInfo("en-US"));
            
            return cultures.ToArray();
        }
    }
} 