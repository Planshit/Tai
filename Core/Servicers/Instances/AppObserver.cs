using Core.Event;
using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class AppObserver : IAppObserver
    {
        private IntPtr hook;
        Win32API.WinEventDelegate winEventDelegate;

        public event ObserverEventHandler OnAppActive;

        public AppObserver()
        {
            winEventDelegate = new Win32API.WinEventDelegate(WinEventProc);
        }
        public void Start()
        {
            hook = Win32API.SetWinEventHook(0x0003, 0x0003, IntPtr.Zero,
            winEventDelegate, 0, 0, 0);

            var window = Win32API.GetForegroundWindow();
            Handle(window);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Handle(hwnd);
        }

        private async void Handle(IntPtr hwnd)
        {
            Debug.WriteLine(hwnd);
            string processName = String.Empty, processFileName = String.Empty, processDescription = String.Empty;

            var processResult = await Task.Run(() =>
            {
                try
                {
                    string[] processInfo = GetProcessInfoByWindowHandle(hwnd);
                    int processID = int.Parse(processInfo[2]);
                    processName = processInfo[0];
                    processFileName = processInfo[1];


                    //  handle uwp app
                    if (processFileName.IndexOf("ApplicationFrameHost.exe") != -1)
                    {
                        processFileName = Win32API.UWP_AppName(hwnd, (uint)processID);
                        if (processFileName == null)
                        {
                            Thread.Sleep(1500);

                            processFileName = Win32API.UWP_AppName(hwnd, (uint)processID);
                            processName = GetUWPAPPName(processFileName);

                            IntPtr foregeroundHwnd = Win32API.GetForegroundWindow();

                            if (foregeroundHwnd != hwnd)
                            {
                                //    获取后焦点变了
                                processInfo = GetProcessInfoByWindowHandle(foregeroundHwnd);
                                processID = int.Parse(processInfo[2]);
                                processName = processInfo[0];
                                processFileName = processInfo[1];
                            }

                        }
                        else
                        {
                            processName = GetUWPAPPName(processFileName);
                        }

                    }
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(processFileName);
                    processDescription = info.FileDescription;


                    int titleCapacity = Win32API.GetWindowTextLength(hwnd) * 2;
                    StringBuilder stringBuilder = new StringBuilder(titleCapacity);
                    Win32API.GetWindowText(hwnd, stringBuilder, stringBuilder.Capacity);
                    if (IsSystemProcess(processName, stringBuilder.ToString()))
                    {
                        //  过滤掉一些系统进程
                        processName = string.Empty;
                    }
                }
                catch (Exception e)
                {
                    Logger.Warn(e.Message + " | Process Name:" + processName + " | Process File:" + processFileName + " | Process Description:" + processDescription);
                }
                return processName;
            });

            EventInvoke(processName, processDescription, processFileName);
        }

        /// <summary>
        /// 指示指定进程是否属于系统进程
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <param name="title">窗口标题</param>
        /// <returns></returns>
        private bool IsSystemProcess(string processName, string title)
        {
            processName = processName.ToLower();
            int length = title.Length;
            string[] titleArr = { "UnlockingWindow", "Program Manager" };

            if (processName == "explorer")
            {
                if (length == 0)
                {
                    //  可能是任务栏
                    return true;
                }
                return titleArr.Contains(title);

            }
            return false;
        }

        private string GetUWPAPPName(string processFileName)
        {
            string processName = string.Empty;
            if (!string.IsNullOrEmpty(processFileName) && processFileName.IndexOf("\\") != -1)
            {
                processName = processFileName.Split('\\').Last();
                processName = processName.Replace(".exe", "");
            }
            return processName;
        }
        /// <summary>
        /// 通过窗口句柄获取进程信息
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        private string[] GetProcessInfoByWindowHandle(IntPtr hwnd)
        {
            string processName, processFileName = string.Empty;

            int processID = 0;
            Win32API.GetWindowThreadProcessId(hwnd, out processID);
            Process process = Process.GetProcessById(processID);
            processName = process.ProcessName;

            IntPtr processHandle = IntPtr.Zero;
            processHandle = Win32API.OpenProcess(0x001F0FFF, false, processID);

            if (processHandle != IntPtr.Zero)
            {
                int MaxPathLength = 4096;
                var buffer = new StringBuilder(MaxPathLength);
                Win32API.QueryFullProcessImageName(processHandle, 0, buffer, ref MaxPathLength);
                processFileName = buffer.ToString();
            }
            else
            {
                Logger.Warn($"无法获取进程：{processName}的句柄，请尝试以管理员身份运行Tai。");
            }
            return new string[] { processName, processFileName, processID.ToString() };
        }
        private void EventInvoke(string processName, string description, string filename)
        {
            OnAppActive?.Invoke(processName, description, filename);
        }

        public void Stop()
        {
            Win32API.UnhookWinEvent(hook);
        }
    }
}
