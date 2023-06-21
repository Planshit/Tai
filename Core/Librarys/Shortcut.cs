using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Librarys
{
    public class Shortcut
    {
        /// <summary>
        /// 为本程序创建一个快捷方式。
        /// </summary>
        /// <param name="lnkFilePath">快捷方式的完全限定路径。</param>
        /// <param name="args">快捷方式启动程序时需要使用的参数。</param>
        private static bool CreateShortcut(string lnkFilePath, string args)
        {
            try
            {
                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                var shortcut = shell.CreateShortcut(lnkFilePath);
                shortcut.TargetPath = Assembly.GetEntryAssembly().Location;
                shortcut.Arguments = args;
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                shortcut.Save();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return false;
            }
        }
        /// <summary>
        /// 设置开机启动
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static bool SetStartup(bool startup = true)
        {
            string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                    "Tai.lnk");
            if (startup)
            {
                if (File.Exists(path))
                {
                    return true;
                }
                return CreateShortcut(path, "");
            }
            else
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return true;
            }
        }
        /// <summary>
        /// 创建桌面快捷方式
        /// </summary>
        /// <param name="create"></param>
        /// <returns></returns>
        public static bool CreateDesktopShortcut()
        {
            return CreateShortcut(Path.Combine(
                     Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                     "Tai.lnk"), "--selfStart");
        }
    }
}
