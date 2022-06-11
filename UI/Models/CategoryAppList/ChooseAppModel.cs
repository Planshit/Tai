using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Controls.Select;

namespace UI.Models.CategoryAppList
{
    public class ChooseAppModel
    {
        public bool IsChoosed { get; set; }
        public AppModel App { get; set; }
        public SelectItemModel Value { get; set; } = new SelectItemModel();
        public Visibility Visibility { get; set; } = Visibility.Visible;
    }
}
