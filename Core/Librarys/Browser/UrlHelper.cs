using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Librarys.Browser
{
    public static class UrlHelper
    {
        private static readonly Dictionary<string, string> _domainNamesDict = new Dictionary<string, string>()
        {
            {"www.google.com","Google" },
            {"github.com","Github" },
            {"translate.google.com","Google 翻译" },
            {"v2ex.com","V2EX" },
            {"www.bilibili.com","哔哩哔哩" },
        };
        /// <summary>
        /// 获取链接中的域名部分
        /// </summary>
        /// <param name="url_">链接</param>
        /// <param name="isRemovePH_">是否移除协议头(默认否)</param>
        /// <returns></returns>
        public static string GetDomain(string url_, bool isRemovePH_ = true)
        {
            if (string.IsNullOrEmpty(url_)) { return url_; }

            ChromeURLEncode(url_);

            var ph = Regex.Match(url_, @"((https|http|ftp|rtsp|mms)?:\/\/)").Value;
            if (!string.IsNullOrEmpty(ph))
            {
                url_ = url_.Replace(ph, string.Empty);
            }

            int index = url_.IndexOf("/");
            if (index != -1)
            {
                url_ = url_.Substring(0, index);
            }

            return !isRemovePH_ ? ph + url_ : url_;
        }

        /// <summary>
        /// 通过链接获取一个名称
        /// </summary>
        /// <param name="url_"></param>
        /// <returns></returns>
        public static string GetName(string url_)
        {
            string result = url_;
            //  获取域名部分
            result = GetDomain(result);

            //  尝试从已存储的列表中搜索
            if (_domainNamesDict.ContainsKey(result))
            {
                return _domainNamesDict[result];
            }

            var arr = result.Split('.');

            //  移除的字符串集合
            string[] removeStrArr = { "www" };

            if (arr.Length == 2)
            {
                return FirstLetterToUpper(arr[0]);
            }
            else if (arr.Length >= 3)
            {
                return removeStrArr.Contains(arr[0]) ? FirstLetterToUpper(arr[1]) : $"{FirstLetterToUpper(arr[1])} {FirstLetterToUpper(arr[0])}";
            }
            return FirstLetterToUpper(result);
        }

        /// <summary>
        /// 将英文的首字母转为大写
        /// </summary>
        /// <param name="str_"></param>
        /// <returns></returns>
        private static string FirstLetterToUpper(string str_)
        {
            if (string.IsNullOrEmpty(str_)) return str_;

            return str_[0].ToString().ToUpper() + str_.Substring(1);
        }
        /// <summary>
        /// 判断链接是否是域名主页
        /// </summary>
        /// <param name="url_"></param>
        /// <returns></returns>
        public static bool IsIndexUrl(string url_)
        {
            url_ = url_.Replace("http://", string.Empty);
            url_ = url_.Replace("https://", string.Empty);
            return url_.IndexOf("/") == -1 || url_.Last() == '/';
        }

        /// <summary>
        /// 谷歌浏览器URL编码
        /// </summary>
        /// <param name="url_"></param>
        /// <returns></returns>
        public static string ChromeURLEncode(string url_)
        {
            //  韩语
            url_ = Regex.Replace(url_, @"[가-힣]+", (e) =>
            {
                var result = WebUtility.UrlEncode(e.Value);
                return result;
            });
            //  泰语
            url_ = Regex.Replace(url_, @"[\u0E00-\u0E7F]", (e) =>
            {
                var result = WebUtility.UrlEncode(e.Value);
                return result;
            });
            //  德语
            url_ = Regex.Replace(url_, @"[äöüÄÖÜß]", (e) =>
            {
                var result = WebUtility.UrlEncode(e.Value);
                return result;
            });
            //  俄语
            url_ = Regex.Replace(url_, @"[ЁёА-я]", (e) =>
            {
                var result = WebUtility.UrlEncode(e.Value);
                return result;
            });
            //  中文标点符号
            url_ = Regex.Replace(url_, @"[\u3002|\uff1f|\uff01|\uff0c|\u3001|\uff1b|\uff1a|\u201c|\u201d|\u2018|\u2019|\uff08|\uff09|\u300a|\u300b|\u3008|\u3009|\u3010|\u3011|\u300e|\u300f|\u300c|\u300d|\ufe43|\ufe44|\u3014|\u3015|\u2026|\u2014|\uff5e|\ufe4f|\uffe5]", (e) =>
            {
                var result = WebUtility.UrlEncode(e.Value);
                return result;
            });
            return Regex.Replace(url_, @"/[\u4e00-\u9fa5]+|[一-龠]+|[ぁ-ゔ]+|[ァ-ヴー]+|[ａ-ｚＡ-Ｚ０-９]+|[々〆〤ヶ]+/u", (e) =>
             {
                 var result = WebUtility.UrlEncode(e.Value);
                 return result;
             });

        }
    }
}
