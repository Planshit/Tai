using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys
{
    public class Logger
    {
        private static readonly object writeLock = new object();

        private enum LogLevel
        {
            Info,
            Warn,
            Error,
        }

        public static void Info(string message, [CallerLineNumber] int callerLineNumber = -1, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
        {
            message = message + "\r\n[Caller Info] Line:" + callerLineNumber + ",File:" + callerFilePath + ",name:" + callerMemberName;
            Save(Fromat(LogLevel.Info, message));
        }

        public static void Warn(string message, [CallerLineNumber] int callerLineNumber = -1, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
        {
            message = message + "\r\n[Caller Info] Line:" + callerLineNumber + ",File:" + callerFilePath + ",name:" + callerMemberName;
            Save(Fromat(LogLevel.Warn, message));
        }
        public static void Error(string message, [CallerLineNumber] int callerLineNumber = -1, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
        {
            message = message + "\r\n[Caller Info] Line:" + callerLineNumber + ",File:" + callerFilePath + ",name:" + callerMemberName;
            Save(Fromat(LogLevel.Error, message));
        }

        private static string Fromat(LogLevel logLevel, string message)
        {
            message = HandleMessage(message);
            string logText = $"[{logLevel.ToString()}] [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] \r\n{message}\r\n------------------------\r\n\r\n";
            Debug.WriteLine(logText);
            return logText;
        }

        private static void Save(string message)
        {
            string loggerName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                         "Log", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
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

        private static string HandleMessage(string message)
        {
            message = message.Replace("\\u", "\\\\u");
            return message;
        }
    }
}
