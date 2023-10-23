using Core.Enums;
using Core.Librarys;
using Core.Models.AppObserver;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class AppManager : IAppManager
    {
        #region win32api
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        public const int PROCESS_VM_READ = 0x0010;

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern uint GetModuleBaseName(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, ref uint nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public uint cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);
        #endregion

        private Dictionary<string, AppInfo> _apps;
        private int _outTime = 5000;
        //private string _windowsVersionName;
        public AppManager()
        {
            _apps = new Dictionary<string, AppInfo>();
            //_windowsVersionName = SystemCommon.GetWindowsVersionName();
        }

        public AppInfo GetAppInfo(IntPtr hwnd_)
        {
            try
            {
                AppInfo app;
                var sw = Stopwatch.StartNew();
                GetWindowThreadProcessId(hwnd_, out int processId);
                sw = Stopwatch.StartNew();
                string processName = GetAppProcessName(processId);
                string executablePath = string.Empty;
                string description = string.Empty;
                AppType appType = AppType.Win32;

                sw = Stopwatch.StartNew();
                if (string.IsNullOrEmpty(processName))
                {
                    return AppInfo.Empty;
                }
                else if (_apps.ContainsKey(processName))
                {
                    return _apps[processName];
                }
                else if (processName == "ApplicationFrameHost")
                {
                    //  uwp应用
                    //  尝试直接获取可执行文件路径，如果为空表示刚启动或者全屏状态
                    executablePath = GetUWPAPPExecutablePath(hwnd_, (uint)processId);
                    appType = AppType.UWP;

                    if (string.IsNullOrEmpty(executablePath))
                    {
                        //  刚启动的程序需要延迟获取
                        int duration = 0;
                        while (processName == "ApplicationFrameHost")
                        {
                            Thread.Sleep(100);
                            duration += 100;
                            if (duration >= _outTime)
                            {
                                break;
                            }
                            executablePath = GetUWPAPPExecutablePath(hwnd_, (uint)processId);

                            if (string.IsNullOrEmpty(executablePath))
                            {
                                //  可能是全屏状态
                                int pid = GetFullScreenUWPPID();
                                string name = GetAppProcessName(pid);

                                processName = string.IsNullOrEmpty(name) ? processName : name;
                                executablePath = GetAppExecutablePath(pid);
                                processId = pid;
                            }
                            else
                            {
                                processName = Path.GetFileNameWithoutExtension(executablePath);
                            }
                        }
                    }
                    else
                    {
                        processName = Path.GetFileNameWithoutExtension(executablePath);
                    }
                }
                //else if (_windowsVersionName.Contains("Windows 7") && processName == "dllhost")
                //{
                //    //  Windows7的COM应用

                //}
                else
                {
                    executablePath = GetAppExecutablePath(processId);
                }

                if (!string.IsNullOrEmpty(executablePath))
                {
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(executablePath);
                    description = info.FileDescription;
                }

                appType = IsSystemComponent(processName, hwnd_) ? AppType.SystemComponent : AppType.Win32;
                app = new AppInfo(hwnd_, processId, processName, description, executablePath, appType);
                if (processName != "explorer" && !_apps.ContainsKey(processName))
                {
                    _apps.Add(processName, app);
                }
                return app;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return AppInfo.Empty;
            }
        }

        private string GetAppProcessName(int processId_)
        {
            Process process = Process.GetProcessById(processId_);
            return process.ProcessName;
        }

        private string GetAppExecutablePath(int processId_)
        {
            IntPtr processHandle = IntPtr.Zero;
            processHandle = OpenProcess(0x001F0FFF, false, processId_);
            string executablePath = string.Empty;
            if (processHandle != IntPtr.Zero)
            {
                int MaxPathLength = 4096;
                var buffer = new StringBuilder(MaxPathLength);
                QueryFullProcessImageName(processHandle, 0, buffer, ref MaxPathLength);
                executablePath = buffer.ToString();
            }
            return executablePath;
        }

        internal struct WINDOWINFO
        {
            public uint ownerpid;
            public uint childpid;
        }

        private string GetUWPAPPExecutablePath(IntPtr hWnd_, uint pID_)
        {
            WINDOWINFO windowinfo = new WINDOWINFO();
            windowinfo.ownerpid = pID_;
            windowinfo.childpid = windowinfo.ownerpid;

            IntPtr pWindowinfo = Marshal.AllocHGlobal(Marshal.SizeOf(windowinfo));

            Marshal.StructureToPtr(windowinfo, pWindowinfo, false);

            Win32API.EnumWindowProc lpEnumFunc = new Win32API.EnumWindowProc(EnumChildWindowsCallback);
            Win32API.EnumChildWindows(hWnd_, lpEnumFunc, pWindowinfo);

            windowinfo = (WINDOWINFO)Marshal.PtrToStructure(pWindowinfo, typeof(WINDOWINFO));
            if (windowinfo.childpid == windowinfo.ownerpid)
            {
                return null;
            }
            IntPtr proc;
            if ((proc = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, (int)windowinfo.childpid)) == IntPtr.Zero) return null;

            int capacity = 2000;
            StringBuilder sb = new StringBuilder(capacity);
            QueryFullProcessImageName(proc, 0, sb, ref capacity);

            Marshal.FreeHGlobal(pWindowinfo);

            return sb.ToString(0, capacity);
        }
        private bool EnumChildWindowsCallback(IntPtr hWnd_, IntPtr lParam_)
        {
            WINDOWINFO info = (WINDOWINFO)Marshal.PtrToStructure(lParam_, typeof(WINDOWINFO));

            int pID;
            GetWindowThreadProcessId(hWnd_, out pID);

            if (pID != info.ownerpid) info.childpid = (uint)pID;

            Marshal.StructureToPtr(info, lParam_, true);

            return true;
        }

        private IntPtr getThreadWindowHandle(uint dwThreadId_)
        {
            IntPtr hWnd;

            // Get Window Handle and title from Thread
            var guiThreadInfo = new GUITHREADINFO();
            guiThreadInfo.cbSize = (uint)Marshal.SizeOf(guiThreadInfo);

            GetGUIThreadInfo(dwThreadId_, ref guiThreadInfo);

            hWnd = guiThreadInfo.hwndFocus;
            //some times while changing the focus between different windows, it returns Zero so we would return the Active window in that case
            if (hWnd == IntPtr.Zero)
            {
                hWnd = guiThreadInfo.hwndActive;
            }
            return hWnd;
        }

        #region 获取全屏UWP应用PID
        /// <summary>
        /// 获取全屏UWP应用PID
        /// </summary>
        /// <returns></returns>
        private int GetFullScreenUWPPID()
        {
            var current = getThreadWindowHandle(0);
            int processId = 0;
            GetWindowThreadProcessId(current, out processId);
            return processId;
        }
        #endregion

        #region 判断应用是否是系统组件
        private bool IsSystemComponent(string processName_, IntPtr windowHandle_)
        {
            //  系统组件类名
            string[] sysClassNameArr = {
                "Progman",
                "WorkerW",
                "Shell_TrayWnd",
                "XamlExplorerHostIslandWindow",
                "TopLevelWindowForOverflowXamlIsland",
                "Shell_InputSwitchTopLevelWindow",
                "LockScreenControllerProxyWindow",
                //  win7
                "DV2ControlHost",
                "Button"
            };
            //  系统进程名称
            string[] sysProcessArr = {
                "ShellExperienceHost",
                "StartMenuExperienceHost",
                "SearchHost",
                "LockApp"
            };

            //  在windows7下，dllhost进程可能是Windows照片查看器，classname为：Photo_Lightweight_Viewer
            //  需要单独为windows7写一个判定器，然后加一个转换器，dllhost进程->windows照片查看器
            //  explorer，class:CabinetWClass在win7下可能是文件夹、控制面板、我的电脑属性
            bool isSys;
            if (processName_ == "explorer")
            {
                //  先通过类名判定
                string className = Win32API.GetWindowClassName(windowHandle_);
                Logger.Info($"IsSystemComponent,process:{processName_},className:{className}");
                if (!string.IsNullOrEmpty(className))
                {
                    isSys = sysClassNameArr.Contains(className);
                }
                else
                {
                    int titleLength = Win32API.GetWindowTextLength(windowHandle_);
                    isSys = titleLength <= 0;
                }
            }
            else
            {
                //  通过进程名称判定
                isSys = sysProcessArr.Contains(processName_);
            }
            return isSys;
        }
        #endregion

        #region 获取COM应用
        private AppInfo GetCOMAPP(int processId_)
        {
            try
            {
                string wmiQuery = string.Format("select ProcessId,CommandLine from Win32_Process where ProcessId='{0}'", processId_);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
                var enu = searcher.Get().GetEnumerator();
                if (enu.MoveNext())
                {
                    var commandLine = enu.Current["CommandLine"];
                    string appId = Regex.Match(commandLine.ToString(), @"\{(.*?)\}").Value;
                    if (!string.IsNullOrEmpty(appId))
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return null;
        }
        #endregion
    }
}
