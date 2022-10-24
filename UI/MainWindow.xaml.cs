using Core;
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
using UI.Controls.Window;
using UI.Servicers;

namespace UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : DefaultWindow
    {
        private readonly IThemeServicer themeServicer;
        public MainWindow(IThemeServicer themeServicer)
        {
            InitializeComponent();
            this.themeServicer = themeServicer;
            themeServicer.SetMainWindow(this);
            themeServicer.UpdateWindowStyle();
            Unloaded += Page_Unloaded;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= Page_Unloaded;
            DataContext = null;
        }
    }
}
