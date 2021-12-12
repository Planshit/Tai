using Core.Models.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.SettingPanel
{
    public class SettingPanelItem : Control
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get { return (string)GetValue(DescriptionProperty); } set { SetValue(DescriptionProperty, value); } }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(SettingPanelItem));

        public object Content { get { return (object)GetValue(ContentProperty); } set { SetValue(ContentProperty, value); } }
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(SettingPanelItem));

        public SettingPanelItem()
        {
            DefaultStyleKey = typeof(SettingPanelItem);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

        }
    }
}
