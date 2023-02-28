using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys.Browser.Favicon
{
    public interface IFaviconManager
    {
        /// <summary>
        /// 保存图标数据到Tai目录下并返回图标相对路径
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> SaveToLocalIconAsync(object data);
        /// <summary>
        /// 通过URL链接获取Favicon图标数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<object> GetIconDataAsync(string url);
    }
}
