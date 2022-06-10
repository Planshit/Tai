using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.Base
{
    public class View : ContentControl
    {
        public string Condition
        {
            get { return (string)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }
        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register("Condition",
                typeof(string),
                typeof(View));
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(object),
                typeof(View), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as View;
            control.Handle();
        }

        public View()
        {
            DefaultStyleKey = typeof(View);
        }


        private void Handle()
        {
            if (string.IsNullOrEmpty(Condition) && Value == null)
            {
                return;
            }

            bool isShow = false;
            if (string.IsNullOrEmpty(Condition))
            {
                if ((bool)Value)
                {
                    isShow = true;
                }
            }
            else
            {


                if (Condition.IndexOf("=") != -1)
                {
                    string val = Condition.Substring(Condition.IndexOf("=") + 1);
                    isShow = val == Value.ToString();
                }
                else
                {
                    isShow = Condition == Value.ToString();
                }
            }


            if (isShow)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }

        }
    }
}
