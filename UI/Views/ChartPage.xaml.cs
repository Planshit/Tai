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
using UI.Controls.Charts.Model;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// ChartPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChartPage : Page
    {
        public ChartPage(ChartPageVM vm)
        {
            InitializeComponent();
            DataContext = vm;

            //var data = new List<ChartsDataModel>();
            //data.Add(new ChartsDataModel()
            //{
            //    Name="分布",
            //    //Color= "#2f69fe",
            //    Values = new double[] { 10, 20, 30, 40,30,403,30,50,43,43,23,65,87,43,23,23 }
            //});
            //data.Add(new ChartsDataModel()
            //{
            //    Name = "测试",

            //    //Color = "#da4d07",
            //    Values = new double[] { 110, 30, 10, 20, 330, 203, 3, 500, 53, 93, 23, 65, 87, 43, 23, 23 }
            //});

       
            //test.Data = data;
        }
    }
}
