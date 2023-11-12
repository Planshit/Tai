using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.Controls.Toggle
{
    public class Toggle : Control
    {
        public event EventHandler ToggleChanged;
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(Toggle), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsCheckedChanged)));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Toggle;

            VisualStateManager.GoToState(control, control.IsChecked ? "On" : "Off", true);
        }
        public ToggleTextPosition TextPosition
        {
            get { return (ToggleTextPosition)GetValue(TextPositionProperty); }
            set { SetValue(TextPositionProperty, value); }
        }
        public static readonly DependencyProperty TextPositionProperty =
            DependencyProperty.Register("TextPosition", typeof(ToggleTextPosition), typeof(Toggle), new PropertyMetadata(ToggleTextPosition.Right));
        public string OnText
        {
            get { return (string)GetValue(OnTextProperty); }
            set { SetValue(OnTextProperty, value); }
        }
        public static readonly DependencyProperty OnTextProperty =
            DependencyProperty.Register("OnText", typeof(string), typeof(Toggle), new PropertyMetadata("开"));
        public string OffText
        {
            get { return (string)GetValue(OffTextProperty); }
            set { SetValue(OffTextProperty, value); }
        }
        public static readonly DependencyProperty OffTextProperty =
            DependencyProperty.Register("OffText", typeof(string), typeof(Toggle), new PropertyMetadata("关"));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Toggle));

        public Toggle()
        {
            DefaultStyleKey = typeof(Toggle);
            //Loaded += Toggle_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsChecked = !IsChecked;
            ToggleChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Toggle_Loaded(object sender, RoutedEventArgs e)
        {
            //VisualStateManager.GoToState(this, IsChecked ? "On" : "Off", true);
        }
    }
}
