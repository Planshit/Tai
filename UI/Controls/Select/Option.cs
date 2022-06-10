using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.Select
{
    public class Option : Control
    {
        public bool IsShowIcon
        {
            get { return (bool)GetValue(IsShowIconProperty); }
            set { SetValue(IsShowIconProperty, value); }
        }
        public static readonly DependencyProperty IsShowIconProperty =
            DependencyProperty.Register("IsShowIcon",
                typeof(bool),
                typeof(Option), new PropertyMetadata(true));
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked",
                typeof(bool),
                typeof(Option), new PropertyMetadata(false));

        public SelectItemModel Value
        {
            get { return (SelectItemModel)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register("Value",
               typeof(SelectItemModel),
               typeof(Option));
        public Option()
        {
            DefaultStyleKey = typeof(Option);

            MouseLeftButtonUp += Option_MouseLeftButtonUp;
        }

        private void Option_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
        }
    }
}
