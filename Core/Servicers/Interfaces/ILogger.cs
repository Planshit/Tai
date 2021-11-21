using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Interfaces
{
    public interface ILogger
    {
        void Info(string message, [CallerLineNumber] int callerLineNumber = -1, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null);
        void Warn(string message, [CallerLineNumber] int callerLineNumber = -1, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null);
        void Error(string message, [CallerLineNumber] int callerLineNumber = -1, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null);
    }
}
