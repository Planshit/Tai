using System;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using UI.Servicers;

namespace UI.Controls.Converters
{
    public class LocalizationExtension : MarkupExtension
    {
        private string key;
        
        public LocalizationExtension(string key)
        {
            this.key = key;
        }
        
        public string Key
        {
            get { return key; }
            set { key = value; }
        }
        
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;
            
            // 获取应用程序的本地化服务
            var app = Application.Current as App;
            if (app?.ServiceProvider != null)
            {
                var localizationService = app.ServiceProvider.GetService(typeof(ILocalizationServicer)) as ILocalizationServicer;
                if (localizationService != null)
                {
                    return localizationService.GetString(key);
                }
            }
            
            // 如果无法获取服务，返回键名
            return key;
        }
    }
} 