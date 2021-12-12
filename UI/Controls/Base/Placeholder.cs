using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace UI.Controls.Base
{
    public class Placeholder : Control
    {
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
                typeof(CornerRadius),
                typeof(Placeholder));

        private Border Flash;
        public Placeholder()
        {

            DefaultStyleKey = typeof(Placeholder);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Flash = GetTemplateChild("Flash") as Border;
            Loaded += Placeholder_Loaded;
        }

        private void Placeholder_Loaded(object sender, RoutedEventArgs e)
        {
            Animation();
        }

        private void Animation()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.From = -10;
            scrollAnimation.To = ActualWidth;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, Flash);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].X"));
            scrollAnimation.AutoReverse = true;
            scrollAnimation.RepeatBehavior = RepeatBehavior.Forever;
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

            storyboard.Children.Add(scrollAnimation);

            storyboard.Begin();
        }

    }
}
