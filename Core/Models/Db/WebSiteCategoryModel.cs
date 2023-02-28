using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Db
{
    /// <summary>
    /// 网站分类数据库模型
    /// </summary>
    public class WebSiteCategoryModel
    {

        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分类图标路径
        /// </summary>
        public string IconFile { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
    }
}
