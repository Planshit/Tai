using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.Controls.Base;

namespace UI.Controls.Button
{
    public class Button : System.Windows.Controls.Button
    {
        public IconTypes Icon
        {
            get { return (IconTypes)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon",
                typeof(IconTypes),
                typeof(Button), new PropertyMetadata(IconTypes.None));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text",
                typeof(string),
                typeof(Button));
        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(bool),
                typeof(Button), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = (Button)d;

            button.SetContent();

        }

        public Button()
        {
            DefaultStyleKey = typeof(Button);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetContent();
        }

        private void SetContent()
        {
            if (string.IsNullOrEmpty(Text) || Text.IndexOf('?') == -1 || Text.IndexOf(':') == -1)
            {
                return;
            }

            string yes = Text.Substring(Text.IndexOf('?') + 1, Text.IndexOf(':') - 1);
            string no = Text.Substring(Text.IndexOf(':') + 1);
            Content = Value ? yes : no;
        }
    }
}
