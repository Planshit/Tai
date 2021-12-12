using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Event
{
    public delegate void ObserverEventHandler(string processName, string description, string file);
}
