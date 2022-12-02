using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.AppObserver
{
    public class AppObserverEventArgs
    {
        private IntPtr _handle;
        private string _processName;
        private string _description;
        private string _file;

        public string ProcessName { get { return _processName; } }
        public string Description { get { return _description; } }
        public string File { get { return _file; } }
        public IntPtr Handle { get { return _handle; } }
        public AppObserverEventArgs(string processName, string description, string file, IntPtr handle)
        {
            _processName = processName;
            _description = description;
            _file = file;
            _handle = handle;
        }
    }
}
