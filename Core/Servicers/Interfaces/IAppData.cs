using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface IAppData
    {
        /// <summary>
        /// 更新app数据，要先调用GetApp获得后更改并传回才有效
        /// </summary>
        void UpdateApp(AppModel app);
        /// <summary>
        /// 保存app数据更改
        /// </summary>
        //void SaveAppChanges();
        /// <summary>
        /// 通过进程名称获取app
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        AppModel GetApp(string name);
        AppModel GetApp(int id);
        /// <summary>
        /// 获取所有app
        /// </summary>
        /// <returns></returns>
        List<AppModel> GetAllApps();
        void AddApp(AppModel app);
        /// <summary>
        /// 加载已存储的app列表，仅建议在启动时调用一次，无必要请勿再次调用
        /// </summary>
        void Load();
        /// <summary>
        /// 获取app列表通过分类ID
        /// </summary>
        /// <param name="categoryID">分类ID</param>
        /// <returns></returns>
        List<AppModel> GetAppsByCategoryID(int categoryID);
    }
}
