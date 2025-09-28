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
using UI.Servicers;

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

        /// <summary>
        /// 是否显示beta标识
        /// </summary>
        public bool IsBeta { get { return (bool)GetValue(IsBetaProperty); } set { SetValue(IsBetaProperty, value); } }
        public static readonly DependencyProperty IsBetaProperty = DependencyProperty.Register("IsBeta", typeof(bool), typeof(SettingPanelItem));

        public string DisplayName { get { return (string)GetValue(DisplayNameProperty); } set { SetValue(DisplayNameProperty, value); } }
        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(SettingPanelItem));
        
        public SettingPanelItem()
        {
            DefaultStyleKey = typeof(SettingPanelItem);
        }

        public void Init(ConfigAttribute configAttribute_, object content_)
        {
            Name = configAttribute_.Name.Replace(".", "_");
            DisplayName = LocalizationServicer.Instance.Translated(configAttribute_.Name);
            Description = LocalizationServicer.Instance.Translated(configAttribute_.Description);
            IsBeta = configAttribute_.IsBeta;
            Content = content_;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

        }
    }
}
