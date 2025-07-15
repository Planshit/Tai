using System.Collections.ObjectModel;
using System.Globalization;
using UI.Models;

namespace UI.Models
{
    public class LanguageSettingPageModel : ModelBase
    {
        private string selectedLanguage;
        private ObservableCollection<string> availableLanguages;

        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                selectedLanguage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AvailableLanguages
        {
            get { return availableLanguages; }
            set
            {
                availableLanguages = value;
                OnPropertyChanged();
            }
        }

        public LanguageSettingPageModel()
        {
            AvailableLanguages = new ObservableCollection<string>();
        }
    }
} 