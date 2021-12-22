using Core.Models.Config.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Config
{
    /// <summary>
    /// 应用设置
    /// </summary>
    public class ConfigModel
    {
        /// <summary>
        /// 关联进程
        /// </summary>
        public List<LinkModel> Links { get; set; }

        /// <summary>
        /// 常规
        /// </summary>
        public GeneralModel General { get; set; }

        /// <summary>
        /// 行为
        /// </summary>
        public BehaviorModel Behavior { get; set; }

    }
}
