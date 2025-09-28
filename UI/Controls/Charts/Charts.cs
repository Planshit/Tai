using Core.Librarys;
using Core.Models;
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
using UI.Base;
using UI.Controls.Base;
using UI.Controls.Charts.Model;
using UI.Controls.Input;
using UI.Extensions;
using UI.Models;
using UI.Servicers;

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

        public IEnumerable<ChartsDataModel> ListViewBindingData
        {
            get { return (IEnumerable<ChartsDataModel>)GetValue(ListViewBindingDataProperty); }
            set { SetValue(ListViewBindingDataProperty, value); }
        }
        public static readonly DependencyProperty ListViewBindingDataProperty =
            DependencyProperty.Register("ListViewBindingData",
                typeof(IEnumerable<ChartsDataModel>),
                typeof(Charts));
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
        public double DataMaxValue
        {
            get { return (double)GetValue(DataMaxValueProperty); }
            set { SetValue(DataMaxValueProperty, value); }
        }
        public static readonly DependencyProperty DataMaxValueProperty =
            DependencyProperty.Register("DataMaxValue",
                typeof(double),
                typeof(Charts),
                new PropertyMetadata((double)0)
                );

        #region 是否以堆叠的形式展示（仅柱状图有效）
        /// <summary>
        /// 是否以堆叠的形式展示（仅柱状图有效）
        /// </summary>
        public bool IsStack
        {
            get { return (bool)GetValue(IsStackProperty); }
            set { SetValue(IsStackProperty, value); }
        }
        public static readonly DependencyProperty IsStackProperty =
            DependencyProperty.Register("IsStack",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));


        #endregion
        #region 是否显示分类信息
        /// <summary>
        /// 是否显示分类信息（仅柱状图有效）
        /// </summary>
        public bool IsShowCategory
        {
            get { return (bool)GetValue(IsShowCategoryProperty); }
            set { SetValue(IsShowCategoryProperty, value); }
        }
        public static readonly DependencyProperty IsShowCategoryProperty =
            DependencyProperty.Register("IsShowCategory",
                typeof(bool),
                typeof(Charts),
                new PropertyMetadata(true, new PropertyChangedCallback(OnPropertyChanged)));


        #endregion
        #region 图标大小（仅列表样式有效）
        /// <summary>
        /// 图标大小（仅列表样式有效）
        /// </summary>
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize",
                typeof(double),
                typeof(Charts),
                new PropertyMetadata((double)25));
        #endregion

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var charts = (d as Charts);
            if (e.Property == DataProperty && e.OldValue != e.NewValue)
            {
                charts.Render();
            }
            if (e.Property == IsLoadingProperty)
            {
                charts.Render();
            }
            if (e.Property == ColumnSelectedIndexProperty && e.OldValue != e.NewValue)
            {
                charts.SetColBorderActiveBg((int)e.OldValue, (int)e.NewValue);
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
        private StackPanel _typeATempContainer;
        private WrapPanel CardContainer;
        private Grid MonthContainer;
        private Border RadarContainer;
        private ListView _listView;
        private Canvas _typeColumnCanvas;
        private Canvas _commonCanvas;

        private Dictionary<int, List<Rectangle>> _typeColValueRectMap;
        private Dictionary<int, Rectangle> _typeColBorderRectMap;
        /// <summary>
        /// 是否在渲染中
        /// </summary>
        private bool isRendering = false;
        /// <summary>
        /// 计算最大值
        /// </summary>
        private double maxValue = 0;

        /// <summary>
        /// 搜索关键字（仅列表样式有效
        /// </summary>
        private string searchKey;
        public Charts()
        {
            DefaultStyleKey = typeof(Charts);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CardContainer = GetTemplateChild("CardContainer") as WrapPanel;
            MonthContainer = GetTemplateChild("MonthContainer") as Grid;
            RadarContainer = GetTemplateChild("Radar") as Border;
            _listView = GetTemplateChild("ListView") as ListView;
            _typeATempContainer = GetTemplateChild("TypeATempContainer") as StackPanel;
            _commonCanvas = GetTemplateChild("CommonCanvasContainer") as Canvas;

            if (ChartsType == ChartsType.Column)
            {
                _typeColumnCanvas = GetTemplateChild("TypeColumnCanvas") as Canvas;
                _typeColumnCanvas.SizeChanged += _typeColumnCanvas_SizeChanged;
            }
            if (ChartsType == ChartsType.Pie)
            {
                _commonCanvas.SizeChanged += _commonCanvas_SizeChanged;
            }
            Render();
        }


        private void _commonCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderPieStyle();
        }

        private void _typeColumnCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderColumnStyle();
        }

        private void Render()
        {
            if (IsLoading)
            {
                RenderLoadingPlaceholder();
            }
            else
            {
                RenderData();
            }
        }
        private void RenderLoadingPlaceholder()
        {
            if (LoadingPlaceholderCount > 0 && CardContainer != null)
            {
                CardContainer.Children.Clear();
                _typeATempContainer.Children.Clear();

                for (int i = 0; i < LoadingPlaceholderCount; i++)
                {
                    switch (ChartsType)
                    {
                        case ChartsType.List:
                            var item = new ChartsItemTypeList();
                            item.IsLoading = true;
                            _typeATempContainer.Children.Add(item);
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

            if (ChartsType == ChartsType.List)
            {
                //不允许最大值小于10，否则效果不好看
                if (maxValue < 10)
                {
                    maxValue = 10;
                }
                //  适当增加最大值
                maxValue = Math.Round(maxValue / 2, MidpointRounding.AwayFromZero) * 2 + 2;

                var countText = GetTemplateChild("ACount") as Run;
                if (countText != null)
                {
                    countText.Text = Data.Count().ToString();
                }
            }

            DataMaxValue = maxValue;


        }

        private void RenderData()
        {
            if (isRendering || CardContainer == null)
            {
                return;
            }


            Calculate();
            CardContainer.Children.Clear();
            MonthContainer.Children.Clear();
            _typeATempContainer.Children.Clear();
            _commonCanvas.Children.Clear();

            RadarContainer.Child = null;
            ListViewBindingData = null;

            if (Data == null || Data.Count() <= 0)
            {
                CardContainer.Children.Add(new EmptyData());
                RadarContainer.Child = new EmptyData();
                _typeATempContainer.Children.Add(new EmptyData());
                _commonCanvas.Children.Add(new EmptyData());

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
                case ChartsType.List:
                    RenderListStyle();
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
                case ChartsType.Pie:
                    RenderPieStyle();
                    break;
            }

        }

        #region 渲染列表样式
        private void RenderListStyle()
        {
            ListViewBindingData = Data.OrderByDescending(x => x.Value).ToList();
            if (ShowLimit > 0)
            {
                ListViewBindingData = ListViewBindingData.Take(ShowLimit);
            }

            isRendering = false;
            var searchBox = GetTemplateChild("ASearchBox") as InputBox;
            if (searchBox != null)
            {
                searchBox.TextChanged += SearchBox_TextChanged;
            }

            _listView.MouseLeftButtonUp += _listView_MouseLeftButtonUp;
            _listView.MouseRightButtonUp += _listView_MouseRightButtonUp;

            if (!IsCanScroll)
            {
                _listView.PreviewMouseWheel += _listView_PreviewMouseWheel;
            }
        }

        private void _listView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;

                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;

                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void _listView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_listView.SelectedItem != null)
            {
                if (ItemMenu != null)
                {
                    ItemMenu.IsOpen = true;
                    ItemMenu.Tag = _listView.SelectedItem;
                }
            }
        }

        private void _listView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_listView.SelectedItem != null)
            {
                OnItemClick?.Invoke(_listView.SelectedItem, null);
                ClickCommand?.Execute(_listView.SelectedItem);
            }
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
                    var newListData = new List<ChartsDataModel>();

                    foreach (var data in Data)
                    {
                        string content = (data.Name + data.PopupText).ToLower();
                        bool show = content.IndexOf(searchKey) != -1;

                        if (!show)
                        {
                            show = searchKey == "忽略" && data.BadgeList.Where(m => m.Type == ChartBadgeType.Ignore).Any();
                            if (data.BadgeList.Any())
                            {
                                //  搜索徽章名（分类名）
                                show = data.BadgeList.Where(m => m.Name.ToLower().Contains(searchKey)).Any();
                            }
                        }

                        if (show)
                        {
                            newListData.Add(data);
                        }
                    }

                    ListViewBindingData = newListData;
                });

            });
        }


        //private void RenderTypeAItems(List<ChartsDataModel> data, int start, int count)
        //{

        //    for (int i = start; i < start + count; i++)
        //    {

        //        if (i >= data.Count)
        //        {
        //            break;
        //        }
        //        ChartsDataModel item = data[i];
        //        var chartsItem = new ChartsItemTypeA();
        //        chartsItem.Data = item;
        //        chartsItem.ToolTip = item.PopupText;
        //        chartsItem.MaxValue = maxValue;
        //        chartsItem.IsShowBadge = IsShowBadge;

        //        if (i == 19 && isLazyloading)
        //        {
        //            chartsItem.Loaded += ChartsItem_Loaded;
        //        }
        //        //  处理点击事件
        //        HandleItemClick(chartsItem, item);
        //        if (!IsCanScroll)
        //        {
        //            NoScrollContainer.Children.Add(chartsItem);
        //        }
        //        else
        //        {
        //            Container.Children.Add(chartsItem);
        //        }
        //        if (ShowLimit > 0 && i == ShowLimit - 1)
        //        {
        //            break;
        //        }

        //    }

        //}

        //private void ChartsItem_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var chartsItem = (ChartsItemTypeA)sender;
        //    chartsItem.Loaded -= ChartsItem_Loaded;

        //    int count = Data.Count() - 20;
        //    for (int i = 0; i < count; i++)
        //    {
        //        var pl = new TextBlock();
        //        pl.Height = chartsItem.ActualHeight;
        //    }

        //    itemHeight = chartsItem.ActualHeight;
        //}
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
                    //if (ShowLimit > 0 && Container.Children.Count == ShowLimit)
                    //{
                    //    break;
                    //}
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
        private void SetColBorderActiveBg(int oldIndex, int newIndex)
        {
            if (!IsCanColumnSelect || _typeColBorderRectMap == null)
            {
                return;
            }

            if (_typeColBorderRectMap.ContainsKey(oldIndex))
            {
                var oldItem = _typeColBorderRectMap[oldIndex];
                oldItem.Fill = new SolidColorBrush(Colors.Transparent);
            }
            if (_typeColBorderRectMap.ContainsKey(newIndex))
            {
                var background = Application.Current.Resources["ThemeBrush"] as SolidColorBrush;

                var checkItem = _typeColBorderRectMap[newIndex];
                checkItem.Fill = new SolidColorBrush(background.Color) { Opacity = .1 };
            }
        }
        /// <summary>
        /// 渲染柱形图
        /// </summary>
        private void RenderColumnStyle()
        {
            if (_typeColumnCanvas == null || _typeColumnCanvas.ActualHeight == 0)
            {
                return;
            }

            _typeColumnCanvas.Children.Clear();
            ColumnInfoList = null;
            _typeColValueRectMap = new Dictionary<int, List<Rectangle>>();
            _typeColBorderRectMap = new Dictionary<int, Rectangle>();

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
            int colValueCount = Data.FirstOrDefault().Values.Length;
            double[] tempValueArr = new double[colValueCount];

            foreach (var item in Data)
            {

                for (int i = 0; i < colValueCount; i++)
                {
                    tempValueArr[i] += item.Values[i];
                }

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
            else
            {
                if (IsStack)
                {
                    maxValue = tempValueArr.Max();
                }
            }
            if (maxValue == 0)
            {
                maxValue = 10;
            }

            Maximum = Covervalue(maxValue);
            Median = Covervalue((maxValue / 2));
            Total = Covervalue(total);

            //  列数
            int columns = Data.FirstOrDefault().Values.Length;


            //  间距
            int margin = 5;

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

            //  列名高度
            const double colNameHeight = 30;
            //  列名下边距
            const double colNameBottomMargin = 5;
            //  画布高度
            double canvasHeight = _typeColumnCanvas.ActualHeight - colNameHeight - colNameBottomMargin;
            //  画布宽度
            double canvasWidth = _typeColumnCanvas.ActualWidth;
            //  边框宽度
            double columnBorderWidth = _typeColumnCanvas.ActualWidth / columns;
            //  列值宽度
            double colValueRectWidth = _typeColumnCanvas.ActualWidth / columns - (margin * 2);

            for (int i = 0; i < columns; i++)
            {
                //  绘制列边框
                var columnBorder = new Rectangle();
                columnBorder.Width = columnBorderWidth;
                columnBorder.Height = canvasHeight;
                columnBorder.Fill = new SolidColorBrush(Colors.Transparent);
                Canvas.SetLeft(columnBorder, i * columnBorderWidth);
                Canvas.SetTop(columnBorder, colNameBottomMargin);
                Panel.SetZIndex(columnBorder, 999);
                _typeColumnCanvas.Children.Add(columnBorder);
                _typeColBorderRectMap.Add(i, columnBorder);

                //  列名
                string colName = list[0].ColumnNames != null && list[0].ColumnNames.Length > 0 ? list[0].ColumnNames[i] : (i + NameIndexStart).ToString();
                //  绘制列名
                TextBlock colNameText = new TextBlock();
                colNameText.TextAlignment = TextAlignment.Center;
                colNameText.Width = columnBorderWidth;
                colNameText.FontSize = 12;
                colNameText.Text = colName;
                colNameText.Foreground = UI.Base.Color.Colors.GetFromString("#FF8A8A8A");

                Canvas.SetLeft(colNameText, i * columnBorderWidth);
                Canvas.SetBottom(colNameText, colNameBottomMargin);
                _typeColumnCanvas.Children.Add(colNameText);


                var valuesPopupList = new List<ChartColumnInfoModel>();

                _typeColValueRectMap.Add(i, new List<Rectangle>());

                for (int di = 0; di < columnCount; di++)
                {
                    var item = list[di];
                    //string colColor = item.Color == null ? UI.Base.Color.Colors.MainColors[di] : item.Color;
                    string themeColor = ((System.Windows.Media.Color)Application.Current.Resources["ThemeColor"]).ToString();
                    string colColor = item.Color == null ? themeColor : item.Color;
                    double value = item.Values[i];


                    if (value > 0)
                    {
                        //  绘制列
                        var colValueRect = new Rectangle();
                        colValueRect.Width = colValueRectWidth;
                        colValueRect.Height = value / maxValue * canvasHeight;
                        colValueRect.Fill = UI.Base.Color.Colors.GetFromString(colColor);
                        if (!IsStack)
                        {
                            colValueRect.RadiusX = 4;
                            colValueRect.RadiusY = 4;
                        }
                        Canvas.SetLeft(colValueRect, i * columnBorderWidth + margin);
                        Canvas.SetBottom(colValueRect, colNameHeight);

                        _typeColumnCanvas.Children.Add(colValueRect);
                        _typeColValueRectMap[i].Add(colValueRect);

                        //  列分类统计数据
                        valuesPopupList.Add(new ChartColumnInfoModel()
                        {
                            Color = item.Color,
                            Name = item.Name,
                            Icon = item.Icon,
                            Text = Covervalue(value) + Unit,
                            Value = value,
                        });
                    }
                }
                var index = i;

                columnBorder.MouseEnter += (e, c) =>
                {
                    ColumnValuesInfoList = valuesPopupList.OrderByDescending(m => m.Value).ToList();
                    ValuesPopupPlacementTarget = columnBorder;
                    IsShowValuesPopup = valuesPopupList.Count > 0;

                    ValuesPopupHorizontalOffset = -17.5 + (columnBorder.ActualWidth / 2);

                    if (ColumnSelectedIndex != index)
                    {
                        var themeBrush = Application.Current.Resources["ThemeBrush"] as SolidColorBrush;
                        columnBorder.Fill = new SolidColorBrush(themeBrush.Color) { Opacity = .05 };
                    }
                };
                columnBorder.MouseLeave += (e, c) =>
                {
                    IsShowValuesPopup = false;
                    if (ColumnSelectedIndex != index)
                    {
                        columnBorder.Fill = new SolidColorBrush(Colors.Transparent);

                    }
                };
                if (IsCanColumnSelect)
                {
                    columnBorder.MouseLeftButtonDown += (e, c) =>
                    {
                        ColumnSelectedIndex = index;
                    };
                }
            }
            //  调整列值 zindex
            foreach (var item in _typeColValueRectMap)
            {
                var rectList = IsStack ? item.Value : item.Value.OrderByDescending(m => m.Height).ToList();
                for (int i = 0; i < rectList.Count; i++)
                {
                    var rect = rectList[i];
                    if (IsStack)
                    {
                        double marginBottom = 0;
                        if (i > 0)
                        {
                            var lastRect = rectList[i - 1];
                            marginBottom = Canvas.GetBottom(lastRect) + lastRect.Height;
                            Canvas.SetBottom(rect, marginBottom);
                        }
                    }
                    else
                    {
                        Panel.SetZIndex(rect, i);
                    }
                }
            }


            //  最高
            var topValueText = new TextBlock();
            topValueText.Text = Maximum;
            topValueText.FontSize = 12;
            topValueText.Foreground = UI.Base.Color.Colors.GetFromString("#ccc");
            topValueText.ToolTip = "最高值";

            var topValueTextSize = UIHelper.MeasureString(topValueText);
            Panel.SetZIndex(topValueText, 1000);
            Canvas.SetRight(topValueText, 0);
            Canvas.SetTop(topValueText, colNameBottomMargin - topValueTextSize.Height / 2);
            _typeColumnCanvas.Children.Add(topValueText);

            var topValueLine = new Line();
            topValueLine.Stroke = UI.Base.Color.Colors.GetFromString("#ccc");
            topValueLine.StrokeDashArray = new DoubleCollection() { 2, 5 };
            topValueLine.X1 = colNameBottomMargin;
            topValueLine.X2 = canvasWidth - colNameBottomMargin - (topValueTextSize.Width);
            topValueLine.Y1 = colNameBottomMargin;
            topValueLine.Y2 = colNameBottomMargin;
            topValueLine.StrokeThickness = 1;
            _typeColumnCanvas.Children.Add(topValueLine);

            //  中间值
            double midY = (maxValue / 2) / maxValue * canvasHeight + colNameBottomMargin;

            var midValueText = new TextBlock();
            midValueText.Text = Median;
            midValueText.FontSize = 12;
            midValueText.Foreground = UI.Base.Color.Colors.GetFromString("#ccc");
            midValueText.ToolTip = "中间值";

            var midValueTextSize = UIHelper.MeasureString(midValueText);
            Panel.SetZIndex(midValueText, 1000);
            Canvas.SetRight(midValueText, 0);
            Canvas.SetTop(midValueText, midY - midValueTextSize.Height / 2);
            _typeColumnCanvas.Children.Add(midValueText);

            var midValueLine = new Line();
            midValueLine.Stroke = UI.Base.Color.Colors.GetFromString("#ccc");
            midValueLine.StrokeDashArray = new DoubleCollection() { 2, 5 };
            midValueLine.StrokeThickness = 1;


            midValueLine.X1 = colNameBottomMargin;
            midValueLine.X2 = canvasWidth - colNameBottomMargin - (midValueTextSize.Width);
            midValueLine.Y1 = midY;
            midValueLine.Y2 = midY;
            _typeColumnCanvas.Children.Add(midValueLine);


            //  平均
            double avg = tempValueArr.Average();
            double avgY = (avg) / maxValue * canvasHeight;

            var avgValueText = new TextBlock();
            avgValueText.Text = Covervalue(avg);
            avgValueText.FontSize = 12;
            avgValueText.Foreground = UI.Base.Color.Colors.GetFromString(StateData.ThemeColor);
            avgValueText.ToolTip = "平均值";

            var avgValueTextSize = UIHelper.MeasureString(avgValueText);
            Panel.SetZIndex(avgValueText, 1000);
            Canvas.SetRight(avgValueText, 0);
            Canvas.SetBottom(avgValueText, avgY + colNameHeight - avgValueTextSize.Height / 2);
            _typeColumnCanvas.Children.Add(avgValueText);

            var avgValueLine = new Line();
            avgValueLine.Stroke = UI.Base.Color.Colors.GetFromString(StateData.ThemeColor);
            avgValueLine.StrokeDashArray = new DoubleCollection() { 2, 5 };
            avgValueLine.StrokeThickness = 1;
            avgValueLine.X1 = colNameBottomMargin;
            avgValueLine.X2 = canvasWidth - colNameBottomMargin - (avgValueTextSize.Width);
            avgValueLine.Y1 = canvasHeight - avgY + colNameBottomMargin + avgValueLine.StrokeThickness;
            avgValueLine.Y2 = canvasHeight - avgY + colNameBottomMargin + avgValueLine.StrokeThickness;
            //avgValueLine.Width = canvasWidth;
            //avgValueLine.Height = 10;
            //Canvas.SetBottom(avgValueLine, avgY + colNameHeight);
            _typeColumnCanvas.Children.Add(avgValueLine);

            //  组装分类统计数据
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
                isRendering = false;
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

        #region 饼图
        private void RenderPieStyle()
        {
            if (_commonCanvas == null)
            {
                isRendering = false;
                return;
            }
            _commonCanvas.Children.Clear();
            var item = new ChartsItemTypePie();
            item.Width = _commonCanvas.ActualWidth;
            item.Height = _commonCanvas.ActualHeight;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            item.VerticalAlignment = VerticalAlignment.Top;
            item.Data = Data.OrderBy(m => m.Value).ToList();
            _commonCanvas.Children.Add(item);

            isRendering = false;
        }
        #endregion

        private string Covervalue(double value)
        {
            if (DataValueType == ChartDataValueType.Seconds)
            {
                return LocalizationServicer.Instance.Translated(Time.ToString((int)value));
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
