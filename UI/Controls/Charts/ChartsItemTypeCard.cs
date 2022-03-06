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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.Controls.Charts.Model;

namespace UI.Controls.Charts
{
    public class ChartsItemTypeCard : Control
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
                typeof(ChartsItemTypeCard));


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
                typeof(ChartsItemTypeCard));


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
                typeof(ChartsItemTypeCard));


        #endregion
        private TextBlock NameTextObj, ValueTextObj;
        private Rectangle ValueBlockObj;
        private StackPanel ValueContainer;
        private Image IconObj;
        private bool isRendering = false;
        public ChartsItemTypeCard()
        {
            DefaultStyleKey = typeof(ChartsItemTypeCard);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            NameTextObj = GetTemplateChild("NameTextObj") as TextBlock;
            ValueTextObj = GetTemplateChild("ValueTextObj") as TextBlock;
            ValueBlockObj = GetTemplateChild("ValueBlockObj") as Rectangle;
            ValueContainer = GetTemplateChild("ValueContainer") as StackPanel;

            IconObj = GetTemplateChild("IconObj") as Image;

            Loaded += ChartsItemTypeCard_Loaded;
        }

        private void ChartsItemTypeCard_Loaded(object sender, RoutedEventArgs e)
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

            //对部分程序未获取程序名的程序使用路径中名字作为程序名
            NameTextObj.Text = Data.Name;
            if (Data.Name.Trim() == "" && Data.PopupText.Trim() != "")
            {
                FileInfo fi = new FileInfo(Data.PopupText);
                NameTextObj.Text = fi.Name.Replace(fi.Extension, "");
            }

            NameTextObj.SizeChanged += (e, c) =>
            {
                //  处理文字过长显示
                if (NameTextObj.ActualWidth > 121 && NameTextObj.FontSize > 8)
                {
                    NameTextObj.FontSize = NameTextObj.FontSize - 1;
                }
            };
            ValueTextObj.Text = Data.Tag;
            IconObj.Source = Imager.Load(Data.Icon);

            ValueTextObj.SizeChanged += (e, c) =>
            {
                //if (MaxValue <= 0)
                //{
                //    return;
                //}
                double size = (Data.Value / MaxValue) * ActualWidth/3;
                ValueBlockObj.Width = ValueBlockObj.Height = size;

                ValueBlockObj.Effect = new BlurEffect()
                {
                    Radius = size,
                    RenderingBias = RenderingBias.Performance
                };
                
            };

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
