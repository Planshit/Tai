using Core.Models.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UI.Controls.Button;
using UI.Controls.Input;
using UI.Controls.List;
using UI.Servicers;

namespace UI.Controls.SettingPanel
{
    public class SettingPanelMultiItem : Control
    {
        /// <summary>
        /// 所有设置数据
        /// </summary>
        public Dictionary<string, List<string>> SettingData { get { return (Dictionary<string, List<string>>)GetValue(SettingDataProperty); } set { SetValue(SettingDataProperty, value); } }
        public static readonly DependencyProperty SettingDataProperty = DependencyProperty.Register("SettingData", typeof(Dictionary<string, List<string>>), typeof(SettingPanelMultiItem));
        public bool Fold { get { return (bool)GetValue(FoldProperty); } set { SetValue(FoldProperty, value); } }
        public static readonly DependencyProperty FoldProperty = DependencyProperty.Register("Fold", typeof(bool), typeof(SettingPanelMultiItem), new PropertyMetadata(false));

        public object Data { get { return (object)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(SettingPanelMultiItem));

        public Command OnRemoveAction { get { return (Command)GetValue(OnRemoveActionProperty); } set { SetValue(OnRemoveActionProperty, value); } }
        public static readonly DependencyProperty OnRemoveActionProperty = DependencyProperty.Register("OnRemoveAction", typeof(Command), typeof(SettingPanelMultiItem));
        public Command OnFoldAction { get { return (Command)GetValue(OnFoldActionProperty); } set { SetValue(OnFoldActionProperty, value); } }
        public static readonly DependencyProperty OnFoldActionProperty = DependencyProperty.Register("OnFoldAction", typeof(Command), typeof(SettingPanelMultiItem));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SettingPanelMultiItem));
        public event EventHandler DataChanged;

        private StackPanel Container;
        private object configData;
        private IconButton FoldBtn;
        public SettingPanelMultiItem()
        {
            DefaultStyleKey = typeof(SettingPanelMultiItem);

            if (SettingData == null)
            {
                SettingData = new Dictionary<string, List<string>>();
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Container = GetTemplateChild("Container") as StackPanel;
            FoldBtn = GetTemplateChild("FoldBtn") as IconButton;

            OnFoldAction = new Command(new Action<object>(FoldAction));
            Render();
        }

        private void FoldAction(object obj)
        {
            Fold = !Fold;
            if (Fold)
            {
                FoldBtn.Icon = Base.IconTypes.ChevronDown;

                FoldBtn.ToolTip = "展开";
            }
            else
            {
                FoldBtn.Icon = Base.IconTypes.ChevronUp;

                FoldBtn.ToolTip = "收起";

            }
        }

        private void Render()
        {
            if (Container == null)
            {
                return;
            }

            Container.Children.Clear();

            configData = Activator.CreateInstance(Data.GetType());

            // 遍历方法特性
            foreach (PropertyInfo pi in Data.GetType().GetProperties())
            {
                foreach (Attribute attr in pi.GetCustomAttributes(true))
                {
                    if (attr is ConfigAttribute)
                    {
                        ConfigAttribute attribute = (ConfigAttribute)attr;

                        if (pi.PropertyType == typeof(string))
                        {
                            //  输入框
                            RenderTextBox(attribute, pi);
                        }
                        else if (pi.PropertyType == typeof(List<string>))
                        {
                            //  String 集合
                            RenderStringList(attribute, pi);
                        }
                    }
                }
            }
        }

        private void RenderStringList(ConfigAttribute attribute, PropertyInfo pi)
        {
            var title = new SettingPanelItem();
            title.Name = LocalizationServicer.Instance.Translated(attribute.Name);
            title.Description = LocalizationServicer.Instance.Translated(attribute.Description);

            Container.Children.Add(title);

            var list = pi.GetValue(Data) as List<string>;

            if (SettingData.ContainsKey(pi.Name))
            {
                SettingData[pi.Name] = SettingData[pi.Name].Concat(list).ToList();
            }
            else
            {
                SettingData.Add(pi.Name, list);
            }

            var listControl = new BaseList();

            listControl.Loaded += (sender, args) =>
              {
                  listControl.Items.CollectionChanged += (o, e) =>
                  {
                      var newData = new List<string>();
                      foreach (var item in listControl.Items)
                      {
                          newData.Add(item.ToString());
                      }
                      pi.SetValue(configData, newData);
                      Data = configData;
                      DataChanged?.Invoke(this, EventArgs.Empty);
                  };
                  //((INotifyCollectionChanged)listControl.Items).CollectionChanged += (sender2, e) =>
                  //{
                  //    var newData = new List<string>();
                  //    foreach (var item in listControl.Items)
                  //    {
                  //        newData.Add(item.ToString());
                  //    }
                  //    pi.SetValue(configData, newData);
                  //    Data = configData;
                  //    DataChanged?.Invoke(this, EventArgs.Empty);
                  //};
              };
            listControl.Margin = new Thickness(15, 0, 15, 10);

            if (list != null)
            {
                foreach (string item in list)
                {
                    listControl.Items.Add(item);
                }
            }
            else
            {
                list = new List<string>();
            }
            var contextMenu = new ContextMenu();

            var contextMenuItemDel = new MenuItem();
            contextMenuItemDel.Header = "移除";
            contextMenuItemDel.Click += (e, c) =>
            {
                listControl.Items.Remove(listControl.SelectedItem);
            };
            contextMenu.Items.Add(contextMenuItemDel);
            listControl.ContextMenu = contextMenu;


            var addInputBox = new InputBox();
            addInputBox.Placeholder = attribute.Name;
            addInputBox.Margin = new Thickness(0, 0, 10, 0);

            var addBtn = new Button.Button();
            //addBtn.Margin = new Thickness(15, 0, 15, 10);
            addBtn.Content = "添加";

            addBtn.Click += (e, c) =>
            {
                addInputBox.Error = attribute.Name + (addInputBox.Text == String.Empty ? "不能为空" : "已存在");

                if (addInputBox.Text == String.Empty || list.Contains(addInputBox.Text))
                {
                    addInputBox.ShowError();
                    return;
                }

                //  判断重复
                if (!attribute.IsCanRepeat && SettingData[pi.Name].Contains(addInputBox.Text))
                {
                    addInputBox.ShowError();
                    return;
                }

                //list.Add(addInputBox.Text);
                listControl.Items.Add(addInputBox.Text);
                addInputBox.Text = String.Empty;



            };
            pi.SetValue(configData, list);
            Container.Children.Add(listControl);

            var inputPanel = new Grid();
            inputPanel.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(10, GridUnitType.Star)
                });
            inputPanel.ColumnDefinitions.Add(
               new ColumnDefinition()
               {
                   Width = new GridLength(2, GridUnitType.Star)
               });
            inputPanel.Margin = new Thickness(15, 10, 15, 10);
            Grid.SetColumn(addInputBox, 0);
            Grid.SetColumn(addBtn, 1);
            inputPanel.Children.Add(addInputBox);
            inputPanel.Children.Add(addBtn);
            Container.Children.Add(inputPanel);


        }
        private void RenderTextBox(ConfigAttribute attribute, PropertyInfo pi)
        {
            string value = (string)pi.GetValue(Data);

            if (SettingData.ContainsKey(pi.Name))
            {
                SettingData[pi.Name].Add(value);
            }
            else
            {
                SettingData.Add(pi.Name, new List<string>()
                {
                    value
                });
            }


            var textBox = new InputBox();
            textBox.Text = value;
            textBox.Placeholder = attribute.Name;
            textBox.Width = 125;
            textBox.TextChanged += (e, c) =>
            {
                textBox.Error = attribute.Name + (textBox.Text == String.Empty ? "不能为空" : "已存在");

                if (!attribute.IsCanRepeat)
                {
                    if (SettingData[pi.Name].Contains(textBox.Text) && textBox.Tag.ToString() != textBox.Text)
                    {
                        textBox.ShowError();
                        return;
                    }
                    else
                    {
                        textBox.HideError();
                    }
                }
                if (textBox.Text == String.Empty)
                {
                    textBox.ShowError();
                }
                else
                {
                    textBox.HideError();
                }
            };
            textBox.GotFocus += (e, c) =>
            {
                //  记录获得焦点时的数据
                textBox.Tag = textBox.Text;
            };
            textBox.LostFocus += (e, c) =>
            {
                if (textBox.Text == String.Empty)
                {
                    textBox.Text = textBox.Tag.ToString();
                    return;
                }
                if (!attribute.IsCanRepeat)
                {
                    if (SettingData[pi.Name].Contains(textBox.Text) && textBox.Tag.ToString() != textBox.Text)
                    {
                        textBox.Text = textBox.Tag.ToString();
                        return;
                    }
                }
                if (attribute.IsName)
                {
                    Title = textBox.Text;
                }
                pi.SetValue(configData, textBox.Text);
                Data = configData;
                DataChanged?.Invoke(this, EventArgs.Empty);

                SettingData[pi.Name].Remove(textBox.Tag.ToString());
                SettingData[pi.Name].Add(textBox.Text);

            };
            var item = new SettingPanelItem();
            item.Name = LocalizationServicer.Instance.Translated(attribute.Name);
            item.Description = LocalizationServicer.Instance.Translated(attribute.Description);
            item.Content = textBox;
            pi.SetValue(configData, textBox.Text);
            if (attribute.IsName)
            {
                Title = textBox.Text;
            }
            Container.Children.Add(item);
        }
    }
}
