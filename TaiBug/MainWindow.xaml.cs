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

namespace TaiBug
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogHyperlinkClick(object sender, RoutedEventArgs e)
        {
            string loggerName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         "Log", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            if (File.Exists(loggerName))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select, " + loggerName);
            }
        }
        private void IssuesHyperlinkClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Planshit/Tai/issues/new"));
        }
        private void EmailHyperlinkClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("heis@thelittlepandaisbehind.com");
            MessageBox.Show("邮箱已复制");
        }
        private void Restart(object sender, RoutedEventArgs e)
        {
            string taiPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Tai.exe");
            if (File.Exists(taiPath))
            {
                Process.Start(taiPath);
            }
            else
            {
                MessageBox.Show("Tai.exe 似乎已被删除", "重启失败提示", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.Start(new ProcessStartInfo("https://github.com/Planshit/Tai"));
            }
            Close();
        }
    }
}
