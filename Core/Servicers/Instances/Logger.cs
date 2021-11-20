using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class Logger : ILogger
    {
        private static readonly object writeLock = new object();
        private string loggerName;
        private enum LogLevel
        {
            Info,
            Warn,
            Error,
        }
        public Logger()
        {
            loggerName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         "Log", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }
        public void Info(string message)
        {
            Save(Fromat(LogLevel.Info, message));
        }

        public void Warn(string message)
        {
            Save(Fromat(LogLevel.Warn, message));
        }
        public void Error(string message)
        {
            Save(Fromat(LogLevel.Error, message));
        }

        private string Fromat(LogLevel logLevel, string message)
        {
            string logText = $"[{logLevel.ToString()}] [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] \r\n{message}\r\n------------------------\r\n\r\n";
            Debug.WriteLine(logText);
            return logText;
        }

        private void Save(string message)
        {
            lock (writeLock)
            {
                string dir = Path.GetDirectoryName(loggerName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.AppendAllText(loggerName, message);
            }
        }
    }
}
