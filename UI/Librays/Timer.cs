using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Librays
{
    public class Timer
    {
        public static string Fromat(int seconds)
        {
            if (seconds < 60)
            {
                return seconds + "秒";
            }
            else
            {
                double minutes = seconds / 60;
                if (minutes < 60)
                {
                    return minutes + "分钟";
                }
                else
                {
                    double hours = (double)seconds / 60;
                    if (hours != (int)hours)
                    {
                        double pointNumber = hours - (int)hours;
                        return (int)hours + "小时" + (int)(pointNumber * 60) + "分";
                    }
                    return hours + "小时";
                }
            }
        }
    }
}
