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
        [Config(Name = "睡眠监测", Description = "离开电脑时自动停止统计，重启软件生效。（一般情况不建议关闭）", Group = "偏好")]
        /// <summary>
        /// 睡眠监测
        /// </summary>
        public bool IsSleepWatch { get; set; } = true;
        [Config(IsCanImportExport = true, Name = "忽略应用", Description = "可以通过进程名称或者正则表达式进行匹配。当使用正则表达式时可以匹配程序路径", Group = "忽略应用", Placeholder = "进程名称，不需要输入.exe。支持正则表达式")]
        /// <summary>
        /// 忽略的进程列表
        /// </summary>
        public List<string> IgnoreProcessList { get; set; } = new List<string>();

        [Config(IsCanImportExport = true, Name = "忽略URL", Description = "过滤不需要统计的网站或链接", Group = "忽略URL", Placeholder = "URL 支持正则表达式")]
        /// <summary>
        /// 忽略的进程列表
        /// </summary>
        public List<string> IgnoreURLList { get; set; } = new List<string>();

        [Config(Name = "应用白名单", Description = "仅统计白名单内的应用", Group = "应用白名单")]
        /// <summary>
        /// 睡眠监测
        /// </summary>
        public bool IsWhiteList { get; set; } = false;
        [Config(IsCanImportExport = true, Name = "应用白名单", Description = "可以通过进程名称或者正则表达式进行匹配，当使用正则表达式时可以匹配程序路径", Group = "应用白名单", Placeholder = "进程名称，不需要输入.exe。支持正则表达式")]
        /// <summary>
        /// 应用白名单
        /// </summary>
        public List<string> ProcessWhiteList { get; set; } = new List<string>();
    }
}
