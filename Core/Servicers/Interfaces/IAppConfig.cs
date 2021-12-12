using Core.Event;
using Core.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    /// <summary>
    /// 应用配置
    /// </summary>
    public interface IAppConfig
    {
        /// <summary>
        /// 加载配置
        /// </summary>
        void Load();

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        ConfigModel GetConfig();

        /// <summary>
        /// 更新配置
        /// </summary>
        void Save();
        /// <summary>
        /// 配置修改时发生
        /// </summary>
        event AppConfigEventHandler ConfigChanged;
    }
}
