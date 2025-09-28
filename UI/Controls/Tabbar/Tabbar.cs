using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UI.Controls.Navigation.Models;

namespace UI.Controls.Tabbar
{
    public class Tabbar : Control
    {

        public Color SelectedTextColor
        {
            get { return (Color)GetValue(SelectedTextColorProperty); }
            set { SetValue(SelectedTextColorProperty, value); }
        }
        public static readonly DependencyProperty SelectedTextColorProperty =
            DependencyProperty.Register("SelectedTextColor", typeof(Color), typeof(Tabbar));
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Tabbar), new PropertyMetadata((int)0, new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Tabbar;
            if (e.NewValue != e.OldValue)
            {
                control.ScrollToActive(int.Parse(e.OldValue.ToString()));
            }
        }


        public ObservableCollection<string> Data
        {
            get
            {
                return (ObservableCollection<string>)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(ObservableCollection<string>), typeof(Tabbar));


        //private StackPanel ItemsPanel;
        private List<TextBlock> ItemsDictionary;
        private Grid ItemsContainer;
        //  选中标记块
        private Border ActiveBlock;
        //  动画
        Storyboard storyboard;
        DoubleAnimation scrollAnimation;
        ColorAnimation fontColorAnimation;
        ColorAnimation oldFontColorAnimation;
        public Tabbar()
        {
            DefaultStyleKey = typeof(Tabbar);
            ItemsDictionary = new List<TextBlock>();
        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //ItemsPanel = GetTemplateChild("ItemsPanel") as StackPanel;
            ActiveBlock = GetTemplateChild("ActiveBlock") as Border;
            ItemsContainer = GetTemplateChild("ItemsContainer") as Grid;

            CreateAnimations();
            Render();
        }


        private void AddItem(string item, int col)
        {
            if (ItemsContainer != null)
            {
                var control = new TextBlock();
                control.TextAlignment = TextAlignment.Center;
                control.Text = item;
                control.Margin = new Thickness(0, 0, 10, 0);
                control.FontSize = 16;
                control.Cursor = Cursors.Hand;
                control.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"));
                control.VerticalAlignment = VerticalAlignment.Bottom;
                control.MouseUp += (e, c) =>
                {
                    int index = Data.IndexOf(item);
                    if (SelectedIndex != index)
                    {
                        SelectedIndex = index;
                    }
                };
                if (Data.IndexOf(item) == Data.Count - 1)
                {
                    control.Loaded += (e, c) =>
                    {
                        ActiveBlock.Width = control.ActualWidth;
                        ScrollToActive();
                        Reset();
                    };
                }
                Grid.SetColumn(control, col);
                ItemsContainer.Children.Add(control);
                ItemsDictionary.Add(control);
            }
        }



        private void Render()
        {
            if (Data != null)
            {
                ItemsContainer.Children.Clear();
                ItemsContainer.ColumnDefinitions.Clear();
                ItemsDictionary.Clear();

                for (int i = 0; i < Data.Count; i++)
                {
                    var item = Data[i];

                    ItemsContainer.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });

                    AddItem(item, i);

                }
                //foreach (var item in Data)
                //{

                //    AddItem(item);
                //}

            }
        }

        #region create animations
        private void CreateAnimations()
        {
            storyboard = new Storyboard();
            scrollAnimation = new DoubleAnimation();
            //scrollAnimation.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut, Amplitude = .8 };
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));

            Storyboard.SetTarget(scrollAnimation, ActiveBlock);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].X"));




            fontColorAnimation = new ColorAnimation();
            //fontColorAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            fontColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));

            Storyboard.SetTargetProperty(fontColorAnimation, new PropertyPath("(TextBlock.Foreground).(SolidColorBrush.Color)"));


            oldFontColorAnimation = new ColorAnimation();
            oldFontColorAnimation.To = (Color)ColorConverter.ConvertFromString("#CCCCCC");
            oldFontColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            //oldFontColorAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTargetProperty(oldFontColorAnimation, new PropertyPath("(TextBlock.Foreground).(SolidColorBrush.Color)"));


            //storyboard.Children.Add(oldFontColorAnimation);
            storyboard.Children.Add(scrollAnimation);
            storyboard.Children.Add(fontColorAnimation);

        }
        #endregion
        private void ScrollToActive(int oldSelectedIndex = 0)
        {
            if (oldSelectedIndex > ItemsDictionary.Count || ItemsDictionary.Count == 0 || !IsLoaded)
            {
                return;
            }
            //  获取选中项
            var item = ItemsDictionary[SelectedIndex];
            var oldSelectedItem = ItemsDictionary[oldSelectedIndex];

            //  选中项的坐标
            double leftOffset = item.TransformToAncestor(ItemsContainer).Transform(new Point(0, 0)).X;

            scrollAnimation.To = leftOffset;
            
            ActiveBlock.Width = item.ActualWidth;


            //  文字颜色动画
            fontColorAnimation.To = SelectedTextColor;
            Storyboard.SetTarget(fontColorAnimation, item);


            if (oldSelectedIndex != SelectedIndex)
            {
                Storyboard.SetTarget(oldFontColorAnimation, oldSelectedItem);
                storyboard.Children.Add(oldFontColorAnimation);
            }
            else
            {
                storyboard.Children.Remove(oldFontColorAnimation);
            }
            storyboard.Begin();
        }

        private void Reset()
        {
            Storyboard storyboard = new Storyboard();

            foreach (var item in ItemsContainer.Children)
            {
                if (item != ItemsContainer.Children[SelectedIndex])
                {
                    var text = item as TextBlock;
                    text.Foreground = UI.Base.Color.Colors.GetFromString("#ccc");

                }
            }

        }
    }
}
