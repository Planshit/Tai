using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// app状态
    /// </summary>
    public class AppState
    {
        /// <summary>
        /// 是否正在加载中
        /// </summary>
        public static bool IsLoading { get; set; } = true;
        /// <summary>
        /// 进度值
        /// </summary>
        public static int ProcessValue { get; set; } = 0;

        public static string ActionText { get; set; } = "加载中，请稍后...";
        
        /// <summary>
        /// 设置操作文本（本地化）
        /// </summary>
        /// <param name="text">本地化键</param>
        public static void SetActionText(string text)
        {
            ActionText = text;
        }
    }
}
