using Core.Librarys.Image;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public class ChartsItemTypeList : Control
    {
        #region ChartsType 样式类型
        /// <summary>
        /// 样式类型
        /// </summary>
        public ChartsDataModel Data
        {
            get { return (ChartsDataModel)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                typeof(ChartsDataModel),
                typeof(ChartsItemTypeList));


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
                typeof(ChartsItemTypeList));


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
                typeof(ChartsItemTypeList));


        #endregion

        #region IsShowBadge 是否显示徽章
        /// <summary>
        /// 是否显示徽章
        /// </summary>
        public bool IsShowBadge
        {
            get { return (bool)GetValue(IsShowBadgeProperty); }
            set { SetValue(IsShowBadgeProperty, value); }
        }
        public static readonly DependencyProperty IsShowBadgeProperty =
            DependencyProperty.Register("IsShowBadge",
                typeof(bool),
                typeof(ChartsItemTypeList));


        #endregion

        #region 图标大小
        /// <summary>
        /// 图标大小
        /// </summary>
        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize",
                typeof(double),
                typeof(ChartsItemTypeList), new PropertyMetadata((double)25));


        #endregion
        private TextBlock NameTextObj, ValueTextObj;
        private Rectangle ValueBlockObj;
        private StackPanel ValueContainer;
        //private Grid ValueContainer;
        private Image IconObj;
        private bool isRendering = false;
        private bool IsAddEvent = false;
        public ChartsItemTypeList()
        {
            DefaultStyleKey = typeof(ChartsItemTypeList);
            Unloaded += ChartsItemTypeList_Unloaded;
        }

        private void ChartsItemTypeList_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= ChartsItemTypeList_Unloaded;
            Loaded -= ChartsItemTypeList_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            NameTextObj = GetTemplateChild("NameTextObj") as TextBlock;
            ValueTextObj = GetTemplateChild("ValueTextObj") as TextBlock;
            ValueBlockObj = GetTemplateChild("ValueBlockObj") as Rectangle;
            //ValueContainer = GetTemplateChild("ValueContainer") as Grid;
            ValueContainer = GetTemplateChild("ValueContainer") as StackPanel;

            IconObj = GetTemplateChild("IconObj") as Image;

            if (!IsAddEvent)
            {
                Loaded += ChartsItemTypeList_Loaded;
                IsAddEvent = true;
            }

            var parent = Parent as FrameworkElement;
            parent.SizeChanged += Parent_SizeChanged;
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateValueBlockWidth();
        }

        private void ChartsItemTypeList_Loaded(object sender, RoutedEventArgs e)
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
            //NameTextObj.Text = Data.Name;
            //对部分程序未获取程序名的程序使用路径中名字作为程序名
            //if (Data.Name.Trim() == "" && Data.PopupText.Trim() != "")
            //{
            //    FileInfo fi = new FileInfo(Data.PopupText);
            //    NameTextObj.Text = fi.Name.Replace(fi.Extension, "");
            //}

            ValueTextObj.Text = Data.Tag;
            IconObj.Source = Imager.Load(Data.Icon);

            ValueTextObj.SizeChanged += (e, c) =>
            {
                if (MaxValue <= 0)
                {
                    return;
                }
                UpdateValueBlockWidth();
            };
        }

        public void UpdateValueBlockWidth()
        {
            if (Data == null || !IsLoaded)
            {
                return;
            }
            ValueBlockObj.Width = (Data.Value / MaxValue) * (ValueContainer.ActualWidth * 0.95 - ValueTextObj.ActualWidth);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, "MouseOver", true);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, "NormalState", true);
        }
    }
}
