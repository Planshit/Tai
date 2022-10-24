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
            DependencyProperty.Register("Data", typeof(ObservableCollection<string>), typeof(Tabbar), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Tabbar;
            if (e.NewValue != e.OldValue)
            {

            }
        }


        //private StackPanel ItemsPanel;
        private List<TextBlock> ItemsDictionary;
        private Grid ItemsContainer;
        //  选中标记块
        private Border ActiveBlock;
        //  动画
        Storyboard storyboard;
        public Tabbar()
        {
            DefaultStyleKey = typeof(Tabbar);
            ItemsDictionary = new List<TextBlock>();
            storyboard = new Storyboard();
            //SelectedTextColor = (Color)ColorConverter.ConvertFromString("#2b20d9");
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    foreach (var item in e.NewItems)
            //    {
            //        AddItem(item as NavigationItemModel);
            //    }
            //}
            //if (e.Action == NotifyCollectionChangedAction.Remove)
            //{
            //    foreach (var item in e.OldItems)
            //    {
            //        RemoveItem(item as NavigationItemModel);
            //    }
            //}
            //if (e.Action == NotifyCollectionChangedAction.Replace)
            //{
            //    foreach (var ritem in e.NewItems)
            //    {
            //        var item = ritem as NavigationItemModel;
            //        var id = item.ID;
            //        ItemsDictionary[id].Icon = item.Icon;
            //        ItemsDictionary[id].Title = item.Title;

            //    }
            //}
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //ItemsPanel = GetTemplateChild("ItemsPanel") as StackPanel;
            ActiveBlock = GetTemplateChild("ActiveBlock") as Border;
            ItemsContainer = GetTemplateChild("ItemsContainer") as Grid;
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
                control.FontSize = 18;
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

        private void ScrollToActive(int oldSelectedIndex = 0)
        {
            if (oldSelectedIndex > ItemsDictionary.Count || ItemsDictionary.Count == 0 || !IsLoaded)
            {
                return;
            }
            //  获取选中项
            Debug.WriteLine(IsLoaded + "");
            var item = ItemsDictionary[SelectedIndex];
            var oldSelectedItem = ItemsDictionary[oldSelectedIndex];

            //ActiveBlock.Height = item.ActualHeight * Data.Count;

            //  选中项的坐标
            Point relativePoint = item.TransformToAncestor(this).Transform(new Point(0, 0));

            //  创建动画

            //  设定动画方向
            //var activeBlockTTF = (ActiveBlock.RenderTransform as TransformGroup).Children[0] as TranslateTransform;

            //var transformGroup = new TransformGroup();
            //transformGroup.Children.Add(new TranslateTransform(0, activeBlockTTF.Y));
            //transformGroup.Children.Add(new ScaleTransform(1, 1, 0, 0));

            //ActiveBlock.RenderTransform = transformGroup;


            storyboard.Children.Clear();

            var activeBlockTG = ActiveBlock.RenderTransform as TransformGroup;
            var blockOldX = (activeBlockTG.Children[0] as TranslateTransform).X;
            DoubleAnimation scrollAnimation = new DoubleAnimation();
            //scrollAnimation.From = activeBlockTTF.Y;
            //double scrollX = relativePoint.X + item.ActualWidth / 2 - ActiveBlock.ActualWidth / 2;
            double scrollX = relativePoint.X;

            scrollX = scrollX < 0 ? 0 : scrollX;

            //scrollX= scrollX + item.ActualWidth / 2 - ActiveBlock.ActualWidth / 2;
            if (scrollX != 0)
            {
                //scrollX = scrollX > blockOldX ? scrollX - 10 : scrollX;
            }

            Debug.WriteLine(blockOldX + " -> " + scrollX);
            scrollAnimation.To = scrollX;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, ActiveBlock);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].X"));

            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));

            //  文字大小动画
            DoubleAnimation fontSizeAnimation = new DoubleAnimation();
            //scrollAnimation.From = activeBlockTTF.Y;
            fontSizeAnimation.To = 18;

            fontSizeAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(fontSizeAnimation, item);
            Storyboard.SetTargetProperty(fontSizeAnimation, new PropertyPath("FontSize"));

            fontSizeAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));



            //  文字颜色动画
            ColorAnimation fontColorAnimation = new ColorAnimation();
            //scrollAnimation.From = activeBlockTTF.Y;
            fontColorAnimation.To = SelectedTextColor;

            fontColorAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(fontColorAnimation, item);
            Storyboard.SetTargetProperty(fontColorAnimation, new PropertyPath("(TextBlock.Foreground).(SolidColorBrush.Color)"));

            fontColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));


            storyboard.Children.Add(scrollAnimation);
            storyboard.Children.Add(fontSizeAnimation);

            storyboard.Children.Add(fontColorAnimation);


            if (oldSelectedIndex != SelectedIndex)
            {
                DoubleAnimation oldFontSizeAnimation = new DoubleAnimation();
                //scrollAnimation.From = activeBlockTTF.Y;
                oldFontSizeAnimation.To = 14;

                oldFontSizeAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                Storyboard.SetTarget(oldFontSizeAnimation, oldSelectedItem);
                Storyboard.SetTargetProperty(oldFontSizeAnimation, new PropertyPath("FontSize"));

                oldFontSizeAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));

                ColorAnimation oldFontColorAnimation = new ColorAnimation();
                //scrollAnimation.From = activeBlockTTF.Y;
                oldFontColorAnimation.To = (Color)ColorConverter.ConvertFromString("#CCCCCC");

                oldFontColorAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                Storyboard.SetTarget(oldFontColorAnimation, oldSelectedItem);
                Storyboard.SetTargetProperty(oldFontColorAnimation, new PropertyPath("(TextBlock.Foreground).(SolidColorBrush.Color)"));

                oldFontColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

                storyboard.Children.Add(oldFontSizeAnimation);
                storyboard.Children.Add(oldFontColorAnimation);

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
                    text.FontSize = 14;
                    text.Foreground = UI.Base.Color.Colors.GetFromString("#ccc");

                    //DoubleAnimation fontSizeAnimation = new DoubleAnimation();
                    ////scrollAnimation.From = activeBlockTTF.Y;
                    //fontSizeAnimation.To = 14;

                    //fontSizeAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                    //Storyboard.SetTarget(fontSizeAnimation, (UIElement)item);
                    //Storyboard.SetTargetProperty(fontSizeAnimation, new PropertyPath("FontSize"));

                    //fontSizeAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.1));

                    //ColorAnimation oldFontColorAnimation = new ColorAnimation();
                    ////scrollAnimation.From = activeBlockTTF.Y;
                    //oldFontColorAnimation.To = (Color)ColorConverter.ConvertFromString("#CCCCCC");

                    //oldFontColorAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
                    //Storyboard.SetTarget(oldFontColorAnimation, (UIElement)item);
                    //Storyboard.SetTargetProperty(oldFontColorAnimation, new PropertyPath("(TextBlock.Foreground).(SolidColorBrush.Color)"));

                    //oldFontColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

                    //storyboard.Children.Add(fontSizeAnimation);
                    //storyboard.Children.Add(oldFontColorAnimation);
                }
            }

            //storyboard.Begin();
        }
    }
}
