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
        /// <summary>
        /// 睡眠监测
        /// </summary>
        [Config(
            Name = "config.behaviorModel.isSleepWatch.name",
            Description = "config.behaviorModel.isSleepWatch.description",
            Group = "config.behaviorModel.group.isSleepWatch")]
        public bool IsSleepWatch { get; set; } = true;

        /// <summary>
        /// 忽略的进程列表
        /// </summary>
        [Config(
            IsCanImportExport = true,
            Name = "config.behaviorModel.ignoreProcessList.name",
            Description = "config.behaviorModel.ignoreProcessList.description",
            Group = "config.behaviorModel.group.ignoreProcess",
            Placeholder = "config.behaviorModel.ignoreProcessList.placeholder")]
        public List<string> IgnoreProcessList { get; set; } = new List<string>();

        /// <summary>
        /// 忽略的 URL 列表
        /// </summary>
        [Config(
            IsCanImportExport = true,
            Name = "config.behaviorModel.ignoreURLList.name",
            Description = "config.behaviorModel.ignoreURLList.description",
            Group = "config.behaviorModel.group.ignoreURL",
            Placeholder = "config.behaviorModel.ignoreURLList.placeholder")]
        public List<string> IgnoreURLList { get; set; } = new List<string>();

        /// <summary>
        /// 睡眠监测
        /// </summary>
        [Config(
            Name = "config.behaviorModel.isWhiteList.name",
            Description = "config.behaviorModel.isWhiteList.description",
            Group = "config.behaviorModel.group.whiteList")]
        public bool IsWhiteList { get; set; } = false;

        /// <summary>
        /// 应用白名单
        /// </summary>
        [Config(
            IsCanImportExport = true,
            Name = "config.behaviorModel.processWhiteList.name",
            Description = "config.behaviorModel.processWhiteList.description",
            Group = "config.behaviorModel.group.whiteList",
            Placeholder = "config.behaviorModel.processWhiteList.placeholder")]
        public List<string> ProcessWhiteList { get; set; } = new List<string>();
    }
}