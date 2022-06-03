using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    /// <summary>
    /// 每小时的使用数据
    /// </summary>
    public class HoursLogModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 统计时段（YYYY-MM-dd HH:00:00)
        /// </summary>
        public DateTime DataTime { get; set; }
        /// <summary>
        /// 使用时长（单位：秒）
        /// </summary>
        public int Time { get; set; } = 0;
        ///// <summary>
        ///// 【已弃用】进程名称
        ///// </summary>
        //public string ProcessName { get; set; }

        ///// <summary>
        ///// 【已弃用】进程文件（记录以防止进程名称重复）
        ///// </summary>
        //public string File { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppModelID { get; set; }

        public virtual AppModel AppModel { get; set; }

    }
}
