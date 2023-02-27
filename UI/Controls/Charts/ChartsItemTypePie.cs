using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Base.Color;
using UI.Controls.Charts.Model;

namespace UI.Controls.Charts
{
    public class ChartsItemTypePie : Canvas
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
                typeof(ChartsItemTypePie));


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
                typeof(ChartsItemTypePie));


        #endregion
        private double _lastAngle = -Math.PI / 2;
        private int _zIndex = 1;
        private List<Path> _paths = new List<Path>();
        public ChartsItemTypePie()
        {
            DefaultStyleKey = typeof(ChartsItemTypePie);
            Loaded += ChartsItemTypePie_Loaded;
        }

        private void ChartsItemTypePie_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            _paths.Clear();
            Children.Clear();
            MaxValue = Data.Sum(m => m.Value);

            int i = 0;
            foreach (var item in Data)
            {
                var angle = item.Value / MaxValue * 360;
                //if (i == 1)
                //{
                //    angle += 10;
                //}

                var path = CreatePath(angle, UI.Base.Color.Colors.GetFromString(item.Color));
                path.ToolTip = item.PopupText;
                path.MouseEnter += Path_MouseEnter;
                path.MouseLeave += Path_MouseLeave;
                _paths.Add(path);
                Children.Add(path);
                i++;
            }
        }

        private void Path_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            foreach (var p in _paths)
            {
                p.Opacity = 1;
            }
        }

        private void Path_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var path = sender as Path;
            foreach (var p in _paths)
            {
                if (p != path)
                {
                    p.Opacity = .2;
                }
            }
        }
        private double _lastX = -1, _lastY = -1;
        private Path CreatePath(double angle_, SolidColorBrush color_)
        {
            Path path = new Path();

            PathGeometry pathGeometry = new PathGeometry();
            double Radius = ActualHeight / 2;
            //double Angle = angle_;
            //Point startPoint = new Point(Radius, Radius);

            //if (Angle >= 360)
            //{
            //    Angle = 359;
            //}


            double x = Math.Cos(_lastAngle) * Radius + Radius;
            double y = Math.Sin(_lastAngle) * Radius + Radius;
            var lin1 = new LineSegment(new Point(x, y), true);

            _lastAngle += Math.PI * angle_ / 180;

            x = Math.Cos(_lastAngle) * Radius + Radius;
            y = Math.Sin(_lastAngle) * Radius + Radius;
            //Point endPoint = ComputeCartesianCoordinate(Angle, Radius);
            //endPoint.X += Radius;
            //endPoint.Y += Radius;

            //_lastX = endPoint.X;
            //_lastY = endPoint.Y;
            //Debug.WriteLine($"angle:{angle_},start:{startPoint},endpoint:{endPoint}");

            var arcSeg = new ArcSegment()
            {
                Size = new Size(Radius, Radius),
                IsLargeArc = angle_ > 180,
                SweepDirection = SweepDirection.Clockwise,
                Point = new Point(x, y),
                RotationAngle = angle_,
            };
            var line2 = new LineSegment(new Point(Radius, Radius), true);
            var fig = new PathFigure(new Point(Radius, Radius), new PathSegment[] { lin1, arcSeg, line2 }, false);
            pathGeometry.Figures.Add(fig);
            path.Data = pathGeometry;
            path.Fill = color_;
            return path;
        }
        //private Path CreatePath(double angle_, SolidColorBrush color_)
        //{
        //    Path path = new Path();
        //    path.Stroke = color_;
        //    path.StrokeThickness = ActualHeight / 2;
        //    //path.StrokeStartLineCap = PenLineCap.Round;
        //    //path.StrokeEndLineCap = PenLineCap.Round;

        //    double width = path.StrokeThickness;

        //    PathGeometry pathGeometry = new PathGeometry();
        //    double Radius = (ActualHeight - width) / 2;
        //    double Angle = angle_ + _lastAngle;
        //    Point startPoint = new Point(Radius, 0);
        //    if (Angle >= 360)
        //    {
        //        Angle = 359;
        //    }
        //    Point endPoint = ComputeCartesianCoordinate(Angle, Radius);
        //    endPoint.X += Radius;
        //    endPoint.Y += Radius;

        //    var pf = new PathFigure()
        //    {
        //        StartPoint = startPoint
        //    };
        //    pf.Segments.Add(new ArcSegment()
        //    {
        //        Size = new Size(Radius, Radius),
        //        IsLargeArc = Angle > 180,
        //        SweepDirection = SweepDirection.Clockwise,
        //        Point = endPoint,
        //    });
        //    pathGeometry.Figures.Add(pf);
        //    path.Data = pathGeometry;
        //    _lastAngle = Angle;
        //    Canvas.SetLeft(path, width / 2);
        //    Canvas.SetTop(path, width / 2);
        //    Canvas.SetZIndex(path, _zIndex);
        //    _zIndex--;
        //    return path;
        //}
        private Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }

    }
}
