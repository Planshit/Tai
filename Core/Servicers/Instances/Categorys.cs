using Core.Librarys.SQLite;
using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Core.Servicers.Instances
{
    public class Categorys : ICategorys
    {
        private List<CategoryModel> _categories;
        public Categorys()
        {
            this._categories = new List<CategoryModel>();
        }
     
        public CategoryModel Create(CategoryModel category)
        {
            using (var db = new TaiDbContext())
            {
                db.Categorys.Add(category);
                db.SaveChanges();
                _categories.Add(category);
                return category;
            }
        }

        public void Delete(CategoryModel category)
        {
            using (var db = new TaiDbContext())
            {
                var item = db.Categorys.Where(m => m.ID == category.ID).FirstOrDefault();
                if (item != null)
                {
                    db.Categorys.Remove(item);
                    db.SaveChanges();
                    _categories.Remove(category);
                }
            }

        }

        public List<CategoryModel> GetCategories()
        {
            return this._categories;
        }

        public CategoryModel GetCategory(int id)
        {
            return _categories.Where(m => m.ID == id).FirstOrDefault();
        }

        public void Load()
        {
            Debug.WriteLine("加载分类");
            using (var db = new TaiDbContext())
            {
                this._categories = db.Categorys.ToList();
                Debug.WriteLine("加载分类完成");

            }
        }


        public void Update(CategoryModel category)
        {
            using (var db = new TaiDbContext())
            {
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
