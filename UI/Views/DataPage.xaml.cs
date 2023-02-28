using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// IndexPage.xaml 的交互逻辑
    /// </summary>
    public partial class DataPage : TPage
    {
        public DataPage(DataPageVM vm)
        {
            InitializeComponent();
 
            DataContext = vm;
        }

    }
}
