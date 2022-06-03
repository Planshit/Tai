using Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys.SQLite
{
    public class StatisticContext : DbContext
    {
        /// <summary>
        /// 每日数据
        /// </summary>
        public DbSet<DailyLogModel> DailyLog { get; set; }
        /// <summary>
        /// 时段数据
        /// </summary>
        public DbSet<HoursLogModel> HoursLog { get; set; }
        public DbSet<AppModel> App { get; set; }

        // public StatisticContext(string n)
        //: base("StatisticContext")
        // {
        //     DbConfiguration.SetConfiguration(new SQLiteConfiguration());
        // }
        public StatisticContext()
       : base(new SQLiteConnection()
       {
           ConnectionString = "Data Source=.\\Data\\data.db"
       }, true)
        {
            DbConfiguration.SetConfiguration(new SQLiteConfiguration());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var model = modelBuilder.Build(Database.Connection);
            new SQLiteBuilder(model).SelfCheck();
        }
    }
}