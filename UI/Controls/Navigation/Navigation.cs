using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UI.Controls.Navigation.Models;

namespace UI.Controls.Navigation
{
    public class Navigation : Control
    {
        public ContextMenu ItemContextMenu
        {
            get { return (ContextMenu)GetValue(ItemContextMenuProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty ItemContextMenuProperty =
            DependencyProperty.Register("ItemContextMenu", typeof(ContextMenu), typeof(Navigation));

        public NavigationItemModel SelectedItem
        {
            get { return (NavigationItemModel)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(NavigationItemModel), typeof(Navigation), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Navigation;
            if (e.NewValue != e.OldValue)
            {
                var newItem = e.NewValue as NavigationItemModel;
                var oldItem = e.OldValue as NavigationItemModel;
                if (newItem != null && control.ItemsDictionary.ContainsKey(newItem.ID))
                {
                    control.ItemsDictionary[newItem.ID].IsSelected = true;
                }
                if (oldItem != null && control.ItemsDictionary.ContainsKey(oldItem.ID))
                {
                    control.ItemsDictionary[oldItem.ID].IsSelected = false;
                }
                control.ScrollToActive();
            }
        }

        public bool IsShowNavigation
        {
            get { return (bool)GetValue(IsShowNavigationProperty); }
            set { SetValue(IsShowNavigationProperty, value); }
        }
        public static readonly DependencyProperty IsShowNavigationProperty =
            DependencyProperty.Register("IsShowNavigation", typeof(bool), typeof(Navigation), new PropertyMetadata(true, new PropertyChangedCallback(OnIsShowNavigationChanged)));

        private static void OnIsShowNavigationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Navigation;
            if (e.NewValue != e.OldValue)
            {
                if (control.IsShowNavigation && control.Visibility == Visibility.Collapsed && control.WindowWidth > 700)
                {
                    control.Visibility = Visibility.Visible;
                }
                if (!control.IsShowNavigation && control.Visibility == Visibility.Visible)
                {
                    control.Visibility = Visibility.Collapsed;
                }
            }
        }

        public object TopExtContent
        {
            get { return (object)GetValue(TopExtContentProperty); }
            set { SetValue(TopExtContentProperty, value); }
        }
        public static readonly DependencyProperty TopExtContentProperty =
            DependencyProperty.Register("TopExtContent", typeof(object), typeof(Navigation));
        public object BottomExtContent
        {
            get { return (object)GetValue(BottomExtContentProperty); }
            set { SetValue(BottomExtContentProperty, value); }
        }
        public static readonly DependencyProperty BottomExtContentProperty =
            DependencyProperty.Register("BottomExtContent", typeof(object), typeof(Navigation));
        public ObservableCollection<NavigationItemModel> Data
        {
            get
            {
                return (ObservableCollection<NavigationItemModel>)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(ObservableCollection<NavigationItemModel>), typeof(Navigation), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Navigation;
            if (e.NewValue != e.OldValue)
            {
                if (control.Data != null)
                {
                    control.Data.CollectionChanged -= control.Data_CollectionChanged;
                    control.Data.CollectionChanged += control.Data_CollectionChanged;

                    foreach (var item in control.Data)
                    {
                        control.AddItem(item);
                    }
                }
            }
        }

        public double WindowWidth
        {
            get
            {
                return (double)GetValue(WindowWidthProperty);
            }
            set
            {
                SetValue(WindowWidthProperty, value);
            }
        }
        public static readonly DependencyProperty WindowWidthProperty =
            DependencyProperty.Register("WindowWidth", typeof(double), typeof(Navigation), new PropertyMetadata(new PropertyChangedCallback(OnWindowWidthChanged)));

        private static void OnWindowWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Navigation;
            if (control.IsShowNavigation)
            {
                if ((double)e.NewValue <= 700)
                {
                    control.Visibility = Visibility.Collapsed;
                }
                else
                {

                    control.Visibility = Visibility.Visible;
                }
            }
        }


        #region items property


        //protected static PropertyChangedCallback ItemsPropertyChangedCallback = new PropertyChangedCallback(ItemsPropertyChanged);

        //public static DependencyProperty ItemsProperty = DependencyProperty.RegisterAttached("Items", typeof(ObservableCollection<NavigationItemModel>), typeof(Navigation), new PropertyMetadata(null, ItemsPropertyChangedCallback));

        //private static void ItemsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = (Navigation)sender;
        //    if (control == null)
        //    {
        //        return;
        //    }
        //    control.UnregisterItems(e.OldValue as ObservableCollection<NavigationItemModel>);
        //    control.RegisterItems(e.NewValue as ObservableCollection<NavigationItemModel>);
        //}

        //public ObservableCollection<NavigationItemModel> Items
        //{
        //    get
        //    {
        //        return (ObservableCollection<NavigationItemModel>)GetValue(ItemsProperty);
        //    }
        //    set
        //    {
        //        SetValue(ItemsProperty, value);
        //    }
        //}

        //protected void UnregisterItems(ObservableCollection<NavigationItemModel> items)
        //{
        //    if (items == null)
        //    {
        //        return;
        //    }
        //    items.CollectionChanged -= ItemsChanged;
        //}

        //protected void RegisterItems(ObservableCollection<NavigationItemModel> items)
        //{
        //    if (items == null)
        //    {
        //        return;
        //    }
        //    items.CollectionChanged += ItemsChanged;
        //}

        //protected virtual void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var item in e.NewItems)
        //        {
        //            var data = item as NavigationItemModel;
        //            AddItem(data);
        //        }
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach (var item in e.OldItems)
        //        {
        //            var data = item as NavigationItemModel;
        //            RemoveItem(data);
        //        }
        //    }
        //}
        #endregion
        public event RoutedEventHandler OnSelected;
        public event RoutedEventHandler OnMouseRightButtonUP;

        private StackPanel ItemsPanel;
        private Dictionary<int, NavigationItem> ItemsDictionary;
        //  选中标记块
        private Border ActiveBlock;
        //  动画
        Storyboard storyboard;
        //  滚动动画
        DoubleAnimation scrollAnimation;
        //  伸缩动画
        DoubleAnimation stretchAnimation;

        public Navigation()
        {
            DefaultStyleKey = typeof(Navigation);
            ItemsDictionary = new Dictionary<int, NavigationItem>();
            Loaded += Navigation_Loaded;
        }

        private void Navigation_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollToActive();
        }

        #region create animation
        private void CreateAnimations()
        {
            if (ActiveBlock != null)
            {
                var tfg = new TransformGroup();
                tfg.Children.Add(new TranslateTransform() { X = 0, Y = 0 });
                tfg.Children.Add(new ScaleTransform() { ScaleX = 0, ScaleY = 0 });
                ActiveBlock.RenderTransform = tfg;
            }
            storyboard = new Storyboard();
            scrollAnimation = new DoubleAnimation();
            stretchAnimation = new DoubleAnimation();

            //  滚动动画
            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.35));

            Storyboard.SetTarget(scrollAnimation, ActiveBlock);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));

            //  伸缩动画
            stretchAnimation.AutoReverse = true;
            stretchAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            stretchAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.35));

            Storyboard.SetTarget(stretchAnimation, ActiveBlock);
            Storyboard.SetTargetProperty(stretchAnimation, new PropertyPath("RenderTransform.Children[1].ScaleY"));

            storyboard.Children.Add(stretchAnimation);
            storyboard.Children.Add(scrollAnimation);
        }
        #endregion

        #region temp region
        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    AddItem(item as NavigationItemModel);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    RemoveItem(item as NavigationItemModel);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var ritem in e.NewItems)
                {
                    var item = ritem as NavigationItemModel;
                    var id = item.ID;
                    ItemsDictionary[id].Icon = item.UnSelectedIcon;
                    ItemsDictionary[id].Title = item.Title;
                    ItemsDictionary[id].SelectedIcon = item.SelectedIcon;

                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsPanel = GetTemplateChild("ItemsPanel") as StackPanel;
            ActiveBlock = GetTemplateChild("ActiveBlock") as Border;

            CreateAnimations();
            Render();
        }
        private int CreateID()
        {
            if (ItemsDictionary.Count == 0)
            {
                return 1;
            }
            return ItemsDictionary.Max(m => m.Key) + 1;
        }
        private void AddItem(NavigationItemModel item)
        {
            if (ItemsPanel != null)
            {
                var navItem = new NavigationItem();
                int id = item.ID < -1 ? CreateID() : item.ID;
                if (ItemsDictionary.ContainsKey(id))
                {
                    return;
                }

                item.ID = id;
                navItem.ID = id;
                navItem.Title = item.Title;
                navItem.Icon = item.UnSelectedIcon;
                navItem.SelectedIcon = item.SelectedIcon;
                navItem.IconColor = item.IconColor;
                navItem.BadgeText = item.BadgeText;
                navItem.Uri = item.Uri;

                if (!string.IsNullOrEmpty(item.Title))
                {
                    navItem.MouseUp += NavItem_MouseUp;
                    navItem.Unloaded += (e, c) =>
                    {
                        navItem.MouseUp -= NavItem_MouseUp;
                    };
                }
                ItemsPanel.Children.Add(navItem);
                ItemsDictionary.Add(id, navItem);
                //if (item.IsSelected)
                //{
                //    SelectedItem = item;
                //}
            }
        }

        private void NavItem_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var navitem = sender as NavigationItem;
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                //左键选中

                SelectedItem = Data.Where(m => m.ID == navitem.ID).FirstOrDefault();
                OnSelected?.Invoke(this, null);

                if (SelectedItem != null && SelectedItem.ID != navitem.ID)
                {
                    ScrollToActive();
                }
            }
            if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
            {
                //右键
                var args = new RoutedEventArgs();
                args.RoutedEvent = e.RoutedEvent;
                args.Source = Data.Where(m => m.ID == navitem.ID).FirstOrDefault();
                OnMouseRightButtonUP?.Invoke(this, args);
            }
        }

        private void RemoveItem(NavigationItemModel item)
        {
            var navItem = ItemsDictionary[item.ID];
            ItemsPanel.Children.Remove(navItem);
            ItemsDictionary.Remove(item.ID);
        }
        private void Render()
        {
            ItemsPanel.Children.Clear();
            ItemsDictionary.Clear();

            if (Data != null)
            {
                foreach (var item in Data)
                {
                    AddItem(item);
                }
            }

            UpdateActiveLocation();
        }
        #endregion
        private void UpdateActiveLocation()
        {
            if (SelectedItem == null || !IsLoaded)
            {
                return;
            }
            var item = ItemsDictionary[SelectedItem.ID];
            item.Loaded += (e, c) =>
            {
                ScrollToActive(0);
            };




        }
        private void ScrollToActive(double animationDuration = 0.35)
        {
            //  获取选中项
            if (SelectedItem == null || ItemsDictionary.Count == 0 || !ItemsDictionary.ContainsKey(SelectedItem.ID))
            {
                return;
            }
            var item = ItemsDictionary[SelectedItem.ID];
            item.IsSelected = true;

            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(animationDuration));
            stretchAnimation.Duration = new Duration(TimeSpan.FromSeconds(animationDuration));


            //  选中项的坐标
            Point relativePoint = item.TransformToAncestor(this).Transform(new Point(0, 0));

            //  设定动画方向
            var activeBlockTTF = (ActiveBlock.RenderTransform as TransformGroup).Children[0] as TranslateTransform;
            scrollAnimation.To = relativePoint.Y + 8;

            //  伸缩动画

            stretchAnimation.To = 1.6;

            if (relativePoint.Y > activeBlockTTF.Y)
            {
                //  向下移动
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(0, activeBlockTTF.Y));
                transformGroup.Children.Add(new ScaleTransform(1, 1, 0, 200));

                ActiveBlock.RenderTransform = transformGroup;
            }
            else
            {
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new TranslateTransform(0, activeBlockTTF.Y));
                transformGroup.Children.Add(new ScaleTransform(1, 1, 0, 0));

                ActiveBlock.RenderTransform = transformGroup;

            }

            storyboard.Begin();
        }
    }
}
