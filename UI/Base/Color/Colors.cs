using Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace UI.Base.Color
{
    public class Colors
    {
        public struct IColor
        {
            /// <summary>
            /// 颜色名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 颜色
            /// </summary>
            public string Color;
        }
        public static Dictionary<ColorTypes, IColor> ColorList = new Dictionary<ColorTypes, IColor>() {
            {ColorTypes.Aquamarine,new IColor{Name="碧绿", Color="#00AA90"}  },
            {ColorTypes.Black,new IColor{Name="暗黑", Color="#080808"} },
            {ColorTypes.Blue,new IColor{Name="蓝色", Color="#0078d4"} },
            {ColorTypes.Cyan,new IColor{Name="青色", Color="#51A8DD"} },
            {ColorTypes.Gold,new IColor{Name="金色", Color="#EFBB24"} },
            {ColorTypes.Gray,new IColor{Name="灰色", Color="#828282"} },
            {ColorTypes.Green,new IColor{Name="绿色", Color="#56C773"} },
            {ColorTypes.Orange,new IColor{Name="橙色", Color="#E98B2A"} },
            {ColorTypes.Pink,new IColor{Name="粉红", Color="#B5495B"} },
            {ColorTypes.Red,new IColor{Name="赤红", Color="#F3221B"} },
            {ColorTypes.Violet,new IColor{Name="紫色", Color="#77428D"} },
            {ColorTypes.Yellow,new IColor{Name="黄色", Color="#FFC408"} },
            {ColorTypes.White,new IColor{Name="白色", Color="#FFFFFF"} },

        };

        public static string[] MainColors = { "#00FFAB", "#A761FF", "#ff587f", "#fff323", "#DAEAF1", "#2B20D9", "#FF5C01", "#9EB23C", "#FF9999", "#998CEB", "#77E4D4", "#FD5E5E", "#B4FF9F", "#07FF01", "#FFE162", "#C70B80", "#590696", "#97DBAE", "#ff1801" };

        public static IColor Get(ColorTypes color)
        {
            return ColorList[color];
        }
        public static SolidColorBrush GetColor(ColorTypes color, double opacity = 1)
        {
            return GetFromString(ColorList[color].Color, opacity);
        }
        public static SolidColorBrush GetFromString(string color, double opacity = 1)
        {
            if (string.IsNullOrEmpty(color))
            {
                color = StateData.ThemeColor;
            }
            return new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(color))
            {
                Opacity = opacity
            };
        }
    }
}
