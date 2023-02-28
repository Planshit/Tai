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

            Loaded += View_Loaded;
            Unloaded += View_Unloaded;
        }

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= View_Loaded;
            Unloaded -= View_Unloaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            Handle();
        }

        private void Handle()
        {
            try
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
                        string conditionVal = Condition.Substring(Condition.IndexOf("=") + 1);
                        string value = (Value == null ? string.Empty : Value.ToString());
                        isShow = Condition.Contains("!") ? conditionVal != value : conditionVal == value;
                    }
                    else if (Condition.IndexOf("not null") != -1)
                    {

                        isShow = Value != null;
                    }
                    else if (Condition.IndexOf("null") != -1)
                    {

                        isShow = Value == null;
                    }
                    else if (Condition.IndexOf("not empty") != -1)
                    {
                        if (Value == null)
                        {
                            isShow = false;
                        }
                        else
                        {
                            var data = Value as IEnumerable<object>;
                            if (data != null)
                            {
                                isShow = data.Count() > 0;
                            }
                            else
                            {
                                isShow = !string.IsNullOrEmpty(Value.ToString());
                            }
                        }
                    }
                    else if (Condition.IndexOf("empty") != -1)
                    {
                        if (Value == null)
                        {
                            isShow = true;
                        }
                        else
                        {
                            var data = Value as IEnumerable<object>;
                            if (data != null)
                            {
                                isShow = data.Count() == 0;
                            }
                            else
                            {
                                isShow = string.IsNullOrEmpty(Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        isShow = Condition == (Value != null ? Value.ToString() : "");
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
            catch (Exception ex)
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
