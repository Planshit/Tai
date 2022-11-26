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
using UI.Controls;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// CategoryAppListPage.xaml 的交互逻辑
    /// </summary>
    public partial class CategoryAppListPage : TPage
    {
        public CategoryAppListPage(CategoryAppListPageVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
