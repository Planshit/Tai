using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.Select
{
    public class Select : Control
    {
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen",
                typeof(bool),
                typeof(Select));
        public List<SelectItemModel> Options
        {
            get { return (List<SelectItemModel>)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }
        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options",
                typeof(List<SelectItemModel>),
                typeof(Select), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (d as Select);
            if (e.Property == OptionsProperty && e.OldValue != e.NewValue)
            {
                control.RenderOptions();
            }
            if (e.Property == SelectItemProperty && e.NewValue != e.OldValue)
            {
                control.OnSelectedItemChange();
            }
        }

        public SelectItemModel SelectedItem
        {
            get { return (SelectItemModel)GetValue(SelectItemProperty); }
            set { SetValue(SelectItemProperty, value); }
        }

        public static readonly DependencyProperty SelectItemProperty = DependencyProperty.Register("SelectedItem", typeof(SelectItemModel), typeof(Select), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));


        private StackPanel _optionsContainer;
        public Select()
        {
            DefaultStyleKey = typeof(Select);

            MouseLeftButtonUp += Select_MouseLeftButtonUp;
        }

        private void Select_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsOpen = !IsOpen;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _optionsContainer = GetTemplateChild("OptionsContainer") as StackPanel;
            this.RenderOptions();
        }

        public void RenderOptions()
        {
            if (_optionsContainer == null || Options == null)
            {
                return;
            }

            _optionsContainer?.Children.Clear();

            foreach (var item in Options)
            {
                var option = new Option();
                option.Value = item;
                option.IsChecked = SelectedItem?.Name == item.Name;
                option.MouseLeftButtonUp += Option_MouseLeftButtonUp; ;
                _optionsContainer.Children.Add(option);
            }
        }

        private void Option_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var option = sender as Option;
            if (option != null)
            {
                SelectedItem = option.Value;
                OnSelectedItemChange();
            }
        }

        private void OnSelectedItemChange()
        {
            foreach (Option option in _optionsContainer.Children)
            {
                option.IsChecked = SelectedItem.Name == option.Value.Name;
            }
        }
    }
}
