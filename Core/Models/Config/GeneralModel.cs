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
        /// <summary>
        /// 是否启用开机自启动
        /// </summary>
        [Config(
            Name = "config.general.isStartAtBoot.name",
            Description = "config.general.isStartAtBoot.description",
            Group = "config.general.group.basic")]
        public bool IsStartatboot { get; set; }

        /// <summary>
        /// 主题模式
        /// </summary>
        [Config(
            Options = "config.general.theme.options",
            Name = "config.general.theme.name",
            Description = "config.general.theme.description",
            Group = "config.general.group.appearance")]
        public int Theme { get; set; } = 0;

        /// <summary>
        /// 主题颜色
        /// </summary>
        [Config(
            Name = "config.general.themeColor.name",
            Description = "config.general.themeColor.description",
            Group = "config.general.group.appearance")]
        public string ThemeColor { get; set; } = "#2b20d9";

        /// <summary>
        /// 是否保留界面大小
        /// </summary>
        [Config(
            Name = "config.general.isSaveWindowSize.name",
            Description = "config.general.isSaveWindowSize.description",
            Group = "config.general.group.appearance")]
        public bool IsSaveWindowSize { get; set; } = false;

        /// <summary>
        /// 窗口宽度
        /// </summary>
        public double WindowWidth { get; set; } = 815;

        /// <summary>
        /// 窗口高度
        /// </summary>
        public double WindowHeight { get; set; } = 585;

        /// <summary>
        /// 启动页选择
        /// </summary>
        [Config(
            Options = "config.general.startPage.options",
            Name = "config.general.startPage.name",
            Description = "config.general.startPage.description",
            Group = "config.general.group.basic")]
        public int StartPage { get; set; } = 0;

        /// <summary>
        /// 启动软件时显示主界面
        /// </summary>
        [Config(
            Name = "config.general.isStartupShowMainWindow.name",
            Description = "config.general.isStartupShowMainWindow.description",
            Group = "config.general.group.basic")]
        public bool IsStartupShowMainWindow { get; set; } = true;

        /// <summary>
        /// 概览页最为频繁显示条数
        /// </summary>
        [Config(
            Options = "config.general.indexPageFrequentUseNum.options",
            Name = "config.general.indexPageFrequentUseNum.name",
            Description = "config.general.indexPageFrequentUseNum.description",
            Group = "config.general.group.overview")]
        public int IndexPageFrequentUseNum { get; set; } = 2;

        /// <summary>
        /// 概览页最为频繁显示条数
        /// </summary>
        [Config(
            Options = "config.general.indexPageMoreNum.options",
            Name = "config.general.indexPageMoreNum.name",
            Description = "config.general.indexPageMoreNum.description",
            Group = "config.general.group.overview")]
        public int IndexPageMoreNum { get; set; } = 11;

        /// <summary>
        /// 是否启用网站记录功能
        /// </summary>
        [Config(
            Name = "config.general.isWebEnabled.name",
            Description = "config.general.isWebEnabled.description",
            Group = "config.general.group.feature")]
        public bool IsWebEnabled { get; set; } = false;
    }
}