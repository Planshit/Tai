using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.WebPage
{
    public class Site
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public override string ToString()
        {
            return $"Title:{Title},Url:{Url}";
        }

        public static Site Empty = new Site()
        {
            Title = string.Empty,
            Url = string.Empty
        };

        public Site(Site site = null)
        {
            if (site != null)
            {
                Title = site.Title;
                Url = site.Url;
            }
        }
    }
}
