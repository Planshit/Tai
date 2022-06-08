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
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// DetailPage.xaml 的交互逻辑
    /// </summary>
    public partial class DetailPage : Page
    {
        public DetailPage(DetailPageVM vm)
        {
            InitializeComponent();

            DataContext = vm;

           
            //test.Options = new List<Controls.Select.SelectItemModel>()
            //{
            //    new Controls.Select.SelectItemModel()
            //{
            //    Name = "123abc",
            //    Img = "D:\\projects\\moneylog\\icon.png"
            //},new Controls.Select.SelectItemModel()
            //{
            //    Name = "AAA",
            //    Img = "D:\\projects\\moneylog\\icon.png"
            //}
            //};

            //test.SelectedItem = test.Options[0];
        }
    }
}
