using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Db
{
    /// <summary>
    /// 网页浏览记录
    /// </summary>
    public class WebBrowseLogModel
    {
        [Key]
        public int ID { get; set; }
        public int UrlId { get; set; }
        [ForeignKey("UrlId")]
        public virtual WebUrlModel Url { get; set; }
        /// <summary>
        /// 统计时段（YYYY-MM-dd HH:00:00)
        /// </summary>
        public DateTime LogTime { get; set; }
        /// <summary>
        /// 使用时长（单位：秒）
        /// </summary>
        public int Duration { get; set; } = 0;
        /// <summary>
        /// 网站ID
        /// </summary>
        public int SiteId { get; set; }
        [ForeignKey("SiteId")]
        public virtual WebSiteModel Site { get; set; }
    }
}
