using Core.Librarys;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace UI.Controls.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime))
            {
                return "参数错误";
            }
            var dateTime = (DateTime)value;
            string pre = dateTime.ToString("yyyy年MM月dd日");
            if (dateTime.Date == DateTime.Now.Date)
            {
                pre = "今天";
            }
            else if (dateTime.Date == DateTime.Now.Date.AddDays(-1).Date)
            {
                pre = "昨天";
            }

            return $"{pre} {dateTime.ToString("HH点")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
