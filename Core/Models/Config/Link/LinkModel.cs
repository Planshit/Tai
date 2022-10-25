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
        [Config(Name = "名称", Description = "设置一个名称用于识别该关联", IsName = true, IsCanRepeat = false)]
        public string Name { get; set; } = "新的关联";
        /// <summary>
        /// 关联进程列表
        /// </summary>
        [Config(Name = "关联进程", Description = "将需要关联的进程名称（不带.exe）添加到此，至少两个。", IsCanRepeat = false)]
        public List<string> ProcessList { get; set; } = new List<string>();
    }
}
