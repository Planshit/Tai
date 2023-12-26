using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Db
{
    /// <summary>
    /// 网页站点数据库模型
    /// </summary>
    public class WebSiteModel
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 分类ID
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string IconFile { get; set; }
        /// <summary>
        /// 累计使用时长
        /// </summary>
        public int Duration { get; set; } = 0;

        [ForeignKey("CategoryID")]
        public virtual WebSiteCategoryModel Category { get; set; }
    }
}
