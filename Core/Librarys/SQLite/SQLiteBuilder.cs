using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
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
        public DbType Type { get; set; }
        public bool PK { get; set; }
        //public bool NotNull { get { return !PK; } }
    }

    public enum DbType
    {
        INTEGER,
        datetime,
        nvarchar,
        @int
    }

    /// <summary>
    ///handle sqlite code first
    /// </summary>
    public class SQLiteBuilder
    {
        public static bool IsSelfChecking = true;

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
        /// 数据库文件路径
        /// </summary>
        private string dbFile;
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

                dbFile = connstr.Split('=')[1];

                versionFile = $"{dbFile}.version";

            }
        }

        /// <summary>
        /// 执行数据库自检
        /// </summary>
        public bool SelfCheck()
        {
            if (IsHandleDb())
            {
                bool res = HandleTable();

                IsSelfChecking = false;

                return res;
            }

            IsSelfChecking = false;
            
            return true;
        }

        /// <summary>
        /// 处理表变化（仅处理新增表、新增表字段、表结构变化）
        /// </summary>
        private bool HandleTable()
        {
            try
            {
                //  处理前先复制一份数据库文件以防万一
                string dir = Path.Combine(Path.GetDirectoryName(dbFile), "backup");
                string backupName = Path.Combine(dir, $"data.handle.backup");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(backupName))
                {
                    File.Delete(backupName);
                }
                File.Copy(dbFile, backupName);

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

                HandleVersion1002Migrate();


                //  处理表字段删除
                foreach (var dbModel in modelInfosDb)
                {
                    //  表名
                    string tableName = dbModel.TableName;

                    //  查找对应的实体模型
                    var model = modelInfos.Where(m => m.TableName == tableName).FirstOrDefault();

                    if (model == null)
                    {
                        //  已删除的模型就从数据库中删除
                        //ExecuteNonQuery($"DROP TABLE {tableName}");// 暂时不处理
                    }
                    else
                    {
                        //  遍历数据库表的属性

                        bool isDel = false;



                        foreach (var dbItem in dbModel.Properties)
                        {
                            //  判断实体模型是否有字段，没有的话需要重建表
                            if (!model.Properties.Where(m => m.Name == dbItem.Name).Any())
                            {
                                isDel = true;
                                break;
                            }


                        }
                        if (isDel)
                        {
                            //  模型字段
                            string field = string.Join(",", model.Properties.Select(m => m.Name).ToArray());

                            //  临时表名
                            string tempName = $"{tableName}_temp_{DateTime.Now.ToString("yyyyMMddHHmmss")}";

                            //  创建一个新表
                            ExecuteNonQuery(GetCreateTableSQL(model, tempName));

                            ////  转移数据
                            ExecuteNonQuery($"INSERT INTO {tempName}({field}) SELECT {field} FROM {tableName}");


                            //  删除旧表
                            ExecuteNonQuery($"drop table {tableName}");


                            //  重新命名临时表
                            ExecuteNonQuery($"alter table {tempName} rename to {tableName}");
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"SQLiteBuilder handle fail!Exception Message:{ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// 数据迁移，1.0.0.2升级1.0.0.3时需要转移表数据
        /// </summary>
        private void HandleVersion1002Migrate()
        {
            //if (ExecuteNonQuery("select count(*) from sqlite_master where type='table' and name='AppModels'") != 0)
            //{
            //    return;
            //}
            ExecuteNonQuery($"insert into AppModels(ID,Name,Description,File,TotalTime) select null,ProcessName as Name,ProcessDescription as Description,File,sum(Time) as TotalTime from DailyLogModels where 1=1 GROUP BY Name");

            using (var con = new SQLiteConnection(connstr))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = $"select ID,Name,Description from AppModels GROUP BY Name";

                    List<string> sqlList = new List<string>();

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var ID = rd[0];
                            var Name = rd[1].ToString();
                            var Description = rd[2].ToString();

                            var icon = Iconer.Get(Name, Description);

                            sqlList.Add($"update DailyLogModels set AppModelID={ID} where ProcessName='{Name}'");
                            sqlList.Add($"update HoursLogModels set AppModelID={ID} where ProcessName='{Name}'");


                            sqlList.Add($"update AppModels set IconFile='{icon}' where ID={ID}");
                        }
                    }

                    cmd.CommandText = string.Join("; ", sqlList.ToArray());
                    cmd.ExecuteNonQuery();
                }
            }

        }

        #region 是否需要处理
        /// <summary>
        /// 判断是否需要处理数据，比对core程序集版本号
        /// </summary>
        /// <returns></returns>
        private bool IsHandleDb()
        {
            string dir = Path.GetDirectoryName(versionFile);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            bool f = File.Exists(versionFile);

            //  core程序集版本号
            string coreVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (f)
            {
                //存在版本文件对比是否需要更新
                string dbVersion = File.ReadAllText(versionFile);
                if (dbVersion != coreVersion)
                {
                    //版本不一致需要更新
                    File.WriteAllText(versionFile, coreVersion);
                    return true;
                }
                //不需要更新
                return false;
            }

            File.WriteAllText(versionFile, coreVersion);
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
            string typeDefault = !properties.PK ? properties.Type == DbType.@int ? "NULL DEFAULT 0" : "NULL DEFAULT ''" : "";

            if (properties.PK)
            {
                properties.Type = DbType.INTEGER;
            }
            return $"ALTER table {tableName} ADD COLUMN  [{properties.Name}] {properties.Type} {typeDefault}";
        }
        #endregion

        #region 生成创建表SQL语句
        /// <summary>
        /// 生成创建表SQL语句
        /// </summary>
        /// <param name="model">模型信息</param>
        /// <returns></returns>
        private string GetCreateTableSQL(IModelInfo model, string tableName = null)
        {
            tableName = string.IsNullOrEmpty(tableName) ? model.TableName : tableName;

            string sql = $"CREATE TABLE \"{tableName}\" (";
            foreach (var p in model.Properties)
            {
                string typeDefault = p.Type == DbType.@int ? " NULL DEFAULT 0" : " NULL DEFAULT ''";
                if (p.PK)
                {
                    typeDefault = "";
                    p.Type = DbType.INTEGER;
                }

                string add = p.PK ? $"{p.Type} PRIMARY KEY" : p.Type.ToString();
                string addChar = p == model.Properties[0] ? "" : ",";
                sql += $"{addChar}[{p.Name}] {add}{typeDefault}";
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
                    Enum.TryParse(p.TypeName, out DbType dbType);

                    ps.Add(new IModelProperties()
                    {
                        Name = p.Name,
                        PK = p.IsStoreGeneratedIdentity,
                        Type = dbType
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
                            Enum.TryParse(rd["type"].ToString(), out DbType dbType);
                            mp.Type = dbType;
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