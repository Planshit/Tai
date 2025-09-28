using System;
using System.Windows.Markup;
using UI.Servicers;

namespace UI.Markup
{
    public class LocalizationExtension : MarkupExtension
    {
        public string Key { get; set; }
        public object Arg0 { get; set; }
        public object Arg1 { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var args = new [] { Arg0, Arg1 };
            var filteredArgs = Array.FindAll(args, x => x != null);
            return LocalizationServicer.Instance.Translated(Key, filteredArgs);
        }
    }
}