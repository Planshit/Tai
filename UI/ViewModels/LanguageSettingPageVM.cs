using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using UI.Models;
using UI.Servicers;

namespace UI.ViewModels
{
    public class LanguageSettingPageVM : LanguageSettingPageModel
    {
        private readonly ILocalizationServicer localizationServicer;

        public LanguageSettingPageVM(ILocalizationServicer localizationServicer)
        {
            this.localizationServicer = localizationServicer;
            InitializeLanguages();
        }

        private void InitializeLanguages()
        {
            var cultures = localizationServicer.GetSupportedCultures();
            var languageNames = cultures.Select(c => GetLanguageDisplayName(c)).ToArray();
            
            AvailableLanguages.Clear();
            foreach (var language in languageNames)
            {
                AvailableLanguages.Add(language);
            }

            // 设置当前选中的语言
            var currentCulture = localizationServicer.CurrentCulture;
            SelectedLanguage = GetLanguageDisplayName(currentCulture);
        }

        private string GetLanguageDisplayName(CultureInfo culture)
        {
            switch (culture.Name)
            {
                case "zh-CN":
                    return "中文 (简体)";
                case "en-US":
                    return "English (US)";
                default:
                    return culture.NativeName;
            }
        }

        public void ChangeLanguage(string languageName)
        {
            string cultureName = GetCultureNameFromDisplayName(languageName);
            if (!string.IsNullOrEmpty(cultureName))
            {
                localizationServicer.SetCulture(cultureName);
                SelectedLanguage = languageName;
            }
        }

        private string GetCultureNameFromDisplayName(string displayName)
        {
            switch (displayName)
            {
                case "中文 (简体)":
                    return "zh-CN";
                case "English (US)":
                    return "en-US";
                default:
                    return "zh-CN"; // 默认中文
            }
        }
    }
} 