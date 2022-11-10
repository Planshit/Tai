using Core.Librarys;
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
using System.Windows.Threading;
using UI.Controls.Base;
using UI.Controls.Charts.Model;
using UI.Controls.Input;
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

        #region IsCanScroll 是否可以滚动
        /// <summary>
        /// 是否在加载中
        /// </summary>
        public bool IsCanScroll
        {
            get { return (bool)GetValue(IsCanScrollProperty); }
            set { SetValue(IsCanScrollProperty, value); }
        }
        public static readonly DependencyProperty IsCanScrollProperty =
            DependencyProperty.Register("IsCanScroll",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));


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
        #region 柱状图信息数据
        /// <summary>
        /// 柱状图信息数据
        /// </summary>
        public List<ChartColumnInfoModel> ColumnInfoList
        {
            get { return (List<ChartColumnInfoModel>)GetValue(ColumnInfoListProperty); }
            set { SetValue(ColumnInfoListProperty, value); }
        }
        public static readonly DependencyProperty ColumnInfoListProperty =
            DependencyProperty.Register("ColumnInfoList",
                typeof(List<ChartColumnInfoModel>),
                typeof(Charts)
                );


        #endregion
        #region 中间值
        /// <summary>
        /// 中间值
        /// </summary>
        public string Median
        {
            get { return (string)GetValue(MedianProperty); }
            set { SetValue(MedianProperty, value); }
        }
        public static readonly DependencyProperty MedianProperty =
            DependencyProperty.Register("Median",
                typeof(string),
                typeof(Charts)
                );


        #endregion
        #region 最大值
        /// <summary>
        /// 最大值
        /// </summary>
        public string Maximum
        {
            get { return (string)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum",
                typeof(string),
                typeof(Charts)
                );


        #endregion
        #region 单位
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit",
                typeof(string),
                typeof(Charts)
                );


        #endregion
        #region 总计
        /// <summary>
        /// 总计
        /// </summary>
        public string Total
        {
            get { return (string)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total",
                typeof(string),
                typeof(Charts)
                );


        #endregion

        #region 数据值类型
        /// <summary>
        /// 数据值类型
        /// </summary>
        public ChartDataValueType DataValueType
        {
            get { return (ChartDataValueType)GetValue(DataValueTypeProperty); }
            set { SetValue(DataValueTypeProperty, value); }
        }
        public static readonly DependencyProperty DataValueTypeProperty =
            DependencyProperty.Register("DataValueType",
                typeof(ChartDataValueType),
                typeof(Charts)
                );


        #endregion
        #region 

        public int NameIndexStart
        {
            get { return (int)GetValue(NameIndexStartProperty); }
            set { SetValue(NameIndexStartProperty, value); }
        }
        public static readonly DependencyProperty NameIndexStartProperty =
            DependencyProperty.Register("NameIndexStart",
                typeof(int),
                typeof(Charts), new PropertyMetadata((int)0)
                );


        #endregion
        #region 是否空数据
        /// <summary>
        /// 是否空数据
        /// </summary>
        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            set { SetValue(IsEmptyProperty, value); }
        }
        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.Register("IsEmpty",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));


        #endregion
        #region 是否显示总计值（仅柱状图有效，默认显示）
        /// <summary>
        /// 是否显示总计值（仅柱状图有效，默认显示）
        /// </summary>
        public bool IsShowTotal
        {
            get { return (bool)GetValue(IsShowTotalProperty); }
            set { SetValue(IsShowTotalProperty, value); }
        }
        public static readonly DependencyProperty IsShowTotalProperty =
            DependencyProperty.Register("IsShowTotal",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(true));


        #endregion
        #region 是否允许选择项(仅柱状图有效,默认不允许)
        /// <summary>
        /// 是否允许选择项(仅柱状图有效,默认不允许)
        /// </summary>
        public bool IsCanColumnSelect
        {
            get { return (bool)GetValue(IsCanColumnSelectProperty); }
            set { SetValue(IsCanColumnSelectProperty, value); }
        }
        public static readonly DependencyProperty IsCanColumnSelectProperty =
            DependencyProperty.Register("IsCanColumnSelect",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(false));


        #endregion
        #region 是否显示搜索(仅列表样式有效,默认不显示)
        /// <summary>
        /// 是否显示搜索(仅列表样式有效,默认不显示)
        /// </summary>
        public bool IsSearch
        {
            get { return (bool)GetValue(IsSearchProperty); }
            set { SetValue(IsSearchProperty, value); }
        }
        public static readonly DependencyProperty IsSearchProperty =
            DependencyProperty.Register("IsSearch",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(false));


        #endregion
        #region 选中列索引（仅柱状图有效）
        /// <summary>
        /// 选中列索引（仅柱状图有效）
        /// </summary>
        public int ColumnSelectedIndex
        {
            get { return (int)GetValue(ColumnSelectedIndexProperty); }
            set { SetValue(ColumnSelectedIndexProperty, value); }
        }
        public static readonly DependencyProperty ColumnSelectedIndexProperty =
            DependencyProperty.Register("ColumnSelectedIndex",
                typeof(int),
                typeof(Charts), new PropertyMetadata(-1, new PropertyChangedCallback(OnPropertyChanged)));


        #endregion
        #region IsShowBadge 是否显示徽章
        /// <summary>
        /// 是否显示徽章(默认不显示)
        /// </summary>
        public bool IsShowBadge
        {
            get { return (bool)GetValue(IsShowBadgeProperty); }
            set { SetValue(IsShowBadgeProperty, value); }
        }
        public static readonly DependencyProperty IsShowBadgeProperty =
            DependencyProperty.Register("IsShowBadge",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(false));


        #endregion
        public bool IsShowValuesPopup
        {
            get { return (bool)GetValue(IsShowValuesPopupProperty); }
            set { SetValue(IsShowValuesPopupProperty, value); }
        }
        public static readonly DependencyProperty IsShowValuesPopupProperty =
            DependencyProperty.Register("IsShowValuesPopup",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(false));
        public FrameworkElement ValuesPopupPlacementTarget
        {
            get { return (FrameworkElement)GetValue(ValuesPopupPlacementTargetProperty); }
            set { SetValue(ValuesPopupPlacementTargetProperty, value); }
        }
        public static readonly DependencyProperty ValuesPopupPlacementTargetProperty =
            DependencyProperty.Register("ValuesPopupPlacementTarget",
                typeof(FrameworkElement),
                typeof(Charts));

        public List<ChartColumnInfoModel> ColumnValuesInfoList
        {
            get { return (List<ChartColumnInfoModel>)GetValue(ColumnValuesInfoListProperty); }
            set { SetValue(ColumnValuesInfoListProperty, value); }
        }
        public static readonly DependencyProperty ColumnValuesInfoListProperty =
            DependencyProperty.Register("ColumnValuesInfoList",
                typeof(List<ChartColumnInfoModel>),
                typeof(Charts)
                );

        public double ValuesPopupHorizontalOffset
        {
            get { return (double)GetValue(ValuesPopupHorizontalOffsetProperty); }
            set { SetValue(ValuesPopupHorizontalOffsetProperty, value); }
        }
        public static readonly DependencyProperty ValuesPopupHorizontalOffsetProperty =
            DependencyProperty.Register("ValuesPopupHorizontalOffset",
                typeof(double),
                typeof(Charts));

        /// <summary>
        /// 数据最大值
        /// </summary>
        public double DataMaximum
        {
            get { return (double)GetValue(DataMaximumProperty); }
            set { SetValue(DataMaximumProperty, value); }
        }
        public static readonly DependencyProperty DataMaximumProperty =
            DependencyProperty.Register("DataMaximum",
                typeof(double),
                typeof(Charts),
                new PropertyMetadata((double)0)
                );


        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var charts = (d as Charts);
            if (e.Property == DataProperty && e.OldValue != e.NewValue)
            {
                charts.Render();
            }
            if (e.Property == IsLoadingProperty)
            {
                if (!charts.IsLoading || e.Property == IsCanScrollProperty)
                {
                    charts.Render();
                }
                else
                {
                    charts.RenderLoadingPlaceholder();
                }
            }
            if (e.Property == ColumnSelectedIndexProperty && e.OldValue != e.NewValue)
            {
                charts.CheckColumnItem((int)e.OldValue, (int)e.NewValue);
            }

        }

        #region 右键菜单
        public ContextMenu ItemMenu
        {
            get { return (ContextMenu)GetValue(ItemMenuProperty); }
            set { SetValue(ItemMenuProperty, value); }
        }
        public static readonly DependencyProperty ItemMenuProperty =
            DependencyProperty.Register("ItemMenu",
                typeof(ContextMenu),
                typeof(Charts));
        #endregion
        /// <summary>
        /// 点击项目时发生
        /// </summary>
        public event EventHandler OnItemClick;

        /// <summary>
        /// 容器
        /// </summary>
        private StackPanel Container;
        private StackPanel NoScrollContainer;
        private WrapPanel CardContainer;
        private Grid MonthContainer;
        private ScrollViewer ScrollViewer;
        private Grid ColumnContainer;
        private Border RadarContainer;
        /// <summary>
        /// 是否在渲染中
        /// </summary>
        private bool isRendering = false;
        /// <summary>
        /// 计算最大值
        /// </summary>
        private double maxValue = 0;
        /// <summary>
        /// 是否启用懒加载
        /// </summary>
        private bool isLazyloading = false;
        /// <summary>
        /// 项目真实高度
        /// </summary>
        private double itemHeight = 0;
        /// <summary>
        /// 懒加载完成条数
        /// </summary>
        private int lazyloadedCount = 0;
        private List<ChartsDataModel> lazyloadingData;

        /// <summary>
        /// 搜索关键字（仅列表样式有效
        /// </summary>
        private string searchKey;
        public Charts()
        {
            DefaultStyleKey = typeof(Charts);

            SizeChanged += Charts_SizeChanged;
            Unloaded += Charts_Unloaded;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            ScrollViewer.Height = 1;
        }
        private void Charts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ChartsType == ChartsType.HorizontalA)
            {
                var header = GetTemplateChild("AHeader") as Grid;
                var h = ActualHeight - header.ActualHeight;

                ScrollViewer.Height = h;
            }
        }

        private void Charts_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= Charts_Unloaded;
            if (Container != null)
            {
                Container.SizeChanged -= Container_SizeChanged;
            }
            if (ScrollViewer != null)
            {
                ScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Container = GetTemplateChild("Container") as StackPanel;
            CardContainer = GetTemplateChild("CardContainer") as WrapPanel;
            MonthContainer = GetTemplateChild("MonthContainer") as Grid;
            ScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            NoScrollContainer = GetTemplateChild("NoScrollContainer") as StackPanel;
            ColumnContainer = GetTemplateChild("ColumnContainer") as Grid;
            RadarContainer = GetTemplateChild("Radar") as Border;

            if (IsLoading)
            {
                RenderLoadingPlaceholder();
            }
            else
            {
                Render();
            }
        }

        private void RenderLoadingPlaceholder()
        {
            if (Container != null && LoadingPlaceholderCount > 0)
            {
                Container.Children.Clear();
                CardContainer.Children.Clear();
                if (!IsCanScroll)
                {
                    NoScrollContainer.Visibility = Visibility.Visible;
                }
                for (int i = 0; i < LoadingPlaceholderCount; i++)
                {
                    switch (ChartsType)
                    {
                        case ChartsType.HorizontalA:
                            var item = new ChartsItemTypeA();
                            item.IsLoading = true;
                            if (IsCanScroll)
                            {
                                Container.Children.Add(item);
                            }
                            else
                            {
                                NoScrollContainer.Children.Add(item);
                            }
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
            if (Data == null || ChartsType == ChartsType.Column)
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

                var countText = GetTemplateChild("ACount") as Run;
                countText.Text = Data.Count().ToString();
            }
        }

        private void Render()
        {
            if (isRendering || Container == null || CardContainer == null || ColumnContainer == null)
            {
                return;
            }

            if (IsCanScroll)
            {
                ScrollViewer.Visibility = Visibility.Visible;
                NoScrollContainer.Visibility = Visibility.Collapsed;
            }
            else
            {
                ScrollViewer.Visibility = Visibility.Collapsed;
                NoScrollContainer.Visibility = Visibility.Visible;
            }
            Calculate();
            Container.Children.Clear();
            CardContainer.Children.Clear();
            MonthContainer.Children.Clear();
            NoScrollContainer.Children.Clear();
            ColumnContainer.Children.Clear();
            RadarContainer.Child = null;

            if (Data == null || Data.Count() <= 0)
            {
                Container.Children.Add(new EmptyData());
                NoScrollContainer.Children.Add(new EmptyData());
                CardContainer.Children.Add(new EmptyData());
                RadarContainer.Child = new EmptyData();

                IsEmpty = true;
                return;
            }
            else
            {
                IsEmpty = false;
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
                case ChartsType.Column:
                    RenderColumnStyle();
                    break;
                case ChartsType.Radar:
                    RenderLadarStyle();
                    break;
            }

        }

        #region 渲染横向样式A
        private void RenderHorizontalAStyle()
        {
            lazyloadingData = null;
            isLazyloading = false;
            lazyloadedCount = 0;

            ScrollViewer.ScrollToVerticalOffset(0);

            var data = Data.OrderByDescending(x => x.Value).ToList();


            //if (data.Count < 20)
            //{
            //    RenderTypeAItems(data, 0, data.Count);
            //}
            //else
            //{
            //    isLazyloading = true;
            //    RenderTypeAItems(data, 0, 20);
            //    lazyloadingData = data;
            //    ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            //}
            RenderTypeAItems(data, 0, data.Count);

            //Debug.WriteLine("【渲染】" + sw.ElapsedMilliseconds);
            if (IsCanScroll)
            {
                Container.SizeChanged -= Container_SizeChanged;
                Container.SizeChanged += Container_SizeChanged;
            }

            isRendering = false;

            var searchBox = GetTemplateChild("ASearchBox") as InputBox;
            searchBox.TextChanged += SearchBox_TextChanged;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as InputBox;
            if (box.Text != searchKey)
            {
                searchKey = box.Text.ToLower();
                HandleSearch();
            }
        }

        private void HandleSearch()
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var item in Container.Children)
                    {
                        var control = item as ChartsItemTypeA;
                        if (control != null)
                        {
                            var data = control.Data;

                            bool show = false;
                            if (data.Name.ToLower().IndexOf(searchKey) != -1 || data.PopupText.ToLower().IndexOf(searchKey) != -1 || string.IsNullOrEmpty(searchKey))
                            {
                                show = true;
                            }
                            control.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                });

            });
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in Container.Children)
            {
                var control = item as ChartsItemTypeA;
                if (control != null)
                {
                    control.UpdateValueBlockWidth();
                }
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ScrollViewer.VerticalOffset > 0 && e.VerticalChange > 0)
            {
                double a = ScrollViewer.VerticalOffset * itemHeight;
                double b = itemHeight * lazyloadedCount;
                if (b - a <= ScrollViewer.ActualHeight)
                {
                    RenderTypeAItems(lazyloadingData, lazyloadedCount, 20);
                }
            }

            if (lazyloadedCount == Data.Count())
            {
                ScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        private void RenderTypeAItems(List<ChartsDataModel> data, int start, int count)
        {
            StackPanel container = Container;
            if (!IsCanScroll)
            {
                container = NoScrollContainer;
            }
            for (int i = start; i < start + count; i++)
            {

                if (i >= data.Count)
                {
                    break;
                }
                ChartsDataModel item = data[i];
                var chartsItem = new ChartsItemTypeA();
                chartsItem.Data = item;
                chartsItem.ToolTip = item.PopupText;
                chartsItem.MaxValue = maxValue;
                chartsItem.IsShowBadge = IsShowBadge;

                if (i == 19 && isLazyloading)
                {
                    chartsItem.Loaded += ChartsItem_Loaded;
                }
                //  处理点击事件
                HandleItemClick(chartsItem, item);
                //container.Children.Insert(lazyloadedCount, chartsItem);
                container.Children.Add(chartsItem);
                if (ShowLimit > 0 && Container.Children.Count == ShowLimit)
                {
                    break;
                }
                //lazyloadedCount++;
                //if (i >= 20)
                //{
                //    container.Children.RemoveAt(Container.Children.Count - 1);
                //}
            }

        }

        private void ChartsItem_Loaded(object sender, RoutedEventArgs e)
        {
            var chartsItem = (ChartsItemTypeA)sender;
            chartsItem.Loaded -= ChartsItem_Loaded;

            int count = Data.Count() - 20;
            for (int i = 0; i < count; i++)
            {
                var pl = new TextBlock();
                pl.Height = chartsItem.ActualHeight;
                Container.Children.Add(pl);
            }

            itemHeight = chartsItem.ActualHeight;
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

        #region 渲染柱形图
        private void CheckColumnItem(int oldIndex, int newIndex)
        {
            if (!IsCanColumnSelect || ColumnContainer.Children.Count == 0)
            {
                return;
            }

            if (oldIndex >= 0 && oldIndex < ColumnContainer.Children.Count)
            {
                var oldItem = ColumnContainer.Children[oldIndex] as Grid;
                oldItem.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (newIndex >= 0 && newIndex < ColumnContainer.Children.Count)
            {
                var background = Application.Current.Resources["ThemeBrush"] as SolidColorBrush;

                var checkItem = ColumnContainer.Children[newIndex] as Grid;
                checkItem.Background = new SolidColorBrush(background.Color) { Opacity = .1 };
            }
        }
        /// <summary>
        /// 渲染柱形图
        /// </summary>
        private void RenderColumnStyle()
        {
            ColumnContainer.Children.Clear();
            ColumnInfoList = new List<ChartColumnInfoModel>();
            ColumnContainer.ColumnDefinitions.Clear();
            maxValue = 0;

            Maximum = string.Empty;
            Median = string.Empty;
            Total = string.Empty;

            if (Data == null || Data.Count() == 0)
            {
                return;
            }




            double total = 0;
            //  查找最大值
            foreach (var item in Data)
            {
                //  查找最大值
                double max = item.Values.Max();
                if (max > maxValue)
                {
                    maxValue = max;
                }

                total += item.Values.Sum();

            }

            if (DataMaximum > 0)
            {
                maxValue = DataMaximum;
            }

            Maximum = Covervalue(maxValue);

            Median = Covervalue((maxValue / 2));

            Total = Covervalue(total);

            int margin = 5;



            //  创建列
            int columns = Data.FirstOrDefault().Values.Length;


            //  调整间距
            if (columns <= 7)
            {
                margin = 25;
            }
            else if (columns <= 12)
            {
                margin = 15;
            }
            else if (columns >= 20)
            {
                margin = 2;
            }

            int columnCount = Data.Count();

            var list = Data.ToList();

            for (int i = 0; i < columns; i++)
            {
                ColumnContainer.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                var valueContainer = new Grid();

                Grid.SetColumn(valueContainer, i);
                ColumnContainer.Children.Add(valueContainer);

                var valuesPopupList = new List<ChartColumnInfoModel>();

                for (int di = 0; di < columnCount; di++)
                {
                    var item = list[di];
                    item.Color = item.Color == null ? UI.Base.Color.Colors.MainColors[di] : item.Color;
                    var column = new ChartItemTypeColumn();
                    column.Value = item.Values[i];
                    column.MaxValue = maxValue;
                    column.Margin = new Thickness(margin, 0, margin, 0);

                    column.Color = item.Color;
                    column.ColumnName = item.ColumnNames != null && item.ColumnNames.Length > 0 ? item.ColumnNames[i] : (i + NameIndexStart).ToString();
                    Panel.SetZIndex(column, column.Value > 0 ? -(int)column.Value : 0);

                    valueContainer.Children.Add(column);

                    if (column.Value > 0)
                    {
                        valuesPopupList.Add(new ChartColumnInfoModel()
                        {
                            Color = item.Color,
                            Name = item.Name,
                            Icon = item.Icon,
                            Text = Covervalue(column.Value) + Unit
                        });
                    }
                }

                valueContainer.MouseEnter += (e, c) =>
                {
                    ColumnValuesInfoList = valuesPopupList;
                    ValuesPopupPlacementTarget = valueContainer;
                    IsShowValuesPopup = valuesPopupList.Count > 0;

                    ValuesPopupHorizontalOffset = -17.5 + (valueContainer.ActualWidth / 2);
                };
                valueContainer.MouseLeave += (e, c) =>
                {
                    IsShowValuesPopup = false;
                };

                var index = i;
                valueContainer.MouseLeftButtonDown += (e, c) =>
                {
                    ColumnSelectedIndex = index;
                };
            }

            var infoList = new List<ChartColumnInfoModel>();

            list = list.OrderByDescending(m => m.Values.Sum()).ToList();

            foreach (var item in list)
            {
                infoList.Add(new ChartColumnInfoModel()
                {
                    Color = item.Color,
                    Name = item.Name,
                    Icon = item.Icon,
                    Text = Covervalue(item.Values.Sum()) + Unit
                });
            }

            ColumnInfoList = infoList;

            isRendering = false;
        }
        #endregion

        #region 渲染雷达图
        private void RenderLadarStyle()
        {
            if (Data.Count() <= 2)
            {
                RadarContainer.Child = new EmptyData();
                return;
            }
            //  最大值
            double total = 0;
            //  查找最大值
            foreach (var item in Data)
            {
                //  查找最大值
                double max = item.Values.Sum();
                if (max > maxValue)
                {
                    maxValue = max;
                }

                total += item.Values.Sum();

            }
            //maxValue = total;

            if (DataMaximum > 0)
            {
                maxValue = DataMaximum;
            }
            var radar = new ChartsItemTypeRadar();
            radar.Data = Data.OrderBy(m => m.Values.Sum()).ToList();
            radar.MaxValue = maxValue;
            RadarContainer.Child = radar;

            isRendering = false;
        }
        #endregion

        private string Covervalue(double value)
        {
            if (DataValueType == ChartDataValueType.Seconds)
            {
                return Time.ToString((int)value);
            }
            else
            {
                return value.ToString();
            }
        }

        private void HandleItemClick(UIElement el, ChartsDataModel data)
        {
            el.MouseLeftButtonUp += (s, e) =>
            {
                OnItemClick?.Invoke(data, null);
                ClickCommand?.Execute(data);
            };
            el.MouseRightButtonUp += (s, e) =>
            {
                if (ItemMenu != null)
                {
                    ItemMenu.IsOpen = true;
                    ItemMenu.Tag = data;
                }
            };
        }
    }
}
