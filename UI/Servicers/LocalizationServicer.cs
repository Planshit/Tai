using System.Globalization;
using System.Linq;
using System.Resources;
using UI.Resources.Localization;

namespace UI.Servicers
{
    public class LocalizationServicer : ILocalizationServicer
    {
        public static LocalizationServicer Instance;
        
        private ResourceManager resourceManager;
        private CultureInfo culture;

        public LocalizationServicer()
        {
            resourceManager = new ResourceManager("UI.Resources.Localization.Localization", typeof(LocalizationServicer).Assembly);
            culture = CultureInfo.CurrentUICulture;
            Instance = this;
        }

        public void SetCulture(CultureInfo culture)
        {
            this.culture = culture ?? CultureInfo.InvariantCulture;
        }

        public string Translated(string key)
        {
            return resourceManager.GetString(key, culture) ?? key;
        }
        
        public string Translated(string key, params object[] args)
        {
            return string.Format(culture, Translated(key), args);
        }
        
        public string Translated(params object[] args)
        {
            return string.Format(culture, Translated(args[0] as string), args.Skip(1).ToArray());
        }
    }
}