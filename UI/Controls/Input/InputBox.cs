using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UI.Controls.Input
{
    public class InputBox : TextBox
    {
        public string Placeholder { get { return (string)GetValue(PlaceholderProperty); } set { SetValue(PlaceholderProperty, value); } }
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(InputBox));
        public InputBox()
        {
            DefaultStyleKey = typeof(InputBox);
        }


    }
}
