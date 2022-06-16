using Core.Enums;
using Core.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Event
{
    public delegate void SleepdiscoverEventHandler(SleepStatus sleepStatus);
    public delegate void AppConfigEventHandler(ConfigModel oldConfig, ConfigModel newConfig);
    public delegate void DateTimeObserverEventHandler(object sender, DateTime e);
}
