
using System;
using System.Collections.Generic;
using System.Text;
using UI.Base.Color;
using UI.Controls.Base;

namespace UI.Controls.Navigation.Models
{
    public class NavigationItemModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public IconTypes Icon { get; set; }
        public ColorTypes IconColor { get; set; }
        public string BadgeText { get; set; }
        public string Uri { get; set; }
        //public bool IsSelected { get; set; }
    }
}
