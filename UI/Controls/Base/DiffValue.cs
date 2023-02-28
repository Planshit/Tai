using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.Base
{
    public class DiffValue : StackPanel
    {
        public enum DiffType
        {
            /// <summary>
            /// 百分比
            /// </summary>
            Percent,
            /// <summary>
            /// 数字
            /// </summary>
            Number
        }
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(double),
                typeof(DiffValue), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
        public double LastValue
        {
            get { return (double)GetValue(LastValueProperty); }
            set { SetValue(LastValueProperty, value); }
        }
        public static readonly DependencyProperty LastValueProperty =
            DependencyProperty.Register("LastValue",
                typeof(double),
                typeof(DiffValue), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
        public DiffType Type
        {
            get { return (DiffType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type",
                typeof(DiffType),
                typeof(DiffValue), new PropertyMetadata(DiffType.Percent, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DiffValue;
            control.Render();
        }

        public DiffValue()
        {
            DefaultStyleKey = typeof(DiffValue);
            Orientation = Orientation.Horizontal;
            Render();
        }



        private void Render()
        {
            Children.Clear();

            double diffValue = Value - LastValue;
            double result = Type == DiffType.Percent ? diffValue / LastValue * 100 : diffValue;
            if (Value > 0 && LastValue <= 0)
            {
                result = Type == DiffType.Percent ? 100 : Value;
            }
            else if (Value == LastValue)
            {
                result = 0;
            }
            var text = new TextBlock();
            text.Text = Type == DiffType.Percent ? Math.Abs(result).ToString("f2") + "%" : Math.Abs(result).ToString();
            text.VerticalAlignment = VerticalAlignment.Center;

            var icon = new Icon();
            icon.FontSize = 10;
            icon.VerticalAlignment = VerticalAlignment.Center;

            if (result > 0)
            {
                icon.IconType = IconTypes.ArrowUp8;
                Children.Add(icon);
                Children.Add(text);
            }
            else if (result < 0)
            {
                icon.IconType = IconTypes.ArrowDown8;
                Children.Add(icon);
                Children.Add(text);
            }
            else
            {
                icon.IconType = IconTypes.SubtractBold;
                Children.Add(icon);
            }


        }
    }
}
