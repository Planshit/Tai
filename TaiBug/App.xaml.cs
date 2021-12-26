using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TaiBug
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private System.Threading.Mutex mutex;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            //  阻止多开和用户主动启动
            if (e.Args.Length == 0 || IsRuned())
            {
                Shutdown();
            }
        }
        #region 获取当前程序是否已运行
        /// <summary>
        /// 获取当前程序是否已运行
        /// </summary>
        private bool IsRuned()
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name, out ret);
            if (!ret)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
