using Core.Models.WebPage;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys
{
    public static class SiteCache
    {
        private static List<Site> sites = new List<Site>();
        private static readonly int maxNum = 100;
        public static Site Get(string key)
        {
            return sites.Where(m => m.Title.Equals(key)).FirstOrDefault();
        }

        public static List<Site> GetAll()
        {
            return sites;
        }

        public static void Set(string key, string value)
        {
            if (Get(key) == null)
            {
                sites.Add(new Site()
                {
                    Title = key,
                    Url = value
                });

                if (sites.Count > maxNum)
                {
                    sites.RemoveAt(0);
                }
            }
        }
    }
}
