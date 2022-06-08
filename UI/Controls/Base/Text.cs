using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.Base
{
    public class Text : Control
    {
        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content",
                typeof(string),
                typeof(Text));
        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(bool),
                typeof(Text), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Text c = (Text)d;

            c.SetContent();

        }

        public string Content_ { get; set; }
        public Text()
        {
            DefaultStyleKey = typeof(Text);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetContent();
        }

        private void SetContent()
        {
            if (string.IsNullOrEmpty(Content) || Content.IndexOf('?') == -1 || Content.IndexOf(':') == -1)
            {
                return;
            }

            string yes = Content.Substring(Content.IndexOf('?') + 1, Content.IndexOf(':') - 1);
            string no = Content.Substring(Content.IndexOf(':') + 1);
            Content_ = Value ? yes : no;
        }
    }
}
