using Core.Librarys.Image;
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
using UI.Controls.Charts.Model;

namespace UI.Controls.Charts
{
    public class ChartsItemTypeMonth : Control
    {
        #region Data 数据
        /// <summary>
        /// 数据
        /// </summary>
        public ChartsDataModel Data
        {
            get { return (ChartsDataModel)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                typeof(ChartsDataModel),
                typeof(ChartsItemTypeMonth));


        #endregion

        #region MaxValue 最大值
        /// <summary>
        /// 样式类型
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue",
                typeof(double),
                typeof(ChartsItemTypeMonth));


        #endregion

        #region IsLoading 是否正在加载中
        /// <summary>
        /// 是否正在加载中
        /// </summary>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading",
                typeof(bool),
                typeof(ChartsItemTypeMonth));


        #endregion
        #region IsSelected 是否选中
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected",
                typeof(bool),
                typeof(ChartsItemTypeMonth));


        #endregion
        //private TextBlock NameTextObj, ValueTextObj;
        private Rectangle ValueBlockObj;
        //private StackPanel ValueContainer;
        //private Image IconObj;
        private bool isRendering = false;
        private bool IsAddEvent = false;

        public ChartsItemTypeMonth()
        {
            DefaultStyleKey = typeof(ChartsItemTypeMonth);

            Unloaded += ChartsItemTypeMonth_Unloaded;
        }

        private void ChartsItemTypeMonth_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= ChartsItemTypeMonth_Unloaded;
            Loaded -= ChartsItemTypeMonth_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //NameTextObj = GetTemplateChild("NameTextObj") as TextBlock;
            //ValueTextObj = GetTemplateChild("ValueTextObj") as TextBlock;
            ValueBlockObj = GetTemplateChild("ValueBlockObj") as Rectangle;
            //ValueContainer = GetTemplateChild("ValueContainer") as StackPanel;

            //IconObj = GetTemplateChild("IconObj") as Image;
            if (!IsAddEvent)
            {
                IsAddEvent = true;
                Loaded += ChartsItemTypeMonth_Loaded;
            }
        }

        private void ChartsItemTypeMonth_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            if (isRendering || Data == null)
            {
                return;
            }
            isRendering = true;
            //Loaded += (s, e) =>
            //{
            //    ValueBlockObj.Width = ValueBlockObj.Height = (Data.Value / MaxValue) * ActualWidth;
            //};

            double size = (Data.Value / MaxValue) * ActualWidth;
            if (size > 0 && size < 8) //防止历史数值太小界面无显示效果
                size = 8;
            ValueBlockObj.Width = ValueBlockObj.Height = size;
            ToolTip = Data.DateTime.ToString("yyyy年MM月dd日") + " " + (string.IsNullOrEmpty(Data.Tag) ? "无数据" : Data.Tag);

            if (Data.DateTime.Date == DateTime.Now.Date)
            {
                IsSelected = true;
                ToolTip = "[今日] " + ToolTip;
            }
            //ValueTextObj.Text = Data.DateTime.Day.ToString();
            //NameTextObj.Text = Data.Name;
            //NameTextObj.SizeChanged += (e, c) =>
            //{
            //    //  处理文字过长显示
            //    if (NameTextObj.ActualWidth > 121 && NameTextObj.FontSize > 8)
            //    {
            //        NameTextObj.FontSize = NameTextObj.FontSize - 1;
            //    }
            //};
            //ValueTextObj.Text = Data.Tag;
            //IconObj.Source = Imager.Load(Data.Icon);

            //ValueTextObj.SizeChanged += (e, c) =>
            //{
            //    //if (MaxValue <= 0)
            //    //{
            //    //    return;
            //    //}
            //    ValueBlockObj.Width = ValueBlockObj.Height = (Data.Value / MaxValue) * ActualWidth;
            //};
        }
    }
}
