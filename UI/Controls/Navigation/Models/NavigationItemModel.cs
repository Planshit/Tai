
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
        /// <summary>
        /// 未选择时默认图标
        /// </summary>
        public IconTypes UnSelectedIcon { get; set; }

        private IconTypes SelectedIcon_ = IconTypes.None;
        /// <summary>
        /// 选中后图标
        /// </summary>
        public IconTypes SelectedIcon
        {
            get
            {

                if (SelectedIcon_ == IconTypes.None)
                {
                    return UnSelectedIcon;
                }
                else
                {
                    return SelectedIcon_;
                }
            }
            set
            {
                SelectedIcon_ = value;
            }
        }
        public ColorTypes IconColor { get; set; }
        public string BadgeText { get; set; }
        public string Uri { get; set; }
        //public bool IsSelected { get; set; }
    }
}
