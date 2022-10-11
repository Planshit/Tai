using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface ICategorys
    {
        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        List<CategoryModel> GetCategories();
        /// <summary>
        /// 加载已存储的分类数据，仅建议在启动时调用一次，无必要请勿再次调用
        /// </summary>
        void Load();
        CategoryModel GetCategory(int id);
        CategoryModel Create(CategoryModel category);
        void Update(CategoryModel category);
        void Delete(CategoryModel category);
    }
}
