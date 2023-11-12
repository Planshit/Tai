using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    /// <summary>
    /// 分类模型
    /// </summary>
    public class CategoryModel
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
        /// <summary>
        /// 是否启用目录匹配
        /// </summary>
        public bool IsDirectoryMath { get; set; } = false;
        /// <summary>
        /// 匹配目录（Json List string）
        /// </summary>
        public string Directories { get; set; }

        /// <summary>
        /// 匹配目录集合（已解析）
        /// </summary>
        [NotMapped]
        public List<string> DirectoryList
        {
            get
            {
                if (string.IsNullOrEmpty(Directories)) return new List<string>();
                return JsonConvert.DeserializeObject<List<string>>(Directories);
            }
        }
    }
}
