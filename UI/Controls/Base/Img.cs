using Core.Librarys.Image;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public bool IsAutoDispose
        {
            get { return (bool)GetValue(IsAutoDisposeProperty); }
            set { SetValue(IsAutoDisposeProperty, value); }
        }
        public static readonly DependencyProperty IsAutoDisposeProperty =
            DependencyProperty.Register("IsAutoDispose",
                typeof(bool),
                typeof(Img), new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));


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
        //private bool isRendered = false;
        //private Image _image;
        //Page page;
        private ImageBrush _image;
        public Img()
        {
            DefaultStyleKey = typeof(Img);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _image = GetTemplateChild("image") as ImageBrush;

            Render();
            //_image = GetTemplateChild("image") as Image;
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
           
            Render();
        }



        //private void Page_LayoutUpdated(object sender, EventArgs e)
        //{
        //    if (!isRendered)
        //    {
        //        Rect bounds = this.TransformToAncestor(page).TransformBounds(new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight));
        //        Rect rect = new Rect(0.0, 0.0, page.ActualWidth, page.ActualHeight);

        //        if (rect.Contains(bounds))
        //        {
        //            Debug.WriteLine("渲染！");

        //            isRendered = true;

        //            Render();
        //        }
        //    }
        //}

        //private void Img_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var parent = VisualTreeHelper.GetParent(this);
        //    while (!(parent is Page))
        //    {
        //        parent = VisualTreeHelper.GetParent(parent);
        //    }
        //    page = (parent as Page);

        //    page.LayoutUpdated += Page_LayoutUpdated;
        //}

        private async void Render()
        {
            if (_image == null || !IsAutoDispose)
            {
                return;
            }

            _image.ImageSource = null;
            //_image.Source = Imager.Load(URL);



            //_image.ImageSource = Imager.Load(URL);
            //BitmapImage bitimg = null;

            string img = URL != null ? URL.ToString() : "";

            var res = Task.Run(() =>
             {

                 Debug.WriteLine("load:" + img);
                 return Imager.Load(img);
             });

            _image.ImageSource = await res;
        }
    }
}
