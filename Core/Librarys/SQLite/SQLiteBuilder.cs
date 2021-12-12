using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys.SQLite
{
    /// <summary>
    /// 数据模型信息接口
    /// </summary>
    public class IModelInfo
    {
        /// <summary>
        /// 模型名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模型数据库表名
        /// </summary>
        public string TableName { get { return Name + "s"; } }
        /// <summary>
        /// 获取模型所有属性
        /// </summary>
        public List<IModelProperties> Properties { get; set; }
    }
    /// <summary>
    /// 模型属性接口
    /// </summary>
    public class IModelProperties
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }
        public string Type { get; set; }
        public bool PK { get; set; }
        public bool NotNull { get { return !PK; } }
    }
    /// <summary>
    ///handle sqlite code first
    /// </summary>
    public class SQLiteBuilder
    {
        private System.Data.Entity.Infrastructure.DbModel model;
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string connstr;
        /// <summary>
        /// 数据库版本文件
        /// </summary>
        private string versionFile;
        /// <summary>
        /// 数据模型
        /// </summary>
        private System.Data.Entity.Core.Metadata.Edm.EdmModel storeModel;
        private System.Data.Entity.Core.Common.DbProviderManifest providerManifest;
        public SQLiteBuilder(System.Data.Entity.Infrastructure.DbModel model)
        {
            this.model = model;

            if (model != null)
            {
                connstr = model.ProviderInfo.ProviderManifestToken;

                storeModel = model.StoreModel;

                providerManifest = model.ProviderManifest;

                versionFile = connstr.Split('=')[1] + ".version";
            }
        }

        public void Handle()
        {
            if (IsHandleDb())
            {
                HandleTable();
            }
        }

        /// <summary>
        /// 处理表变化（仅处理新增表、新增表字段。不处理表名更改、列名更改、删除）
        /// </summary>
        private void HandleTable()
        {
            var modelInfos = GetModelInfos();

            var modelInfosDb = GetModelInfosForDb();

            foreach (var model in modelInfos)
            {
                //表名
                string tableName = model.TableName;

                var modelInfoDb = modelInfosDb.Where(m => m.TableName == tableName);
                //判断表是否存在
                if (modelInfoDb.Count() <= 0)
                {
                    //不存在则重新创建表
                    string csql = GetCreateTableSQL(model);
                    ExecuteNonQuery(csql);
                }
                else
                {
                    //存在则扫描新字段（*暂时不处理字段的属性变化，只处理新增）

                    //对应的数据库模型信息
                    var modelDb = modelInfoDb.Single();
                    //查找差异属性
                    foreach (var item in model.Properties)
                    {
                        if (modelDb.Properties.Where(m => m.Name == item.Name).Count() <= 0)
                        {
                            string csql = GetCreateColumnSQL(tableName, item);
                            ExecuteNonQuery(csql);
                        }
                    }

                }

            }
        }

        #region 是否需要处理
        private bool IsHandleDb()
        {
            string dir = Path.GetDirectoryName(versionFile);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            bool f = File.Exists(versionFile);
            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (f)
            {
                //存在版本文件对比是否需要更新
                string dbVersion = File.ReadAllText(versionFile);
                if (dbVersion != appVersion)
                {
                    //版本不一致需要更新
                    File.WriteAllText(versionFile, appVersion);
                    return true;
                }
                //不需要更新
                return false;
            }

            File.WriteAllText(versionFile, appVersion);
            return true;
        }

        #endregion

        #region 生成创建列SQL语句
        /// <summary>
        /// 生成创建列SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="properties">属性</param>
        /// <returns></returns>
        private string GetCreateColumnSQL(string tableName, IModelProperties properties)
        {
            string typeDefault = properties.NotNull ? properties.Type == "int" ? "NOT NULL DEFAULT 0" : "NOT NULL DEFAULT ''" : "";
            return $"ALTER table {tableName} ADD COLUMN  [{properties.Name}] {properties.Type} {typeDefault}";
        }
        #endregion

        #region 生成创建表SQL语句
        /// <summary>
        /// 生成创建表SQL语句
        /// </summary>
        /// <param name="model">模型信息</param>
        /// <returns></returns>
        private string GetCreateTableSQL(IModelInfo model)
        {
            string sql = $"CREATE TABLE \"{model.TableName}\" (";
            foreach (var p in model.Properties)
            {
                string add = p.PK ? $"{p.Type} PRIMARY KEY" : p.Type + (p.NotNull ? " NOT NULL" : "");
                string addChar = p == model.Properties[0] ? "" : ",";
                sql += $"{addChar}[{p.Name}] {add}";
            }
            sql += ")";
            return sql;
        }
        #endregion

        #region 判断表是否存在数据库中
        /// <summary>
        /// 判断表是否存在数据库中
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool IsTableExistDb(string tableName)
        {
            using (var con = new SQLiteConnection(@"Data Source=.\data.db"))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = "select count(*)  from sqlite_master where type='table' and name = 'StatisticModels3'";
                    var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        var count = rd.GetInt32(0);
                        return count > 0;
                    }
                    return false;
                }

            }
        }

        #endregion

        #region 获取所有数据模型的信息（从代码）
        /// <summary>
        /// 获取所有数据模型的信息（从代码）
        /// </summary>
        /// <returns></returns>
        private List<IModelInfo> GetModelInfos()
        {
            var result = new List<IModelInfo>();

            foreach (var model in storeModel.EntityTypes)
            {
                var i = new IModelInfo();
                var ps = new List<IModelProperties>();

                i.Name = model.Name;
                i.Properties = ps;
                //属性获取
                foreach (var p in model.Properties)
                {
                    ps.Add(new IModelProperties()
                    {
                        Name = p.Name,
                        PK = p.IsStoreGeneratedIdentity,
                        Type = p.TypeName == "int" && p.IsStoreGeneratedIdentity ? "INTEGER" : p.TypeName
                    });
                }
                result.Add(i);
            }
            return result;
        }
        #endregion

        #region 获取所有数据模型的信息（从数据库）
        /// <summary>
        /// 获取所有数据模型的信息（从数据库）
        /// </summary>
        /// <returns></returns>
        private List<IModelInfo> GetModelInfosForDb()
        {
            var result = new List<IModelInfo>();

            using (var con = new SQLiteConnection(connstr))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = "select name from sqlite_master where type='table' order by name";
                    using (var rd = cmd.ExecuteReader())
                    {


                        while (rd.Read())
                        {
                            var modelInfo = new IModelInfo();
                            modelInfo.Name = rd["name"].ToString().Substring(0, rd["name"].ToString().Length - 1);
                            modelInfo.Properties = GetModelProperties(modelInfo.TableName);
                            result.Add(modelInfo);
                        }
                    }
                }

            }

            return result;
        }
        #endregion

        #region 获取表的所有属性（从数据库）
        private List<IModelProperties> GetModelProperties(string tableName)
        {
            var result = new List<IModelProperties>();

            using (var con = new SQLiteConnection(connstr))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = $"PRAGMA table_info([{tableName}])";
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var mp = new IModelProperties();
                            mp.Name = rd["name"].ToString();
                            mp.Type = rd["type"].ToString();
                            mp.PK = rd["pk"].ToString() == "1";
                            result.Add(mp);
                        }
                    }
                }
            }
            return result;
        }
        #endregion


        //sqlite帮助
        /// <summary>
        /// 执行一条sql语句，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private int ExecuteNonQuery(string sql)
        {
            using (var con = new SQLiteConnection(connstr))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = sql;
                    return cmd.ExecuteNonQuery();
                }

            }
        }
    }

}