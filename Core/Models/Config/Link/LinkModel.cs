using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Config.Link
{
    public class LinkModel
    {
        /// <summary>
        /// 关联名称
        /// </summary>
        [Config(Name = "config.linkMode.name.name", Description = "config.linkMode.name.description", IsName = true, IsCanRepeat = false)]
        public string Name { get; set; } = "新的关联";
        /// <summary>
        /// 关联进程列表
        /// </summary>
        [Config(Name = "config.linkMode.processList.name", Description = "config.linkMode.processList.description", IsCanRepeat = false)]
        public List<string> ProcessList { get; set; } = new List<string>();
    }
}
