using Core.Librarys;
using Core.Models.Config;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Controls.Button;
using UI.Controls.Input;
using UI.Controls.List;

namespace UI.Controls.SettingPanel
{

    public class SettingPanel : Control
    {
        public struct Config
        {
            public ConfigAttribute Attribute { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }
        public object Data { get { return (object)GetValue(DataProperty); } set { SetValue(DataProperty, value); } }
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(SettingPanel), new PropertyMetadata(new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SettingPanel;
            if (e.NewValue != e.OldValue)
            {
                control.Render();
            }
        }


        private StackPanel Container;
        private object configData;
        private readonly string nosetGroupKey = "noset_group";
        private Dictionary<string, List<Config>> configList;
        public Dictionary<string, List<string>> SettingData { get; set; }
        public SettingPanel()
        {
            DefaultStyleKey = typeof(SettingPanel);
            SettingData = new Dictionary<string, List<string>>();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Container = GetTemplateChild("Container") as StackPanel;

            Render();
        }
        private void Render()
        {
            configList = new Dictionary<string, List<Config>>();
            configList.Add(nosetGroupKey, new List<Config>());

            if (Container == null)
            {
                return;
            }

            Container.Children.Clear();

            if (Data == null)
            {
                return;
            }



            if (Data is IEnumerable)
            {
                //  多例
                RenderMultiPanel();
            }
            else
            {
                //  单例
                RenderSinglePanel();
            }


        }


        #region 渲染多例面板
        /// <summary>
        /// 渲染多例面板
        /// </summary>

        private void RenderMultiPanel()
        {
            var dataList = Data as IEnumerable<object>;
            var list = dataList.ToList();
            var cacheList = new Dictionary<int, object>();

            var dataType = Data.GetType();
            var itemType = dataType.GenericTypeArguments[0];
            var test1 = dataType.GetGenericArguments()[0];

            var emptyData = Activator.CreateInstance(itemType);

            //  添加按钮
            var addBtn = new Button.Button();
            addBtn.Content = "新建项";
            addBtn.Width = 150;
            addBtn.Icon = Base.IconTypes.CalculatorAddition;
            addBtn.Click += (s, e) =>
            {
                int id = cacheList.Keys.Count > 0 ? cacheList.Keys.Max() + 1 : 1;

                cacheList.Add(id, emptyData);

                var panel = CreateMultiPanel(emptyData, id, cacheList);

                Container.Children.Insert(Container.Children.Count - 1, panel);

            };
            Container.Children.Add(addBtn);


            //  渲染数据
            for (int i = 0; i < list.Count; i++)
            {
                int id = cacheList.Keys.Count > 0 ? cacheList.Keys.Max() + 1 : 1;

                cacheList.Add(id, list[i]);
                var panel = CreateMultiPanel(list[i], id, cacheList);
                Container.Children.Insert(Container.Children.Count - 1, panel);
            }



        }


        private SettingPanelMultiItem CreateMultiPanel(object data, int id, Dictionary<int, object> cacheList)
        {
            var panel = new SettingPanelMultiItem();
            panel.Data = data;
            panel.Tag = id;
            panel.SettingData = SettingData;
            panel.DataChanged += (e, c) =>
            {
                int pid = (int)panel.Tag;

                if (cacheList.ContainsKey(pid))
                {
                    //  更新
                    cacheList[pid] = panel.Data;

                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(panel.Data.GetType());

                    var newList = (IList)Activator.CreateInstance(constructedListType);

                    foreach (var item in cacheList.Values)
                    {
                        newList.Add(item);
                    }
                    Data = newList;
                }
            };
            panel.OnRemoveAction = new Command(new Action<object>((s) =>
              {
                  cacheList.Remove(id);
                  var listType = typeof(List<>);
                  var constructedListType = listType.MakeGenericType(panel.Data.GetType());

                  var newList = (IList)Activator.CreateInstance(constructedListType);
                  foreach (var item in cacheList.Values)
                  {
                      newList.Add(item);
                  }
                  Data = newList;
              }));
            return panel;
        }
        #endregion

        /// <summary>
        /// 渲染单例面板
        /// </summary>
        private void RenderSinglePanel()
        {
            configData = Activator.CreateInstance(Data.GetType());

            // 遍历方法特性
            foreach (PropertyInfo pi in Data.GetType().GetProperties())
            {
                foreach (Attribute attr in pi.GetCustomAttributes(true))
                {
                    if (attr is ConfigAttribute)
                    {
                        ConfigAttribute attribute = (ConfigAttribute)attr;

                        var config = new Config()
                        {
                            Attribute = attribute,
                            PropertyInfo = pi
                        };
                        if (string.IsNullOrEmpty(attribute.Group))
                        {
                            //  未分组
                            configList[nosetGroupKey].Add(config);
                        }
                        else
                        {
                            if (!configList.ContainsKey(attribute.Group))
                            {
                                configList.Add(attribute.Group, new List<Config>());
                            }
                            configList[attribute.Group].Add(config);
                        }


                    }
                }
            }

            //  渲染已分组的项目
            foreach (var item in configList)
            {
                if (item.Key != nosetGroupKey)
                {
                    RenderGroup(item.Value, item.Key);
                }
            }

            //  渲染未分组项目
            RenderGroup(configList[nosetGroupKey]);
        }
        private void RenderGroup(List<Config> configList, string groupName = null)
        {
            if (configList == null || configList.Count == 0)
            {
                return;
            }
            var itemContainer = new StackPanel();

            var sortList = configList.OrderBy(m => m.Attribute.Index).ToList();
            for (int i = 0; i < sortList.Count; i++)
            {
                var config = sortList[i];

                var control = RenderConfigItem(config.Attribute, config.PropertyInfo);
                if (control != null)
                {
                    itemContainer.Children.Add(control);

                    if (i != sortList.Count - 1)
                    {
                        //  添加分割线
                        var spliteLine = new Border();
                        spliteLine.Height = 1;
                        spliteLine.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ededed"));
                        itemContainer.Children.Add(spliteLine);
                    }
                }


            }

            var groupControl = GetCreateGroupContainer(itemContainer, groupName);
            Container.Children.Add(groupControl);
        }

        private StackPanel GetCreateGroupContainer(UIElement item, string groupName = null)
        {
            var container = new StackPanel();
            container.Margin = new Thickness(0, 10, 0, 20);
            if (groupName != null)
            {
                var groupNameControl = new TextBlock();
                groupNameControl.Text = groupName;
                groupNameControl.FontSize = 14;
                groupNameControl.Margin = new Thickness(0, 0, 0, 10);
                container.Children.Add(groupNameControl);
            }

            var border = new Border();
            border.Background = new SolidColorBrush(Colors.White);
            border.CornerRadius = new CornerRadius(6);
            border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ededed"));
            border.BorderThickness = new Thickness(1);
            border.Child = item;

            container.Children.Add(border);

            return container;
        }


        private UIElement RenderConfigItem(ConfigAttribute attribute, PropertyInfo pi)
        {
            UIElement uIElement = null;
            if (pi.PropertyType == typeof(bool))
            {
                uIElement = RenderBooleanConfigControl(attribute, pi);
            }
            else if (pi.PropertyType == typeof(List<string>))
            {
                uIElement = RenderListStringConfigControl(attribute, pi);
            }

            return uIElement;
        }

        private UIElement RenderBooleanConfigControl(ConfigAttribute configAttribute, PropertyInfo pi)
        {
            var inputControl = new Toggle.Toggle();
            inputControl.ToggleChanged += (e, c) =>
            {
                pi.SetValue(configData, inputControl.IsChecked);

                Data = configData;
            };

            inputControl.IsChecked = (bool)pi.GetValue(Data);

            var item = new SettingPanelItem();
            item.Name = configAttribute.Name;
            item.Description = configAttribute.Description;
            item.Content = inputControl;

            return item;
        }

        private UIElement RenderListStringConfigControl(ConfigAttribute configAttribute, PropertyInfo pi)
        {
            var list = pi.GetValue(Data) as List<string>;

            var listControl = new BaseList();
            listControl.MaxHeight = 200;
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
                };
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


            //  添加输入框
            var addInputBox = new InputBox();
            addInputBox.Placeholder = configAttribute.Placeholder;
            addInputBox.Margin = new Thickness(0, 0, 10, 0);


            //添加
            var addBtn = new Button.Button();
            //addBtn.Margin = new Thickness(15, 0, 15, 10);
            addBtn.Content = "添加";

            addBtn.Click += (e, c) =>
            {
                if (addInputBox.Text == String.Empty || list.Contains(addInputBox.Text))
                {
                    addInputBox.Error = configAttribute.Name + (addInputBox.Text == String.Empty ? "不能为空" : "已存在");
                    addInputBox.ShowError();
                    return;
                }
                //list.Add(addInputBox.Text);
                listControl.Items.Add(addInputBox.Text);
                addInputBox.Text = String.Empty;



            };
            pi.SetValue(configData, list);

            IconButton moreActionBtn = new IconButton();
            moreActionBtn.VerticalAlignment = VerticalAlignment.Center;
            moreActionBtn.HorizontalAlignment = HorizontalAlignment.Right;
            moreActionBtn.Margin = new Thickness(0, 5, 5, 0);
            moreActionBtn.Icon = Base.IconTypes.More;

            var moreActionMenu = new ContextMenu();

            moreActionBtn.MouseLeftButtonUp += (e, c) =>
            {
                moreActionMenu.IsOpen = true;
            };
            bool isHasMoreAction = false;
            if (configAttribute.IsCanImportExport)
            {
                //  允许导入导出
                isHasMoreAction = true;

                //  导入操作
                var importMenuItem = new MenuItem();
                importMenuItem.Header = "导入";
                importMenuItem.Click += (e, c) =>
                {
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.Title = "选择文件";
                    ofd.Filter = "json(*.json)|*.json";
                    ofd.FileName = configAttribute.Name;

                    bool? result = ofd.ShowDialog();
                    if (result == true)
                    {
                        try
                        {
                            List<string> data = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(ofd.FileName));
                            if (data == null)
                            {
                                MessageBox.Show("文件格式有误或者数据为空，请选择有效的导出文件。");
                            }
                            else
                            {
                                if (MessageBox.Show("导入将覆盖现有配置，确定吗？", "注意", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    pi.SetValue(configData, data);
                                    listControl.Items.Clear();
                                    foreach (string item in data)
                                    {
                                        listControl.Items.Add(item);
                                    }
                                    MessageBox.Show("导入完成！", "提示");
                                }
                            }
                        }catch (Exception ex)
                        {
                            Logger.Error($"导入配置“{configAttribute.Name}”时失败：{ex.Message}");
                            MessageBox.Show("导入失败！", "提示");
                        }
                    }

                };

                //  导出操作
                var exportMenuItem = new MenuItem();
                exportMenuItem.Header = "导出";
                exportMenuItem.Click += (e, c) =>
                {
                    try
                    {
                        Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                        sfd.Title = "选择文件";
                        sfd.Filter = "json(*.json)|*.json";
                        sfd.FileName = configAttribute.Name + "导出配置";

                        bool? result = sfd.ShowDialog();
                        if (result == true)
                        {
                            File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(listControl.Items));
                            MessageBox.Show("导出完成", "提示");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"导出配置“{configAttribute.Name}”时失败：{ex.Message}");
                        MessageBox.Show("导出失败！", "提示");
                    }

                };


                moreActionMenu.Items.Add(importMenuItem);
                moreActionMenu.Items.Add(exportMenuItem);
            }


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

            //  标题和说明

            var description = new TextBlock();
            description.Text = configAttribute.Description;
            description.Margin = new Thickness(10, 10, 10, 0);
            description.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#989CA1"));
            var container = new StackPanel();

            var head = new Grid();
            head.ColumnDefinitions.Add(
                 new ColumnDefinition()
                 {
                     Width = new GridLength(10, GridUnitType.Star)
                 });
            head.ColumnDefinitions.Add(
               new ColumnDefinition()
               {
                   Width = new GridLength(2, GridUnitType.Star)
               });
            Grid.SetColumn(description, 0);
            head.Children.Add(description);

            //  更多操作按钮
            if (isHasMoreAction)
            {
                Grid.SetColumn(moreActionBtn, 1);
                head.Children.Add(moreActionBtn);
            }

            container.Children.Add(head);
            container.Children.Add(inputPanel);
            container.Children.Add(listControl);
            return container;
        }
    }
}
