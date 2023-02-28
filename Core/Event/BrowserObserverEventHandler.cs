using Core.Enums;
using Core.Models.WebPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Event
{
    public delegate void BrowserObserverEventHandler(BrowserType browserType, Site site);
}
