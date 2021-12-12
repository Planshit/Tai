using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UI.Controls.Base;
using UI.Controls.Charts.Model;
using UI.Extensions;

namespace UI.Controls.Charts
{
    public class Charts : Control
    {
        #region ChartsType 样式类型
        /// <summary>
        /// 样式类型
        /// </summary>
        public ChartsType ChartsType
        {
            get { return (ChartsType)GetValue(ChartsTypeProperty); }
            set { SetValue(ChartsTypeProperty, value); }
        }
        public static readonly DependencyProperty ChartsTypeProperty =
            DependencyProperty.Register("ChartsType",
                typeof(ChartsType),
                typeof(Charts));


        #endregion
        #region MaxValueLimit 限制最大值
        /// <summary>
        /// 样式类型
        /// </summary>
        public double MaxValueLimit
        {
            get { return (double)GetValue(MaxValueLimitProperty); }
            set { SetValue(MaxValueLimitProperty, value); }
        }
        public static readonly DependencyProperty MaxValueLimitProperty =
            DependencyProperty.Register("MaxValueLimit",
                typeof(double),
                typeof(Charts),
                new PropertyMetadata((double)0));


        #endregion
        #region ShowLimit 显示条数限制
        /// <summary>
        /// 样式类型
        /// </summary>
        public int ShowLimit
        {
            get { return (int)GetValue(ShowLimitProperty); }
            set { SetValue(ShowLimitProperty, value); }
        }
        public static readonly DependencyProperty ShowLimitProperty =
            DependencyProperty.Register("ShowLimit",
                typeof(int),
                typeof(Charts),
                new PropertyMetadata((int)0));


        #endregion
        #region Data 数据
        /// <summary>
        /// 样式类型
        /// </summary>
        public IEnumerable<ChartsDataModel> Data
        {
            get { return (IEnumerable<ChartsDataModel>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                typeof(IEnumerable<ChartsDataModel>),
                typeof(Charts),
                new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged))
                );


        #endregion
        #region LoadingPlaceholderCount 加载中时占位显示条数
        /// <summary>
        /// 加载中时占位显示条数
        /// </summary>
        public int LoadingPlaceholderCount
        {
            get { return (int)GetValue(LoadingPlaceholderCountProperty); }
            set { SetValue(LoadingPlaceholderCountProperty, value); }
        }
        public static readonly DependencyProperty LoadingPlaceholderCountProperty =
            DependencyProperty.Register("LoadingPlaceholderCount",
                typeof(int),
                typeof(Charts),
                new PropertyMetadata((int)3));


        #endregion
        #region IsLoading 是否在加载中
        /// <summary>
        /// 是否在加载中
        /// </summary>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));


        #endregion
        #region ClickCommand 点击命令
        /// <summary>
        /// 点击命令
        /// </summary>
        public Command ClickCommand
        {
            get { return (Command)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }
        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand",
                typeof(Command),
                typeof(Charts));


        #endregion
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var charts = (d as Charts);
            if (e.Property == DataProperty)
            {
                var dc = charts.ChartsType;
                charts.Render();
            }
            if (e.Property == IsLoadingProperty)
            {
                if (!charts.IsLoading)
                {
                    charts.Render();
                }
                else
                {
                    charts.RenderLoadingPlaceholder();
                }
            }

        }
        /// <summary>
        /// 点击项目时发生
        /// </summary>
        public event EventHandler OnItemClick;

        /// <summary>
        /// 容器
        /// </summary>
        private StackPanel Container;
        private WrapPanel CardContainer;
        private Grid MonthContainer;

        /// <summary>
        /// 是否在渲染中
        /// </summary>
        private bool isRendering = false;
        /// <summary>
        /// 计算最大值
        /// </summary>
        private double maxValue = 0;
        public Charts()
        {
            DefaultStyleKey = typeof(Charts);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Container = GetTemplateChild("Container") as StackPanel;
            CardContainer = GetTemplateChild("CardContainer") as WrapPanel;
            MonthContainer = GetTemplateChild("MonthContainer") as Grid;

            Loaded += Charts_Loaded;
        }

        private void Charts_Loaded(object sender, RoutedEventArgs e)
        {
            if (Container != null)
            {
                if (IsLoading)
                {
                    RenderLoadingPlaceholder();
                }
                else
                {
                    Render();
                }
            }
        }

        private void RenderLoadingPlaceholder()
        {
            if (Container != null && LoadingPlaceholderCount > 0)
            {
                Container.Children.Clear();
                CardContainer.Children.Clear();

                for (int i = 0; i < LoadingPlaceholderCount; i++)
                {
                    switch (ChartsType)
                    {
                        case ChartsType.HorizontalA:
                            var item = new ChartsItemTypeA();
                            item.IsLoading = true;
                            Container.Children.Add(item);
                            break;
                        case ChartsType.Card:
                            var card = new ChartsItemTypeCard();
                            card.IsLoading = true;
                            CardContainer.Children.Add(card);
                            break;
                    }
                }

            }
        }
        private void Calculate()
        {
            if (Data == null)
            {
                return;
            }
            //  计算最大值
            //  如果设置了固定的最大值则使用，否则查找数据中的最大值
            maxValue = MaxValueLimit > 0 ? MaxValueLimit : Data.Count() > 0 ? Data.Max(m => m.Value) : 0;

            if (ChartsType == ChartsType.HorizontalA)
            {
                //不允许最大值小于10，否则效果不好看
                if (maxValue < 10)
                {
                    maxValue = 10;
                }
                //  适当增加最大值
                maxValue = Math.Round(maxValue / 2, MidpointRounding.AwayFromZero) * 2 + 2;
            }
        }

        private void Render()
        {
            if (isRendering || Container == null || CardContainer == null)
            {
                return;
            }
            Calculate();
            Container.Children.Clear();
            CardContainer.Children.Clear();
            MonthContainer.Children.Clear();

            if (Data == null || Data.Count() <= 0)
            {
                Container.Children.Add(new EmptyData());
                CardContainer.Children.Add(new EmptyData());
                return;
            }

            isRendering = true;

            switch (ChartsType)
            {
                case ChartsType.HorizontalA:
                    RenderHorizontalAStyle();
                    break;
                case ChartsType.Card:
                    RenderCardStyle();
                    break;
                case ChartsType.Month:
                    RenderMonthStyle();
                    break;
            }
        }

        #region 渲染横向样式A
        private void RenderHorizontalAStyle()
        {
           
            var data = Data.OrderByDescending(x => x.Value).ToList();

            foreach (var item in data)
            {
                var chartsItem = new ChartsItemTypeA();
                chartsItem.Data = item;
                chartsItem.ToolTip = item.PopupText;
                chartsItem.MaxValue = maxValue;

                //  处理点击事件
                HandleItemClick(chartsItem, item);

                Container.Children.Add(chartsItem);
                if (ShowLimit > 0 && Container.Children.Count == ShowLimit)
                {
                    break;
                }
            }
            isRendering = false;
        }
        #endregion
        #region 渲染卡片样式Card
        private void RenderCardStyle()
        {
            var data = Data.OrderByDescending(x => x.Value).Take(ShowLimit).ToList();

            data.Shuffle();

            foreach (var item in data)
            {
                var chartsItem = new ChartsItemTypeCard();
                chartsItem.Data = item;

                //  处理点击事件
                HandleItemClick(chartsItem, item);

                chartsItem.MaxValue = maxValue;
                chartsItem.ToolTip = item.PopupText;
                CardContainer.Children.Add(chartsItem);
            }
            isRendering = false;

        }
        #endregion

        #region 渲染月份样式
        private void RenderMonthStyle()
        {
            var data = Data.ToList();
            DateTime month = data[0].DateTime;

            int days = DateTime.DaysInMonth(month.Year, month.Month);

            //  绘制网格
            var headerGrid = new Grid();
            headerGrid.Margin = new Thickness(0, 0, 0, 10);

            var dataGrid = new Grid();

            string[] week = { "一", "二", "三", "四", "五", "六", "日" };
            for (int i = 0; i < 7; i++)
            {

                dataGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                //  填充头部
                var header = new TextBlock();
                header.Text = week[i];
                header.VerticalAlignment = VerticalAlignment.Center;
                header.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetColumn(header, i);
                headerGrid.Children.Add(header);
            }

            for (int i = 0; i < 6; i++)
            {
                dataGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            //  填充空数据
            for (int i = 0; i < days; i++)
            {
                var date = new DateTime(month.Year, month.Month, i + 1);
                var chartsItem = new ChartsItemTypeMonth();
                chartsItem.Data = new ChartsDataModel()
                {
                    DateTime = date,
                };
                var location = CalGridLocation(date);
                Grid.SetColumn(chartsItem, location[0]);
                Grid.SetRow(chartsItem, location[1]);
                dataGrid.Children.Add(chartsItem);
                Debug.WriteLine((i + 1) + " -> " + location[0] + "," + location[1]);
            }

            //  填充有效数据
            foreach (var item in data)
            {
                if (item.DateTime != null)
                {
                    int day = item.DateTime.Day;

                    Debug.WriteLine("日期：" + day + "号");
                    var chartsItem = new ChartsItemTypeMonth();
                    chartsItem.Data = item;
                    chartsItem.ToolTip = item.PopupText;
                    chartsItem.MaxValue = maxValue;

                    //  处理点击事件
                    HandleItemClick(chartsItem, item);

                    var location = CalGridLocation(item.DateTime);

                    Grid.SetColumn(chartsItem, location[0]);
                    Grid.SetRow(chartsItem, location[1]);
                    dataGrid.Children.Add(chartsItem);
                    if (ShowLimit > 0 && Container.Children.Count == ShowLimit)
                    {
                        break;
                    }
                }
            }

            var sp = new StackPanel();
            sp.Children.Add(headerGrid);
            sp.Children.Add(dataGrid);

            MonthContainer.Children.Add(sp);
            isRendering = false;
        }

        private int[] CalGridLocation(DateTime date)
        {
            var res = new int[2];
            //  判断1号是星期几
            int firstDayWeekNum = (int)new DateTime(date.Year, date.Month, 1).DayOfWeek;
            if (firstDayWeekNum == 0)
            {
                firstDayWeekNum = 7;
            }

            int col = 0;
            int row = 0;
            if (firstDayWeekNum == 1)
            {
                col = date.Day - 1;
            }
            else
            {
                col = date.Day + firstDayWeekNum - 2;
            }
            if (col > 6)
            {
                row = (int)col / 7;

                int dayWeekNum = (int)new DateTime(date.Year, date.Month, date.Day).DayOfWeek;
                if (dayWeekNum == 0)
                {
                    dayWeekNum = 7;
                }
                col = dayWeekNum - 1;


            }

            res[0] = col;
            res[1] = row;
            return res;
        }
        #endregion


        private void HandleItemClick(UIElement el, ChartsDataModel data)
        {
            el.MouseLeftButtonUp += (s, e) =>
            {
                OnItemClick?.Invoke(data, null);
                ClickCommand?.Execute(data);
            };
        }
    }
}
