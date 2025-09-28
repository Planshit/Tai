using Core.Librarys;
using Core.Librarys.Image;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using UI.Servicers;

namespace UI.Controls.Charts
{
    public class ChartsItemTypeRadar : Control
    {
        #region Data
        /// <summary>
        /// Data
        /// </summary>
        public List<ChartsDataModel> Data
        {
            get { return (List<ChartsDataModel>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data",
                typeof(List<ChartsDataModel>),
                typeof(ChartsItemTypeRadar));


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
                typeof(ChartsItemTypeRadar));


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
                typeof(ChartsItemTypeRadar));


        #endregion

        public Geometry RadarPathData
        {
            get { return (Geometry)GetValue(RadarPathDataProperty); }
            set { SetValue(RadarPathDataProperty, value); }
        }
        public static readonly DependencyProperty RadarPathDataProperty =
            DependencyProperty.Register("RadarPathData",
                typeof(Geometry),
                typeof(ChartsItemTypeRadar));

        private Canvas canvas;
        public ChartsItemTypeRadar()
        {
            DefaultStyleKey = typeof(ChartsItemTypeRadar);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            canvas = GetTemplateChild("Canvas") as Canvas;
            Loaded += ChartsItemTypeRadar_Loaded;
        }

        private void ChartsItemTypeRadar_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            if (Data == null || Data.Count == 0)
            {
                return;
            }
            double size = ActualWidth != double.NaN ? ActualWidth : 200;
            //size -= 50;
            int count = Data.Count;

            //  边长
            double lineWidth = size / 2;

            //  角度
            double angle = (Math.PI * 2) / count;

            var r = lineWidth / count;

            //  多边形边框
            for (int i = 0; i < 3; i++)
            {
                var points = new List<Point>();
                //  当前半径
                var currR = (r - (5 * i)) * (count - i);

                for (int j = 0; j < count; j++)
                {
                    points.Add(new Point(lineWidth + currR * Math.Cos(angle * j), lineWidth + currR * Math.Sin(angle * j)));
                }
                //  画出边框
                for (int i2 = 0; i2 < points.Count; i2++)
                {
                    var line = new Line();
                    line.X1 = points[i2].X;
                    line.Y1 = points[i2].Y;

                    line.X2 = i2 == points.Count - 1 ? points[0].X : points[i2 + 1].X;
                    line.Y2 = i2 == points.Count - 1 ? points[0].Y : points[i2 + 1].Y;

                    line.StrokeThickness = i == 0 ? 1 : 1;

                    line.Stroke = UI.Base.Color.Colors.GetFromString(i == 0 ? "#eeeef2" : "#eeeef2");

                    canvas.Children.Add(line);

                }
            }

            //  顶点连线
            for (var i = 0; i < count; i++)
            {
                //double diff = i == 0 || i == count - 1 ? 0 : 5;
                double diff = 0;
                var x = (double)(lineWidth + (lineWidth - diff) * Math.Cos((angle) * i));
                var y = (double)(lineWidth + (lineWidth - diff) * Math.Sin((angle) * i));

                var line = new Line();
                line.X1 = lineWidth;
                line.Y1 = lineWidth;

                line.X2 = x;
                line.Y2 = y;

                line.StrokeThickness = .5;
                line.Stroke = UI.Base.Color.Colors.GetFromString("#dedede");

                canvas.Children.Add(line);

                //  类别文字
                var font = new TextBlock();
                font.Text = Data[i].Name.Length > 4 ? Data[i].Name.Substring(0, 4) : Data[i].Name;
                font.Foreground = UI.Base.Color.Colors.GetFromString("#7f7f7f");
                font.FontSize = 12;
                font.ToolTip = $"{Data[i].Name} {LocalizationServicer.Instance.Translated(Time.ToString((int)Data[i].Values.Sum()))}";

                var textSize = MeasureString(font);
                Debug.WriteLine(font.Text + " -> " + angle * i);
                if (angle * i > 0 && angle * i <= Math.PI / 2)
                {
                    // >0 && <  1.57
                    x += textSize.Height / 2;
                    y += textSize.Height / 2;
                }
                else if (angle * i > 1.79 && angle * i < 1.8)
                {
                    x += textSize.Height / 2;
                    y += textSize.Width / 2;
                }
                else if (angle * i > 2.09 && angle * i < 2.1)
                {
                    x += textSize.Height / 2;
                    y += textSize.Height / 2;
                }
                else if (angle * i > 2.51 && angle * i < 2.52)
                {
                    x += textSize.Height / 2;
                    y += textSize.Height / 2;
                }
                else if (angle * i > 2.69 && angle * i < 2.7)
                {
                    x += textSize.Height / 2;
                    y += textSize.Height / 2;
                }
                else if (angle * i > Math.PI / 2 && angle * i <= Math.PI)
                {
                    // > 1.57 && < 3.14
                    x -= textSize.Height / 2;
                    y -= textSize.Width / 2;

                }
                //else if (angle * i > 3.5 && angle * i < 4.2)
                //{
                //    x -= textSize.Height / 2;
                //    y -= textSize.Width / 2;

                //}
                else if (angle * i > Math.PI && angle * i <= Math.PI * 3 / 2)
                {
                    // > 3.14 && < 4.71
                    x += font.FontSize / 2;
                    y -= textSize.Width + textSize.Height / 2;
                }
                else if (angle * i > Math.PI * 3 / 2)
                {
                    //  > 4.71
                    x += textSize.Height / 2;
                    y -= textSize.Width + textSize.Height / 2;
                }
                else
                {
                    //  顶点
                    x += textSize.Height + textSize.Height / 2;
                    y -= textSize.Width / 2;
                }

                font.RenderTransform = new RotateTransform()
                {
                    Angle = 90
                };
                Canvas.SetLeft(font, x);
                Canvas.SetTop(font, y);
                canvas.Children.Add(font);
            }

            //  中心装饰点
            var centerPoint = new Ellipse();
            centerPoint.Width = 15;
            centerPoint.Height = 15;
            centerPoint.Fill = UI.Base.Color.Colors.GetFromString("#ffffff");
            centerPoint.Stroke = UI.Base.Color.Colors.GetFromString("#dedede");
            centerPoint.StrokeThickness = 1;
            Canvas.SetLeft(centerPoint, lineWidth - centerPoint.Width / 2);
            Canvas.SetTop(centerPoint, lineWidth - centerPoint.Width / 2);
            canvas.Children.Add(centerPoint);

            //  数据区域
            var pc = new PointCollection();

            for (int i = 0; i < count; i++)
            {
                double sum = Data[i].Values.Sum();
                double value = sum / MaxValue;
                value = value == 1 ? 0.97 : value;

                var x = (double)(lineWidth + (lineWidth) * Math.Cos((angle) * i) * value);
                var y = (double)(lineWidth + (lineWidth) * Math.Sin((angle) * i) * value);

                pc.Add(new Point(x, y));

                //  数据点
                var dataPoint = new Ellipse();
                dataPoint.Width = 5;
                dataPoint.Height = 5;

                dataPoint.Fill = UI.Base.Color.Colors.GetFromString(StateData.ThemeColor);
                dataPoint.Stroke = UI.Base.Color.Colors.GetFromString(StateData.ThemeColor);
                dataPoint.StrokeThickness = 1;
                Canvas.SetLeft(dataPoint, x - dataPoint.Width / 2);
                Canvas.SetTop(dataPoint, y - dataPoint.Width / 2);
                canvas.Children.Add(dataPoint);
            }

            var p = new Polygon();
            p.Stroke = UI.Base.Color.Colors.GetFromString(StateData.ThemeColor);
            p.Fill = UI.Base.Color.Colors.GetFromString(StateData.ThemeColor, .3);
            p.StrokeThickness = 1;
            p.HorizontalAlignment = HorizontalAlignment.Left;
            p.VerticalAlignment = VerticalAlignment.Center;
            p.Points = pc;
            canvas.Children.Add(p);

        }
        private Size MeasureString(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution());

            return new Size(formattedText.Width, formattedText.Height);
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
