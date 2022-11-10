using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Controls.Charts.Model
{
    /// <summary>
    /// 徽章模型
    /// </summary>
    public class ChartBadgeModel
    {
        /// <summary>
        /// 徽章名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 徽章颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 徽章类型
        /// </summary>
        public ChartBadgeType Type { get; set; }
        /// <summary>
        /// 忽略应用徽章
        /// </summary>
        public static ChartBadgeModel IgnoreBadge = new ChartBadgeModel()
        {
            Name = "忽略",
            Color = "#f51837",
            Type = ChartBadgeType.Ignore
        };
    }
}
