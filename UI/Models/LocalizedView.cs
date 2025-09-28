using UI.Servicers;

namespace UI.Models
{
    public abstract class LocalizedView : UINotifyPropertyChanged
    {
        private readonly ILocalizationServicer Localization;

        protected LocalizedView(ILocalizationServicer localization)
        {
            Localization = localization;
        }
        
        protected string Translated(string key) => Localization.Translated(key);
        
        protected string Translated(string key, params object[] args) => Localization.Translated(key, args);
        
        protected string Translated(params object[] args) => Localization.Translated(args);
    }
}