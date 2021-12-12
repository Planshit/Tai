using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.Controls.List
{
    public class BaseListItem : Control
    {
        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BaseListItem));

        public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(BaseListItem));

        //public Command RemoveCommand { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
        //public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(BaseListItem));

        //public event EventHandler ItemChanged;

        public BaseListItem()
        {
            DefaultStyleKey = typeof(BaseListItem);
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, "MouseOver", true);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, "Normal", true);
        }
    }
}
