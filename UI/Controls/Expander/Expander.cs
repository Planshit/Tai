using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using UI.Controls.Button;
using UI.Controls.Input;
using UI.Controls.SettingPanel;

namespace UI.Controls.Expander
{
    public class Expander : System.Windows.Controls.Expander
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
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Expander));

        public Command ExpanderCommand { get; set; }
        private Border HeaderBorder_;
        private IconButton ExpBtn_;
        //  内容容器高度隐藏容器,用于辅助显示隐藏动画
        private Canvas ContentHeightCanvas_;
        //  内容部分容器边框,用于设置下移动画
        private Border ContentBorder_;
        private StackPanel ContentStackPanel_;


        private Storyboard storyboard;
        private DoubleAnimation expandTTFDoubleAnimation;
        private DoubleAnimation expandHeightDoubleAnimation;

        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
            ExpanderCommand = new Command(new Action<object>(OnExpanderCommand));
            Loaded += Expander_Loaded;
        }

        private void Expander_Loaded(object sender, RoutedEventArgs e)
        {
            ContentHeightCanvas_.SetValue(HeightProperty, ContentStackPanel_.ActualHeight);
            ContentStackPanel_.SizeChanged += ContentStackPanel__SizeChanged;
            //OnExpand(false);
        }

        private void CreateAnimations()
        {
            storyboard = new Storyboard();
            expandTTFDoubleAnimation = new DoubleAnimation();
            expandTTFDoubleAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(expandTTFDoubleAnimation, ContentBorder_);
            Storyboard.SetTargetProperty(expandTTFDoubleAnimation, new PropertyPath("(Border.RenderTransform).(TranslateTransform.Y)"));

            expandHeightDoubleAnimation = new DoubleAnimation();
            expandHeightDoubleAnimation.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
            Storyboard.SetTarget(expandHeightDoubleAnimation, ContentHeightCanvas_);
            Storyboard.SetTargetProperty(expandHeightDoubleAnimation, new PropertyPath("(Canvas.Height)"));


            storyboard.Children.Add(expandTTFDoubleAnimation);
            storyboard.Children.Add(expandHeightDoubleAnimation);

            storyboard.Completed += Storyboard_Completed;
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            ContentStackPanel_.SizeChanged -= ContentStackPanel__SizeChanged;

            if (IsExpanded)
            {
                ContentStackPanel_.SizeChanged += ContentStackPanel__SizeChanged;
            }
        }

        private void ContentStackPanel__SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnExpand(false);
        }

        private void OnExpanderCommand(object obj)
        {
            IsExpanded = !IsExpanded;
            OnExpand();
        }

        private void OnExpand(bool isAnimation = true)
        {
            //ContentBorder_.SizeChanged -= ContentBorder__SizeChanged;
            if (IsExpanded)
            {
                expandTTFDoubleAnimation.From = -ContentStackPanel_.ActualHeight;
                expandTTFDoubleAnimation.To = 0;

                expandHeightDoubleAnimation.From = 0;
                expandHeightDoubleAnimation.To = ContentStackPanel_.ActualHeight;
            }
            else
            {
                expandTTFDoubleAnimation.From = 0;
                expandTTFDoubleAnimation.To = -ContentStackPanel_.ActualHeight;

                expandHeightDoubleAnimation.From = ContentStackPanel_.ActualHeight;
                expandHeightDoubleAnimation.To = 0;
            }

            if (!isAnimation)
            {
                expandTTFDoubleAnimation.Duration = new Duration(TimeSpan.Zero);
                expandHeightDoubleAnimation.Duration = new Duration(TimeSpan.Zero);
            }
            else
            {
                expandTTFDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(.3));
                expandHeightDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(.25));
            }
            storyboard.Begin();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HeaderBorder_ = GetTemplateChild("HeaderBorder") as Border;
            ExpBtn_ = GetTemplateChild("ExpBtn") as IconButton;
            ContentHeightCanvas_ = GetTemplateChild("ContentHeight") as Canvas;
            ContentBorder_ = GetTemplateChild("Content") as Border;
            ContentStackPanel_ = GetTemplateChild("ContentStackPanel") as StackPanel;

            CreateAnimations();
            HandleEvent();
        }

        private void HandleEvent()
        {
            if (HeaderBorder_ != null)
            {
                HeaderBorder_.MouseEnter += HeaderBorder__MouseEnter;
                HeaderBorder_.MouseLeave += HeaderBorder__MouseLeave;
                HeaderBorder_.MouseLeftButtonDown += HeaderBorder__MouseLeftButtonDown;
                HeaderBorder_.MouseLeftButtonUp += HeaderBorder__MouseLeftButtonUp;
            }

            if(ExpBtn_!= null)
            {
                ExpBtn_.MouseLeftButtonUp += ExpBtn__MouseLeftButtonUp; ; 
            }

        }

        private void ExpBtn__MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void HeaderBorder__MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ExpBtn_.RaiseEvent(e);
        }

        private void HeaderBorder__MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExpBtn_.RaiseEvent(e);
        }

        private void HeaderBorder__MouseLeave(object sender, MouseEventArgs e)
        {
            ExpBtn_.RaiseEvent(e);
        }

        private void HeaderBorder__MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ExpBtn_.RaiseEvent(e);
        }
    }
}
