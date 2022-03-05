using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Config
{
    /// <summary>
    /// 行为
    /// </summary>
    public class BehaviorModel
    {
        [Config(IsCanImportExport = true, Name = "忽略进程", Description = "忽略的进程将不进行时长统计", Group = "忽略进程", Placeholder = "进程名，不需要输入.exe")]
        /// <summary>
        /// 忽略的进程列表
        /// </summary>
        public List<string> IgnoreProcessList { get; set; } = new List<string>();
    }
}
