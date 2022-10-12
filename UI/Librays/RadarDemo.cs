using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Librays
{
    public static class RadarDemo
    {
        static float mW = 1200;
        static float mH = 1200;
        static Dictionary<string, float> mData = new Dictionary<string, float>
  {
      //{ "速度",77},
      { "力量", 72},
      { "防守", 110},
      { "射门", 50},
      { "传球", 80},
      { "耐力", 60 }
  };//维度数据
        static float mCount = mData.Count; //边数
        static float mCenter = mW * 0.5f; //中心点
        static float mRadius = mCenter - 100; //半径(减去的值用于给绘制的文本留空间)
        static double mAngle = (Math.PI * 2) / mCount; //角度
        static Graphics graphics = null;
        static int mPointRadius = 5; // 各个维度分值圆点的半径  
        static int textFontSize = 18;  //顶点文字大小 px
        const string textFontFamily = "Microsoft Yahei"; //顶点字体
        static Color lineColor = Color.Green;
        static Color fillColor = Color.FromArgb(128, 255, 0, 0);
        static Color fontColor = Color.Black;
        public static void Show()
        {
            Bitmap img = new Bitmap((int)mW, (int)mH);
            graphics = Graphics.FromImage(img);
            graphics.Clear(Color.White);
            img.Save($"{AppDomain.CurrentDomain.BaseDirectory}/0.png", ImageFormat.Jpeg);
            DrawPolygon(graphics);
            img.Save($"{AppDomain.CurrentDomain.BaseDirectory}/1.png", ImageFormat.Png);
            DrawLines(graphics);
            img.Save($"{AppDomain.CurrentDomain.BaseDirectory}/2.png", ImageFormat.Png);
            DrawText(graphics);
            img.Save($"{AppDomain.CurrentDomain.BaseDirectory}/3.png", ImageFormat.Png);
            DrawRegion(graphics);
            img.Save($"{AppDomain.CurrentDomain.BaseDirectory}/4.png", ImageFormat.Png);
            DrawCircle(graphics);
            img.Save($"{AppDomain.CurrentDomain.BaseDirectory}/5.png", ImageFormat.Png);
            img.Dispose();
            graphics.Dispose();
        }
        // 绘制多边形边
        private static void DrawPolygon(Graphics ctx)
        {
            var r = mRadius / mCount; //单位半径
            Pen pen = new Pen(lineColor);
            //画6个圈
            for (var i = 0; i < mCount; i++)
            {
                var points = new List<PointF>();
                var currR = r * (i + 1); //当前半径
                                         //画6条边
                for (var j = 0; j < mCount; j++)
                {
                    var x = (float)(mCenter + currR * Math.Cos(mAngle * j));
                    var y = (float)(mCenter + currR * Math.Sin(mAngle * j));
                    points.Add(new PointF { X = x, Y = y });
                }
                ctx.DrawPolygon(pen, points.ToArray());
                //break;
            }
            ctx.Save();
        }
        //顶点连线
        private static void DrawLines(Graphics ctx)
        {
            for (var i = 0; i < mCount; i++)
            {
                var x = (float)(mCenter + mRadius * Math.Cos(mAngle * i));
                var y = (float)(mCenter + mRadius * Math.Sin(mAngle * i));
                ctx.DrawLine(new Pen(lineColor), new PointF { X = mCenter, Y = mCenter }, new PointF { X = x, Y = y });
                //break;
            }
            ctx.Save();
        }
        //绘制文本
        private static void DrawText(Graphics ctx)
        {
            var fontSize = textFontSize;//mCenter / 12;
            Font font = new Font(textFontFamily, fontSize, FontStyle.Regular);
            int i = 0;
            foreach (var item in mData)
            {
                var x = (float)(mCenter + mRadius * Math.Cos(mAngle * i));
                var y = (float)(mCenter + mRadius * Math.Sin(mAngle * i) - fontSize);
                if (mAngle * i > 0 && mAngle * i <= Math.PI / 2)
                {
                    ctx.DrawString(item.Key, font, new SolidBrush(fontColor), x - ctx.MeasureString(item.Key, font).Width * 0.5f, y + fontSize/* y + fontSize*/);
                }
                else if (mAngle * i > Math.PI / 2 && mAngle * i <= Math.PI)
                {
                    ctx.DrawString(item.Key, font, new SolidBrush(fontColor), x - ctx.MeasureString(item.Key, font).Width, y /*y + fontSize*/);
                }
                else if (mAngle * i > Math.PI && mAngle * i <= Math.PI * 3 / 2)
                {
                    ctx.DrawString(item.Key, font, new SolidBrush(fontColor), x - ctx.MeasureString(item.Key, font).Width, y);
                }
                else if (mAngle * i > Math.PI * 3 / 2)
                {
                    ctx.DrawString(item.Key, font, new SolidBrush(fontColor), x - ctx.MeasureString(item.Key, font).Width * 0.5f, y - fontSize * 0.5f);
                }
                else
                {
                    ctx.DrawString(item.Key, font, new SolidBrush(fontColor), x, y /* y + fontSize*/);
                }
                i++;
            }
            ctx.Save();
        }
        //绘制数据区域
        private static void DrawRegion(Graphics ctx)
        {
            int i = 0;
            List<PointF> points = new List<PointF>();
            foreach (var item in mData)
            {
                var x = (float)(mCenter + mRadius * Math.Cos(mAngle * i) * item.Value / 100);
                var y = (float)(mCenter + mRadius * Math.Sin(mAngle * i) * item.Value / 100);
                points.Add(new PointF { X = x, Y = y });
                //ctx.DrawArc(new Pen(lineColor), x, y, r, r, 0, (float)Math.PI * 2); 
                i++;
            }
            //GraphicsPath path = new GraphicsPath();
            //path.AddLines(points.ToArray());
            ctx.FillPolygon(new SolidBrush(fillColor), points.ToArray());
            ctx.Save();
        }
        //画点
        private static void DrawCircle(Graphics ctx)
        {
            //var r = mCenter / 18;
            var r = mPointRadius;
            int i = 0;
            foreach (var item in mData)
            {
                var x = (float)(mCenter + mRadius * Math.Cos(mAngle * i) * item.Value / 100);
                var y = (float)(mCenter + mRadius * Math.Sin(mAngle * i) * item.Value / 100);
                ctx.FillPie(new SolidBrush(fillColor), x - r, y - r, r * 2, r * 2, 0, 360);
                //ctx.DrawArc(new Pen(lineColor), x, y, r, r, 0, (float)Math.PI * 2); 
                i++;
            }
            ctx.Save();
        }
    }
}
