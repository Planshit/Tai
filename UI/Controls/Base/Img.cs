using Core.Librarys.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace UI.Controls.Base
{
    public class Img : Control
    {
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
                control.Render();
            }
        }

        private Image _image;
        public Img()
        {
            DefaultStyleKey = typeof(Img);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _image = GetTemplateChild("image") as Image;
            Render();
        }

        private void Render()
        {
            if (_image == null)
            {
                return;
            }
            _image.Source = Imager.Load(URL);
        }
    }
}
