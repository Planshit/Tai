using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UI.Base
{
    public static class UIHelper
    {
        /// <summary>
        /// 测量字符串控件尺寸
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns></returns>
        public static Size MeasureString(TextBlock textBlock)
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
    }
}
