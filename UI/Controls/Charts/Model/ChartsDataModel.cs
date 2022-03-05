using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Controls.Charts.Model
{
    public class ChartsDataModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 悬浮文本
        /// </summary>
        public string PopupText { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 图标文件
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 时间（仅在类型为Month样式时有效）
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 数据模型
        /// </summary>
        public object Data { get; set; }
    }
}
