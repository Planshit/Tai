using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UI.Controls.Input
{
    public class InputBox : TextBox
    {
        public string Error { get { return (string)GetValue(ErrorProperty); } set { SetValue(ErrorProperty, value); } }
        public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register("Error", typeof(string), typeof(InputBox));

        public bool IsShowError { get { return (bool)GetValue(IsShowErrorProperty); } set { SetValue(IsShowErrorProperty, value); } }
        public static readonly DependencyProperty IsShowErrorProperty = DependencyProperty.Register("IsShowError", typeof(bool), typeof(InputBox));


        public string Placeholder { get { return (string)GetValue(PlaceholderProperty); } set { SetValue(PlaceholderProperty, value); } }
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(InputBox));
        private Popup ErrorPopup;
        public InputBox()
        {
            DefaultStyleKey = typeof(InputBox);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ErrorPopup = GetTemplateChild("ErrorPopup") as Popup;
            ErrorPopup.Closed += ErrorPopup_Closed;
        }

        private void ErrorPopup_Closed(object sender, EventArgs e)
        {
            IsShowError = false;
        }

        public void ShowError()
        {
            IsShowError = true;
        }

        public void HideError()
        {
            IsShowError = false;
        }
    }
}
