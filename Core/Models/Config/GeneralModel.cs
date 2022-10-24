using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Config
{
    /// <summary>
    /// 常规
    /// </summary>
    public class GeneralModel
    {
        [Config(Name = "开机自启动", Description = "在电脑启动时自动运行 Tai", Group = "基础")]
        /// <summary>
        /// 是否启用开机自启动
        /// </summary>
        public bool IsStartatboot { get; set; }

        [Config(Options = "浅色|深色", Name = "主题模式", Description = "设置以浅色或深色模式显示", Group = "外观")]
        public int Theme { get; set; } = 0;
    }
}
