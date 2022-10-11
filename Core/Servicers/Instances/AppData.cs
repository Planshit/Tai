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
    public class AppData : IAppData
    {

        private List<AppModel> _apps;


        public List<AppModel> GetAllApps()
        {
            return _apps;
        }

        public void Load()
        {
            Debug.WriteLine("加载app开始");
            using (var db = new TaiDbContext())
            {
                _apps = (
                    from app in db.App
                    join c in db.Categorys
                    on app.CategoryID equals c.ID into itdata
                    from n in itdata.DefaultIfEmpty()
                    select app
                    ).ToList()
                    .Select(m => new AppModel
                    {
                        ID = m.ID,
                        Category = m.Category != null ? m.Category : null,
                        CategoryID = m.CategoryID,
                        Description = m.Description,
                        File = m.File,
                        IconFile = m.IconFile,
                        Name = m.Name,
                        TotalTime = m.TotalTime
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// 更新app数据，要先调用GetApp获得后更改并传回才有效
        /// </summary>
        /// <param name="app"></param>
        public void UpdateApp(AppModel app)
        {
            using (var db = new TaiDbContext())
            {
                db.Entry(app).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        public AppModel GetApp(string name)
        {
            return _apps.Where(m => m.Name == name).FirstOrDefault();
        }
        public AppModel GetApp(int id)
        {
            return _apps.Where(m => m.ID == id).FirstOrDefault();
        }
        public void AddApp(AppModel app)
        {
            if (_apps.Where(m => m.Name == app.Name).Any())
            {
                return;
            }

            using (var db = new TaiDbContext())
            {
                db.App.Add(app);
                int res = db.SaveChanges();
                if (res > 0)
                {
                    _apps.Add(app);
                }
            }
        }


        public List<AppModel> GetAppsByCategoryID(int categoryID)
        {
            return _apps.Where(m => m.CategoryID == categoryID).ToList();
        }
    }
}
