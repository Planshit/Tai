using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Config.Category;

namespace Core.Models.Config
{
    /// <summary>
    /// 行为
    /// </summary>
    public class BehaviorModel
    {
        [Config(IsCanImportExport = true, Name = "忽略应用", Description = "可以通过进程名称或者正则表达式进行匹配。当使用正则表达式时可以匹配程序路径", Group = "忽略应用", Placeholder = "进程名称，不需要输入.exe。支持正则表达式")]
        /// <summary>
        /// 忽略的进程列表
        /// </summary>
        public List<string> IgnoreProcessList { get; set; } = new List<string>();

        [Config(IsCanImportExport = true, Name = "匹配规则", Description = "文件夹路径或正则表达式对进程进行匹配", Group = "自动分类")]
        public List<AutoCategoryModel> AutoCategoryProcessList { get; set; } = new List<AutoCategoryModel>();
    }
}
