using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.Select
{
    public class ImageSelect : Control
    {
        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }
        public static readonly DependencyProperty URLProperty =
            DependencyProperty.Register("URL",
                typeof(string),
                typeof(ImageSelect), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageSelect;
            if (e.Property == URLProperty)
            {
                control.IsSelected = !string.IsNullOrEmpty(control.URL);
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected",
                typeof(bool),
                typeof(ImageSelect), new PropertyMetadata(false));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth",
                typeof(double),
                typeof(ImageSelect), new PropertyMetadata((double)30));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight",
                typeof(double),
                typeof(ImageSelect), new PropertyMetadata((double)30));
        public Command SelectCommand { get; set; }
        public ImageSelect()
        {
            DefaultStyleKey = typeof(ImageSelect);
            SelectCommand = new Command(new Action<object>(OnSelect));
        }

        private void OnSelect(object obj)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            dlg.Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";


            bool? result = dlg.ShowDialog();


            if (result == true)
            {
                URL = dlg.FileName;
            }
        }
    }
}
