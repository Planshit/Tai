using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using UI.Controls.Base;

namespace UI.Controls.Input
{
    public class InputBox : TextBox
    {
        /// <summary>
        /// 圆角半径
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(InputBox));

        /// <summary>
        /// 图标
        /// </summary>
        public IconTypes Icon
        {
            get { return (IconTypes)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(IconTypes), typeof(InputBox), new PropertyMetadata(IconTypes.None));

        public Thickness BoxPadding { get { return (Thickness)GetValue(BoxPaddingProperty); } set { SetValue(BoxPaddingProperty, value); } }
        public static readonly DependencyProperty BoxPaddingProperty = DependencyProperty.Register("BoxPadding", typeof(Thickness), typeof(InputBox));

        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(InputBox));

        public Thickness TitleMargin { get { return (Thickness)GetValue(TitleMarginProperty); } set { SetValue(TitleMarginProperty, value); } }
        public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.Register("TitleMargin", typeof(Thickness), typeof(InputBox));

        public Thickness IconMargin { get { return (Thickness)GetValue(IconMarginProperty); } set { SetValue(IconMarginProperty, value); } }
        public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(InputBox));

        public SolidColorBrush IconColor { get { return (SolidColorBrush)GetValue(IconColorProperty); } set { SetValue(IconColorProperty, value); } }
        public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register("IconColor", typeof(SolidColorBrush), typeof(InputBox));

        public SolidColorBrush TitleColor { get { return (SolidColorBrush)GetValue(TitleColorProperty); } set { SetValue(TitleColorProperty, value); } }
        public static readonly DependencyProperty TitleColorProperty = DependencyProperty.Register("TitleColor", typeof(SolidColorBrush), typeof(InputBox));

        public double IconSize { get { return (double)GetValue(IconSizeProperty); } set { SetValue(IconSizeProperty, value); } }
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(InputBox));

        public double TitleSize { get { return (double)GetValue(TitleSizeProperty); } set { SetValue(TitleSizeProperty, value); } }
        public static readonly DependencyProperty TitleSizeProperty = DependencyProperty.Register("TitleSize", typeof(double), typeof(InputBox));
        public string Error { get { return (string)GetValue(ErrorProperty); } set { SetValue(ErrorProperty, value); } }
        public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register("Error", typeof(string), typeof(InputBox));
        public bool IsError { get { return (bool)GetValue(IsErrorProperty); } set { SetValue(IsErrorProperty, value); } }
        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register("IsError", typeof(bool), typeof(InputBox));

        public bool IsShowError { get { return (bool)GetValue(IsShowErrorProperty); } set { SetValue(IsShowErrorProperty, value); } }
        public static readonly DependencyProperty IsShowErrorProperty = DependencyProperty.Register("IsShowError", typeof(bool), typeof(InputBox));


        public string Placeholder { get { return (string)GetValue(PlaceholderProperty); } set { SetValue(PlaceholderProperty, value); } }
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(InputBox));
        private Popup ErrorPopup;

        public InputBox()
        {
            DefaultStyleKey = typeof(InputBox);
            Unloaded += InputBox_Unloaded;
        }

        private void InputBox_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= InputBox_Unloaded;
            if (ErrorPopup != null)
            {
                ErrorPopup.Closed -= ErrorPopup_Closed;
            }
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
