using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    /// <summary>
    /// 网站过滤器
    /// </summary>
    public interface IWebFilter
    {
        /// <summary>
        /// 初始化过滤器
        /// </summary>
        void Init();
        /// <summary>
        /// 指示输入的URL是否已被忽略
        /// </summary>
        /// <param name="url_">URL</param>
        /// <returns>已被忽略返回True</returns>
        bool IsIgnore(string url_);
        /// <summary>
        /// 指示输入的URL是否被域名配置忽略
        /// </summary>
        /// <param name="url_">URL</param>
        /// <returns>已被忽略返回True</returns>
        bool IsDomainIgnore(string url_);
        /// <summary>
        /// 指示输入的URL是否被正则表达式匹配忽略
        /// </summary>
        /// <param name="url_">URL</param>
        /// <returns>已被忽略返回True</returns>
        bool IsRegexIgnore(string url_);

    }
}
