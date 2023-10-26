using Core.Librarys;
using Core.Librarys.Browser;
using Core.Librarys.SQLite;
using Core.Models.Data;
using Core.Models.Db;
using Core.Models.WebPage;
using Core.Servicers.Interfaces;
using CsvHelper;
using Newtonsoft.Json;
using Npoi.Mapper;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace Core.Servicers.Instances
{
    public class WebData : IWebData
    {
        private readonly IDatabase _database;
        private readonly object _createUrlLocker = new object();
        public WebData(IDatabase database)
        {
            _database = database;
        }

        #region AddUrlBrowseTime
        public void AddUrlBrowseTime(Site site_, int duration_, DateTime? dateTime_ = null)
        {
            Debug.WriteLine("AddUrlBrowseTime");
            Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(site_.Url))
                    {
                        return;
                    }

                    var dateTime = dateTime_.HasValue ? dateTime_.Value : DateTime.Now;
                    var logTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
                    if (dateTime.Minute == 59 && dateTime.Second == 59)
                    {
                        AddUrlBrowseTime(site_, duration_, logTime.AddHours(1));
                        return;
                    }
                    //  当前时段时长
                    int nowTimeDuration = duration_;
                    //  下一个时段时长
                    int nextTimeDuration = 0;

                    //  当前时段最大使用时长
                    int nowTimeMaxDuration = (60 - dateTime.Minute) * 60;

                    if (duration_ > 3600)
                    {
                        nowTimeDuration = 3600;
                        nextTimeDuration = duration_ - 3600;
                    }
                    if (nowTimeDuration > nowTimeMaxDuration)
                    {
                        nextTimeDuration += nowTimeDuration - nowTimeMaxDuration;

                        nowTimeDuration = nowTimeMaxDuration;
                    }

                    //var db = _database.GetContext("AddUrlBrowseTime");
                    //using (var db = new TaiDbContext())
                    using (var db = _database.GetWriterContext())
                    {
                        //  获取站点信息
                        string domain = UrlHelper.GetDomain(site_.Url);
                        var site = db.WebSites.Where(m => m.Domain == domain).FirstOrDefault();
                        Debug.WriteLine("AddUrlBrowseTime 获取站点数据");

                        if (site == null)
                        {
                            site = db.WebSites.Add(new Models.Db.WebSiteModel()
                            {
                                Title = UrlHelper.GetName(site_.Url),
                                Domain = domain,
                            });

                            db.SaveChanges();
                        }

                        //  获取链接
                        var url = GetCreateUrl(db, site_);
                        if (url == null)
                        {
                            throw new Exception("在创建URL时异常");
                        }

                        //  记录
                        var log = db.WebBrowserLogs.Where(m => m.LogTime == logTime && m.UrlId == url.ID).FirstOrDefault();

                        if (log != null)
                        {
                            //  时长
                            int duration = log.Duration + nowTimeDuration;
                            //  判断是否超出
                            if (log.Duration + nowTimeDuration > 3600)
                            {
                                duration = 3600;
                                nextTimeDuration += log.Duration + nowTimeDuration - 3600;
                            }

                            //  更新记录
                            log.Duration = duration;
                        }
                        else
                        {
                            //  新记录
                            log = new Models.Db.WebBrowseLogModel()
                            {
                                Duration = nowTimeDuration,
                                UrlId = url.ID,
                                LogTime = logTime,
                                SiteId = site.ID,
                            };


                            db.WebBrowserLogs.Add(log);
                        }
                        site.Duration += nowTimeDuration;
                        db.SaveChanges();
                    }
                    if (nextTimeDuration > 0)
                    {
                        AddUrlBrowseTime(site_, nextTimeDuration, logTime.AddHours(1));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"在更新链接[{site_.Url}]时长[{duration_}]时异常，{ex}");
                }
                finally
                {
                    _database.CloseWriter();
                }
            });

        }
        #endregion

        #region UpdateUrlFavicon
        public void UpdateUrlFavicon(Site site_, string iconFile_)
        {
            if (string.IsNullOrEmpty(iconFile_) || string.IsNullOrEmpty(site_.Url))
            {
                return;
            }

            Debug.WriteLine("UpdateUrlFavicon");

            Task.Run(() =>
            {

                //  判断是否是主域名
                bool isIndex = UrlHelper.IsIndexUrl(site_.Url);

                ////  获取主域名
                //string domain = UrlHelper.GetDomain(site_.Url);

                //  获取站点信息
                using (var db = _database.GetWriterContext())
                {
                    //  更新站点图标
                    var site = GetCreateWebSite(db, site_.Url);
                    Debug.WriteLine("UpdateUrlFavicon 获取站点数据");
                    if (site != null && (string.IsNullOrEmpty(site.IconFile) || (isIndex && site.IconFile != iconFile_)))
                    {
                        site.IconFile = iconFile_;
                    }

                    //  更新链接图标
                    var url = GetCreateUrl(db, site_);
                    if (url != null)
                    {
                        url.IconFile = iconFile_;
                    }

                    db.SaveChanges();
                    _database.CloseWriter();
                }
            });

        }
        #endregion

        #region GetCreateSite
        /// <summary>
        /// 输入URL获取站点信息,不存在时创建
        /// </summary>
        /// <param name="url_">URL</param>
        /// <returns></returns>
        private WebSiteModel GetCreateWebSite(TaiDbContext db, string url_)
        {
            //  获取主域名
            string domain = UrlHelper.GetDomain(url_);

            //using (var db = _database.GetWriterContext())
            //{
            var site = db.WebSites.Where(m => m.Domain == domain).FirstOrDefault();
            if (site == null)
            {
                site = db.WebSites.Add(new WebSiteModel()
                {
                    Title = UrlHelper.GetName(url_),
                    Domain = domain,
                });
                db.SaveChanges();
            }

            return site;
            //}
        }
        #endregion
        #region GetCreateUrl
        /// <summary>
        /// 获取url数据,不存在时创建
        /// </summary>
        /// <param name="url_"></param>
        /// <returns></returns>
        private Models.Db.WebUrlModel GetCreateUrl(TaiDbContext db, Site site_)
        {
            lock (_createUrlLocker)
            {
                var result = db.WebUrls.Where(m => m.Url == site_.Url).FirstOrDefault();
                if (result == null)
                {
                    result = db.WebUrls.Add(new Models.Db.WebUrlModel()
                    {
                        Url = site_.Url,
                        Title = site_.Title,
                    });
                    db.SaveChanges();
                }
                return result;

            }
        }
        #endregion

        #region GetDateRangeWebSiteList
        public List<WebSiteModel> GetDateRangeWebSiteList(DateTime start, DateTime end, int take = 0, int skip = -1, bool isTime_ = false)
        {
            if (isTime_)
            {
                start = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, end.Hour, 59, 59);
            }
            else
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            using (var db = _database.GetReaderContext())
            {
                var data = db.WebBrowserLogs
              .Where(m => m.LogTime >= start && m.LogTime <= end && m.SiteId != 0)
              .GroupBy(m => m.SiteId)
              .Select(s => new
              {
                  Title = s.FirstOrDefault().Site.Title,
                  Domain = s.FirstOrDefault().Site.Domain,
                  CategoryID = s.FirstOrDefault().Site.CategoryID,
                  IconFile = s.FirstOrDefault().Site.IconFile,
                  ID = s.FirstOrDefault().Site.ID,
                  Duration = s.Sum(m => m.Duration),
              });

                if (skip > 0 && take > 0)
                {
                    data = data.OrderByDescending(m => m.Duration).Skip(skip).Take(take);
                }
                else if (skip > 0)
                {
                    data = data.OrderByDescending(m => m.Duration).Skip(skip);
                }
                else if (take > 0)
                {
                    data = data.OrderByDescending(m => m.Duration).Take(take);
                }
                else
                {
                    data = data.OrderByDescending(m => m.Duration);
                }
                var result = JsonConvert.DeserializeObject<List<WebSiteModel>>(JsonConvert.SerializeObject(data.ToList()));
                return result;
            }
        }


        #endregion

        #region GetWebSiteCategories
        public List<WebSiteCategoryModel> GetWebSiteCategories()
        {
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebSiteCategories.ToList();
                return result;
            }
        }


        #endregion

        #region CreateWebSiteCategory
        public WebSiteCategoryModel CreateWebSiteCategory(WebSiteCategoryModel data_)
        {
            using (var db = _database.GetWriterContext())
            {
                var result = db.WebSiteCategories.Add(data_);
                db.SaveChanges();
                _database.CloseWriter();
                return result;
            }
        }
        #endregion

        #region UpdateWebSiteCategory
        public void UpdateWebSiteCategory(WebSiteCategoryModel data_)
        {
            using (var db = _database.GetWriterContext())
            {
                db.Entry(data_).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                _database.CloseWriter();
            }
        }
        #endregion

        #region DeleteWebSiteCategory
        public void DeleteWebSiteCategory(WebSiteCategoryModel data_)
        {
            Task.Run(() =>
            {
                using (var db = _database.GetWriterContext())
                {
                    //  取消网站关联分类
                    db.Database.ExecuteSqlCommand("update WebSiteModels set CategoryID=0 where CategoryID=" + data_.ID);

                    //  删除分类
                    db.Entry(data_).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                    _database.CloseWriter();
                }
            });
        }
        #endregion
        #region GetWebSites
        public List<WebSiteModel> GetWebSites(int categoryId_)
        {
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebSites.Where(m => m.CategoryID == categoryId_).ToList();
                return result;
            }
        }
        #endregion
        #region GetWebSitesCount
        public int GetWebSitesCount(int categoryId_)
        {
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebSites.Where(m => m.CategoryID == categoryId_).Count();
                return result;
            }
        }



        //public List<WebSiteModel> GetWebSites(Expression<Func<WebSiteModel, bool>> predicate_)
        //{
        //    using (var db = new TaiDbContext())
        //    {
        //        return db.WebSites.Where(predicate_).ToList();
        //    }
        //}
        #endregion
        #region GetUnSetCategoryWebSites
        public List<WebSiteModel> GetUnSetCategoryWebSites()
        {
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebSites.Where(m => m.CategoryID == 0).ToList();
                return result;
            }
        }


        #endregion

        public void UpdateWebSitesCategory(int[] siteIds_, int categoryId_)
        {
            Task.Run(() =>
            {
                using (var db = _database.GetWriterContext())
                {
                    string sql = $"update WebSiteModels set CategoryID={categoryId_} where ID in ({string.Join(",", siteIds_)})";
                    db.Database.ExecuteSqlCommand(sql);
                    db.SaveChanges();
                    _database.CloseWriter();
                }
            });
        }

        public List<CommonDataModel> GetCategoriesStatistics(DateTime start_, DateTime end_)
        {
            using (var db = _database.GetReaderContext())
            {
                var sql = "select sum(WebBrowseLogModels.Duration) as Value,WebSiteCategoryModels.ID as ID,WebSiteCategoryModels.Name as Name from WebBrowseLogModels join WebSiteModels on WebBrowseLogModels.SiteId=WebSiteModels.ID JOIN WebSiteCategoryModels on WebSiteModels.CategoryID=WebSiteCategoryModels.ID WHERE WebBrowseLogModels.LogTime>='" + start_.ToString("yyyy-MM-dd 00:00:00") + "' and WebBrowseLogModels.LogTime<= '" + end_.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY WebSiteCategoryModels.ID";

                var data = db.Database.SqlQuery<CommonDataModel>(sql).ToList();
                //  未分类

                sql = "select sum(WebBrowseLogModels.Duration) as Value,WebSiteModels.CategoryID as ID,WebSiteModels.Title as Name from WebBrowseLogModels left join WebSiteModels on WebBrowseLogModels.SiteId=WebSiteModels.ID WHERE WebSiteModels.CategoryID=0 AND WebBrowseLogModels.LogTime>='" + start_.ToString("yyyy-MM-dd 00:00:00") + "' and WebBrowseLogModels.LogTime<= '" + end_.ToString("yyyy-MM-dd 23:59:59") + "' Group BY WebSiteModels.CategoryID";
                var noCategoryData = db.Database.SqlQuery<CommonDataModel>(sql).ToList();
                var result = data.Concat(noCategoryData).ToList();
                return result;
            }
        }

        public WebSiteCategoryModel GetWebSiteCategory(int categoryId_)
        {
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebSiteCategories.Where(m => m.ID == categoryId_).FirstOrDefault();
                return result;
            }
        }

        public List<CommonDataModel> GetBrowseDataStatistics(DateTime start_, DateTime end_, int siteId_ = 0)
        {
            using (var db = _database.GetReaderContext())
            {
                var start = new DateTime(start_.Year, start_.Month, start_.Day, 0, 0, 0);
                var end = new DateTime(end_.Year, end_.Month, end_.Day, 23, 59, 59);
                var dbResult = db.WebBrowserLogs.Where(m => m.LogTime >= start && m.LogTime <= end);
                if (siteId_ > 0)
                {
                    dbResult = dbResult.Where(m => m.SiteId == siteId_);
                }

                var data = dbResult.GroupBy(m => m.LogTime)
                .Select(m => new
                {
                    Value = m.Sum(s => s.Duration),
                    Time = m.FirstOrDefault().LogTime
                }).ToList();

                var result = new List<CommonDataModel>();

                if (start_ == end_)
                {

                    //  获取24小时数据
                    for (int i = 0; i < 24; i++)
                    {
                        var time = new DateTime(start_.Year, start_.Month, start_.Day, i, 0, 0);
                        var item = data.Where(m => m.Time == time).FirstOrDefault();
                        var dataItem = new CommonDataModel()
                        {
                            ID = i,
                            Name = i.ToString(),
                            Value = item != null ? item.Value : 0,
                        };
                        result.Add(dataItem);
                    }
                }
                else
                {
                    //  获取日期范围数据
                    var days = (end_ - start_).TotalDays + 1;
                    if (days <= 31)
                    {
                        //  按天
                        for (int i = 0; i < days; i++)
                        {
                            var date = start_.AddDays(i);
                            var startTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                            var endTime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

                            var value = data.Where(m => m.Time >= startTime && m.Time <= endTime).Sum(m => m.Value);
                            var dataItem = new CommonDataModel()
                            {
                                ID = i,
                                Name = i.ToString(),
                                Value = value,
                            };
                            result.Add(dataItem);
                        }
                    }
                    else
                    {
                        //  按月
                        for (int i = 0; i < 12; i++)
                        {
                            var date = new DateTime(start_.Year, i + 1, 1, 0, 0, 0);
                            var startTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                            var endTime = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

                            var value = data.Where(m => m.Time >= startTime && m.Time <= endTime).Sum(m => m.Value);
                            var dataItem = new CommonDataModel()
                            {
                                ID = i,
                                Name = i.ToString(),
                                Value = value,
                            };
                            result.Add(dataItem);
                        }
                    }
                }
                return result;
            }
        }

        private class CategoryStatisticModel
        {
            public int Duration { get; set; }
            public DateTime LogTime { get; set; }
            public int CategoryID { get; set; }
        }
        public List<ColumnDataModel> GetBrowseDataByCategoryStatistics(DateTime start_, DateTime end_)
        {
            using (var db = _database.GetReaderContext())
            {


                var start = new DateTime(start_.Year, start_.Month, start_.Day, 0, 0, 0);
                var end = new DateTime(end_.Year, end_.Month, end_.Day, 23, 59, 59);

                //  查询分类
                var categorySql = "select sum(WebBrowseLogModels.Duration) as Duration,LogTime,WebSiteModels.CategoryID as CategoryID from WebBrowseLogModels LEFT JOIN WebSiteModels on WebBrowseLogModels.SiteId=WebSiteModels.ID LEFT JOIN WebSiteCategoryModels on WebSiteModels.CategoryID=WebSiteCategoryModels.ID  WHERE LogTime>='" + start.ToString("yyyy-MM-dd 00:00:00") + "' and LogTime<= '" + end.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY WebSiteModels.CategoryID";

                var categories = db.Database.SqlQuery<CategoryStatisticModel>(categorySql).ToList();

                //  查询数据
                var dataSql = "select sum(WebBrowseLogModels.Duration) as Duration,LogTime,WebSiteModels.CategoryID as CategoryID from WebBrowseLogModels LEFT JOIN WebSiteModels on WebBrowseLogModels.SiteId=WebSiteModels.ID LEFT JOIN WebSiteCategoryModels on WebSiteModels.CategoryID=WebSiteCategoryModels.ID  WHERE LogTime>='" + start.ToString("yyyy-MM-dd 00:00:00") + "' and LogTime<= '" + end.ToString("yyyy-MM-dd 23:59:59") + "' GROUP BY WebBrowseLogModels.LogTime,WebSiteModels.CategoryID";

                var data = db.Database.SqlQuery<CategoryStatisticModel>(dataSql).ToList();
                var result = new List<ColumnDataModel>();

                if (start_ == end_)
                {
                    //  获取24小时数据
                    foreach (var category in categories)
                    {
                        result.Add(new ColumnDataModel()
                        {
                            CategoryID = category.CategoryID,
                            Values = new double[24]
                        });
                    }
                    for (int i = 0; i < 24; i++)
                    {
                        var time = new DateTime(start_.Year, start_.Month, start_.Day, i, 0, 0);
                        foreach (var category in categories)
                        {
                            var log = data.Where(m => m.CategoryID == category.CategoryID && m.LogTime == time).FirstOrDefault();
                            if (log != null)
                            {
                                var resultItem = result.Where(m => m.CategoryID == category.CategoryID).FirstOrDefault();
                                resultItem.Values[i] = log.Duration;
                            }
                        }
                    }
                }
                else
                {
                    //  获取日期范围数据
                    int days = (int)(end_ - start_).TotalDays + 1;
                    if (days <= 31)
                    {
                        //  按天
                        foreach (var category in categories)
                        {
                            result.Add(new ColumnDataModel()
                            {
                                CategoryID = category.CategoryID,
                                Values = new double[days]
                            });
                        }

                        for (int i = 0; i < days; i++)
                        {
                            var date = start_.AddDays(i);
                            var startTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                            var endTime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

                            foreach (var category in categories)
                            {
                                var duration = data.Where(m => m.CategoryID == category.CategoryID && m.LogTime >= startTime && m.LogTime <= endTime).Sum(m => m.Duration);

                                var resultItem = result.Where(m => m.CategoryID == category.CategoryID).FirstOrDefault();
                                resultItem.Values[i] = duration;
                            }
                        }
                    }
                    else
                    {
                        //  按月
                        foreach (var category in categories)
                        {
                            result.Add(new ColumnDataModel()
                            {
                                CategoryID = category.CategoryID,
                                Values = new double[12]
                            });
                        }
                        for (int i = 0; i < 12; i++)
                        {
                            var date = new DateTime(start_.Year, i + 1, 1, 0, 0, 0);
                            var startTime = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                            var endTime = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

                            foreach (var category in categories)
                            {
                                var duration = data.Where(m => m.CategoryID == category.CategoryID && m.LogTime >= startTime && m.LogTime <= endTime).Sum(m => m.Duration);

                                var resultItem = result.Where(m => m.CategoryID == category.CategoryID).FirstOrDefault();
                                resultItem.Values[i] = duration;
                            }
                        }
                    }
                }
                return result;
            }
        }

        public int GetBrowseDurationTotal(DateTime start_, DateTime end_)
        {
            start_ = new DateTime(start_.Year, start_.Month, start_.Day, 0, 0, 0);
            end_ = new DateTime(end_.Year, end_.Month, end_.Day, 23, 59, 59);

            using (var db = _database.GetReaderContext())
            {
                var data = db.WebBrowserLogs.Where(m => m.LogTime >= start_ && m.LogTime <= end_);
                var result = data.Any() ? data.Sum(m => m.Duration) : 0;
                return result;
            }
        }

        public int GetBrowseSitesTotal(DateTime start_, DateTime end_)
        {
            start_ = new DateTime(start_.Year, start_.Month, start_.Day, 0, 0, 0);
            end_ = new DateTime(end_.Year, end_.Month, end_.Day, 23, 59, 59);
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebBrowserLogs.Where(m => m.LogTime >= start_ && m.LogTime <= end_).GroupBy(m => m.SiteId).Count();
                return result;
            }
        }

        public int GetBrowsePagesTotal(DateTime start_, DateTime end_)
        {
            start_ = new DateTime(start_.Year, start_.Month, start_.Day, 0, 0, 0);
            end_ = new DateTime(end_.Year, end_.Month, end_.Day, 23, 59, 59);
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebBrowserLogs.Where(m => m.LogTime >= start_ && m.LogTime <= end_).GroupBy(m => m.UrlId).Count();
                return result;
            }
        }

        public List<WebBrowseLogModel> GetBrowseLogList(DateTime start_, DateTime end_, int siteId_ = 0)
        {
            start_ = new DateTime(start_.Year, start_.Month, start_.Day, 0, 0, 0);
            end_ = new DateTime(end_.Year, end_.Month, end_.Day, 23, 59, 59);
            using (var db = _database.GetReaderContext())
            {
                var query = (from log in db.WebBrowserLogs
                             join url in db.WebUrls on log.UrlId equals url.ID
                             join site in db.WebSites on log.SiteId equals site.ID
                             where log.LogTime >= start_ && log.LogTime <= end_ && log.SiteId == siteId_
                             orderby log.LogTime descending
                             select new
                             {
                                 Duration = log.Duration,
                                 ID = log.ID,
                                 LogTime = log.LogTime,
                                 Site = site,
                                 SiteId = log.SiteId,
                                 Url = url,
                                 UrlId = log.UrlId,
                             }).ToList().Select(s => new WebBrowseLogModel
                             {
                                 Duration = s.Duration,
                                 ID = s.ID,
                                 LogTime = s.LogTime,
                                 Site = s.Site,
                                 SiteId = s.SiteId,
                                 Url = s.Url,
                                 UrlId = s.UrlId,
                             });

                var result = query.ToList();
                return result;
            }
        }

        public WebSiteModel GetWebSite(int id_)
        {
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebSites.Where(m => m.ID == id_).FirstOrDefault();
                return result;
            }
        }

        public WebSiteModel GetWebSite(string domain)
        {
            using (var db = _database.GetReaderContext())
            {
                var result = db.WebSites.Where(m => m.Domain.ToLower() == domain.ToLower()).FirstOrDefault();
                return result;
            }
        }

        public void Clear(DateTime start_, DateTime end_)
        {
            end_ = new DateTime(end_.Year, end_.Month, DateTime.DaysInMonth(end_.Year, end_.Month));

            using (var db = _database.GetReaderContext())
            {
                //db.Database.ExecuteSqlCommand("update WebSiteModels set Duration=0");

                db.Database.ExecuteSqlCommand("delete from WebBrowseLogModels  where LogTime>='" + start_.Date.ToString("yyyy-MM-01 00:00:00") + "' and LogTime<= '" + end_.Date.ToString("yyyy-MM-dd 23:59:59") + "'");
            }
        }

        public List<WebSiteModel> GetWebSiteLogList(DateTime start_, DateTime end_)
        {
            start_ = new DateTime(start_.Year, start_.Month, start_.Day, 0, 0, 0);
            end_ = new DateTime(end_.Year, end_.Month, end_.Day, 23, 59, 59);
            using (var db = _database.GetReaderContext())
            {
                var query = (from log in db.WebBrowserLogs
                             where log.LogTime >= start_ && log.LogTime <= end_
                             join site in db.WebSites on log.SiteId equals site.ID into newSite
                             join category in db.WebSiteCategories on log.Site.CategoryID equals category.ID into newCategory
                             from nc in newCategory.DefaultIfEmpty()
                             select new
                             {
                                 ID = log.ID,
                                 UrlId = log.UrlId,
                                 Duration = log.Duration,
                                 SiteId = log.SiteId,
                                 Site = log.Site,
                                 Category = nc
                             })
                             .ToList();
                var result = query.GroupBy(m => m.SiteId).Select(s => new WebSiteModel
                {
                    Duration = s.Sum(m => m.Duration),
                    ID = s.FirstOrDefault().Site.ID,
                    Title = s.FirstOrDefault().Site.Title,
                    Domain = s.FirstOrDefault().Site.Domain,
                    IconFile = s.FirstOrDefault().Site.IconFile,
                    CategoryID = s.FirstOrDefault().Site.CategoryID,
                    Category = s.FirstOrDefault().Category,
                }).ToList();

                //var result = query.Select(s => new WebSiteModel
                //{
                //    Duration = s.Duration,
                //    ID = s.Site.ID,
                //    Title = s.Site.Title,
                //    Domain = s.Site.Domain,
                //    IconFile = s.Site.IconFile,
                //    //Category = s.Category,
                //    CategoryID = s.Site.CategoryID,
                //}).ToList();
                //return result;
                //return query;
                return result;
            }
        }

        public void Clear(int siteId_)
        {
            using (var db = _database.GetReaderContext())
            {
                db.Database.ExecuteSqlCommand("delete from WebBrowseLogModels  where SiteId = " + siteId_);
                db.Database.ExecuteSqlCommand("update WebSiteModels set Duration = 0  where ID = " + siteId_);
            }
        }

        public void Export(string dir_, DateTime start_, DateTime end_)
        {
            start_ = new DateTime(start_.Year, start_.Month, 1, 0, 0, 0);
            end_ = new DateTime(end_.Year, end_.Month, DateTime.DaysInMonth(end_.Year, end_.Month), 23, 59, 59);

            using (var db = _database.GetReaderContext())
            {
                var webSiteData = db.WebBrowserLogs.Where(m => m.LogTime >= start_ && m.LogTime <= end_)
                    .ToList()
                    .Select(m => new
                    {
                        时间 = m.LogTime,
                        标题 = m.Url.Title,
                        网址 = m.Url.Url,
                        时长 = m.Duration,
                    });


                var mapper = new Mapper();
                mapper.Put(webSiteData, "浏览记录");

                string name = $"Tai网页统计数据({start_.ToString("yyyy年MM月")}-{end_.ToString("yyyy年MM月")})";
                if (start_.Year == end_.Year && start_.Month == end_.Month)
                {
                    name = $"Tai网页统计数据({start_.ToString("yyyy年MM月")})";
                }
                mapper.Save(Path.Combine(dir_, $"{name}.xlsx"));

                //  导出csv
                using (var writer = new StreamWriter(Path.Combine(dir_, $"{name}.csv"), false, System.Text.Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(webSiteData);
                }
            }
        }
    }
}