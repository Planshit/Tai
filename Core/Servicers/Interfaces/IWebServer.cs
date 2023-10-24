using Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface IWebServer
    {
        void Start();
        void Stop();
        void SendMsg(string msg_);
    }
}
