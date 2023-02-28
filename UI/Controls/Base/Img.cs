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

        public string Src
        {
            get { return (string)GetValue(SrcProperty); }
            set { SetValue(SrcProperty, value); }
        }
        public static readonly DependencyProperty SrcProperty =
            DependencyProperty.Register("Src",
                typeof(string),
                typeof(Img), new PropertyMetadata("pack://application:,,,/Tai;component/Resources/Icons/defaultIcon.png", new PropertyChangedCallback(OnPropertyChanged)));



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
            if (e.Property == URLProperty && e.OldValue != e.NewValue && e.NewValue != null)
            {
                control.Handle(e.NewValue.ToString());
            }
        }

        public Img()
        {
            DefaultStyleKey = typeof(Img);
        }

        private void Handle(string path)
        {
            string defaultIconFile = "pack://application:,,,/Tai;component/Resources/Icons/defaultIcon.png";
            if (string.IsNullOrEmpty(path))
            {
                Src = defaultIconFile;
                return;
            }
            if (path.IndexOf("pack:") != -1)
            {
                Src = path;
                return;
            }

            string src = path.IndexOf(":") != -1 ? path : System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            Src = File.Exists(src) ? src : defaultIconFile;
        }

    }
}
