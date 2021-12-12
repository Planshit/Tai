using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.Controls.List
{
    public class BaseList : Control
    {
        public ObservableCollection<string> Items { get { return (ObservableCollection<string>)GetValue(ItemsProperty); } set { SetValue(ItemsProperty, value); } }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<string>), typeof(BaseList), new PropertyMetadata(new PropertyChangedCallback(ItemsChanged)));

        private static void ItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BaseList;
            if (e.NewValue != e.OldValue)
            {
                control.Render();
            }
        }

        public string SelectedItem { get { return (string)GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(BaseList));

        private StackPanel Container;

        //private Dictionary<int, BaseListItem> ItemsMap;
        private List<BaseListItem> ItemsMap;

        public BaseList()
        {
            DefaultStyleKey = typeof(BaseList);
            Items = new ObservableCollection<string>();
            ItemsMap = new List<BaseListItem>();
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    AddItem(item as string);
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    RemoveItem(item as string);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Container = GetTemplateChild("Container") as StackPanel;
            Render();
        }

        private void Render()
        {
            if (Container != null)
            {
                Container.Children.Clear();
                ItemsMap.Clear();

                if (Items == null)
                {
                    return;
                }

                foreach (var item in Items)
                {
                    AddItem(item);
                }
            }
        }

        private void AddItem(string item)
        {
            if (Container == null)
            {
                return;
            }
            var itemControl = new BaseListItem();
            itemControl.Text = item;
            itemControl.MouseLeftButtonUp += ItemClick;
            itemControl.MouseRightButtonUp += ItemClick;
            ItemsMap.Add(itemControl);
            Container.Children.Add(itemControl);
        }

        private void RemoveItem(string item)
        {
            if (Container == null)
            {
                return;
            }
            var control = ItemsMap.Where(m => m.Text == item).FirstOrDefault();
            if (control != null)
            {
                Container.Children.Remove(control);
            }
        }

        private void ItemClick(object sender, MouseButtonEventArgs e)
        {
            ClearSelectedItems();
            (sender as BaseListItem).IsSelected = true;
            SelectedItem = (sender as BaseListItem).Text;
        }

        private void ClearSelectedItems()
        {
            foreach (var item in ItemsMap)
            {
                item.IsSelected = false;
            }
        }
    }
}
