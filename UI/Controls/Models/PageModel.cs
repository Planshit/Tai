using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UI.Controls.Models
{
    public class PageModel
    {
        /// <summary>
        /// 页面实例
        /// </summary>
        public Page Instance { get; set; }
        /// <summary>
        /// 滚动条位置
        /// </summary>
        public double ScrollValue { get; set; }
    }
}
