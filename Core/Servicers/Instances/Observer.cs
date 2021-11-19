using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class Observer : IObserver
    {
        private IntPtr hook;
        Win32API.WinEventDelegate winEventDelegate;

        public Observer()
        {
            winEventDelegate = new Win32API.WinEventDelegate(WinEventProc);
        }
        public void Start()
        {
            hook = Win32API.SetWinEventHook(0x0003, 0x0003, IntPtr.Zero,
            winEventDelegate, 0, 0, 0);
        }

        private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            int processID = 0;
            Win32API.GetWindowThreadProcessId(hwnd, out processID);
            Process process = Process.GetProcessById(processID);
            Debug.WriteLine(process.ProcessName);
        }


        public void Stop()
        {
            Win32API.UnhookWinEvent(hook);
        }
    }
}
