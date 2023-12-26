using Core.Librarys;
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

        private readonly IDatabase _databse;
        private readonly object _locker = new object();
        public AppData(IDatabase databse)
        {
            _databse = databse;
        }
        public List<AppModel> GetAllApps()
        {
            return _apps;
        }

        public void Load()
        {
            Debug.WriteLine("加载app开始");
            using (var db = _databse.GetReaderContext())
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
                        Alias = m.Alias,
                        TotalTime = m.TotalTime
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// 更新app数据，要先调用GetApp获得后更改并传回才有效
        /// </summary>
        /// <param name="app"></param>
        public void UpdateApp(AppModel app_)
        {
            try
            {
                using (var db = _databse.GetWriterContext())
                {
                    var app = db.App.FirstOrDefault(c => c.ID.Equals(app_.ID));
                    if (app != null)
                    {
                        app.TotalTime = app_.TotalTime;
                        app.IconFile = app_.IconFile;
                        app.Name = app_.Name;
                        app.Description = app_.Description;
                        app.File = app_.File;
                        app.CategoryID = app_.CategoryID;
                        app.Alias = app_.Alias;
                        db.SaveChanges();
                    }
                    _databse.CloseWriter();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }
        public AppModel GetApp(string name)
        {
            lock (_locker)
            {
                return _apps.Where(m => m.Name == name).FirstOrDefault();
            }
        }
        public AppModel GetApp(int id)
        {
            lock (_locker)
            {
                return _apps.Where(m => m.ID == id).FirstOrDefault();
            }
        }
        public void AddApp(AppModel app)
        {
            lock (_locker)
            {
                if (_apps.Where(m => m.Name == app.Name).Any())
                {
                    return;
                }
                try
                {
                    using (var db = _databse.GetWriterContext())
                    {
                        var r = db.App.Add(app);
                        int res = db.SaveChanges();
                        if (res > 0)
                        {
                            Debug.WriteLine("add done!" + r.ID);

                            _apps.Add(app);
                        }
                        _databse.CloseWriter();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }
        }


        public List<AppModel> GetAppsByCategoryID(int categoryID)
        {
            lock (_locker)
            {
                return _apps.Where(m => m.CategoryID == categoryID).ToList();
            }
        }
    }
}
