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

        /// <summary>
        /// 有数据更新的app
        /// </summary>
        private List<AppModel> _updateTempApps;
        public AppData()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 30, 0);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();

            _apps = new List<AppModel>();
        }


        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            SaveAppChanges();
        }
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

            _updateTempApps = new List<AppModel>();
        }

        /// <summary>
        /// 更新app数据，要先调用GetApp获得后更改并传回才有效
        /// </summary>
        /// <param name="app"></param>
        public void UpdateApp(AppModel app)
        {
            if (!_updateTempApps.Where(m => m.ID == app.ID).Any())
            {
                _updateTempApps.Add(app);
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

        public void SaveAppChanges()
        {

            using (var db = new TaiDbContext())
            {
                foreach (var item in _updateTempApps)
                {
                    var app = db.App.Find(item.ID);
                    if (app != null)
                    {
                        app.TotalTime = item.TotalTime;
                        app.File = item.File;
                        app.IconFile = item.IconFile;
                        app.Name = item.Name;
                        app.CategoryID = item.CategoryID;
                        app.Description = item.Description;
                        //app.Category = item.Category;
                    }
                }
                db.SaveChanges();
                _updateTempApps.Clear();
            }

        }

        public List<AppModel> GetAppsByCategoryID(int categoryID)
        {
            return _apps.Where(m => m.CategoryID == categoryID).ToList();
        }
    }
}
