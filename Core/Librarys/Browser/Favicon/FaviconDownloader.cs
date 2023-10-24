using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys.Browser.Favicon
{
    public class FaviconDownloader
    {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url_">文件远程地址</param>
        /// <param name="savePath_">本地保存路径</param>
        public static async Task<string> DownloadAsync(string url_, string saveName_)
        {
            if (string.IsNullOrEmpty(url_)) return string.Empty;

            string savePath = Path.Combine(FileHelper.GetRootDirectory(), "WebFavicons", saveName_);
            string dir = Path.GetDirectoryName(savePath);
            //  暂时不支持svg格式ico，尝试获取png
            url_ = url_.Replace(".svg", ".png");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(savePath))
            {
                return savePath;
            }
            try
            {
                using (var web = new WebClient())
                {
                    await web.DownloadFileTaskAsync(url_, savePath);
                    return savePath;
                }
            }
            catch (Exception e)
            {
                Logger.Error("下载图标失败，" + e);
                return string.Empty;
            }
        }
    }
}
