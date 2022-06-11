using Core.Librarys.Image;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UI.Controls.Base
{
    public class Img : Control
    {
        public CornerRadius Radius
        {
            get { return (CornerRadius)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius",
                typeof(CornerRadius),
                typeof(Img));


        /// <summary>
        /// 图片链接
        /// </summary>
        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }
        public static readonly DependencyProperty URLProperty =
            DependencyProperty.Register("URL",
                typeof(string),
                typeof(Img), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (d as Img);
            if (e.Property == URLProperty && e.OldValue != e.NewValue)
            {
                control.Handle();
            }
        }
      
        public Img()
        {
            DefaultStyleKey = typeof(Img);

        }

        private void Handle()
        {
            if (URL != null && URL.Length > 8 && URL.Substring(0, 8) == "AppIcons")
            {
                URL = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, URL);
            }
            if (URL == null || (URL.IndexOf("pack://") == -1 && !File.Exists(URL)))
            {
                URL = "pack://application:,,,/Tai;component/Resources/Icons/defaultIcon.png";
            }
        }
      
    }
}
