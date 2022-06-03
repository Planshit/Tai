using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    /// <summary>
    /// 每日使用数据
    /// </summary>
    public class DailyLogModel
    {
        public int ID { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 使用时长（单位：秒）
        /// </summary>
        public int Time { get; set; } = 0;
        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppModelID { get; set; }
        ///// <summary>
        ///// 进程名称
        ///// </summary>
        //public string ProcessName { get; set; }
        ///// <summary>
        ///// 进程描述
        ///// </summary>
        //public string ProcessDescription { get; set; }
        ///// <summary>
        ///// 进程文件
        ///// </summary>
        //public string File { get; set; }
        public virtual AppModel AppModel { get; set; }
    }
}
