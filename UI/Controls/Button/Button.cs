using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.Controls.Base;

namespace UI.Controls.Button
{
    public class Button : System.Windows.Controls.Button
    {
        public IconTypes Icon
        {
            get { return (IconTypes)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon",
                typeof(IconTypes),
                typeof(Button), new PropertyMetadata(IconTypes.None));
        public Button()
        {
            DefaultStyleKey = typeof(Button);
        }

        //protected override void OnMouseEnter(MouseEventArgs e)
        //{
        //    base.OnMouseEnter(e);
        //    VisualStateManager.GoToState(this, "MouseOver", true);
        //}
        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    VisualStateManager.GoToState(this, "Normal", true);
        //}
    }
}
