using Core.Event;
using Core.Librarys;
using Core.Models.AppObserver;
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
using System.Timers;

namespace Core.Servicers.Instances
{
    public class AppObserver : IAppObserver
    {
        public event AppObserverEventHandler OnAppActiveChanged;

        #region win32 api
        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
         IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
         hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
         uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        #endregion

        //  获得焦点事件
        private WinEventDelegate _foregroundEventDelegate;
        private readonly IAppManager _appManager;
        private readonly IWindowManager _windowManager;
        private IntPtr _hook;
        private bool _isStart = false;
        private System.Timers.Timer _delayDetectTimer;
        public AppObserver(IAppManager appManager_, IWindowManager windowManager)
        {
            _appManager = appManager_;
            _windowManager = windowManager;
            _foregroundEventDelegate = new WinEventDelegate(ForegroundEventCallback);
            _delayDetectTimer = new System.Timers.Timer();
            _delayDetectTimer.Interval = 1000;
            _delayDetectTimer.Elapsed += DelayDetectTimer_Elapsed;
        }



        private void ForegroundEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            _delayDetectTimer?.Stop();
            DateTime activeTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();
            var args = GetAppInfoEventArgs(hwnd, activeTime);
            Debug.WriteLine(activeTime);
            Debug.WriteLine(args.App.ToString());
            //  响应事件
            OnAppActiveChanged?.Invoke(this, args);
            if (args.App.Type == Enums.AppType.SystemComponent)
            {
                _delayDetectTimer?.Start();
            }

        }

        private void DelayDetectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _delayDetectTimer?.Stop();
            DelayDetect();
        }

        private void DelayDetect()
        {
            DateTime activeTime = DateTime.Now;
            IntPtr w = Win32API.GetForegroundWindow();
            var args = GetAppInfoEventArgs(w, activeTime);
            if (args.App.Type != Enums.AppType.SystemComponent)
            {
                //  响应事件
                OnAppActiveChanged?.Invoke(this, args);
            }
        }

        private AppActiveChangedEventArgs GetAppInfoEventArgs(IntPtr handle_, DateTime activeTime_)
        {
            var app = _appManager.GetAppInfo(handle_);
            var window = _windowManager.GetWindowInfo(handle_);
            return new AppActiveChangedEventArgs(app, window, activeTime_);
        }

        private void HandleForegroundWindow()
        {
            IntPtr w = Win32API.GetForegroundWindow();
            ForegroundEventCallback(IntPtr.Zero, 0, w, 0, 0, 0, 0);
        }

        public void Start()
        {
            if (_isStart)
            {
                return;
            }
            _isStart = true;
            _hook = SetWinEventHook(0x0003, 0x0003, IntPtr.Zero, _foregroundEventDelegate, 0, 0, 0);
            HandleForegroundWindow();
        }

        public void Stop()
        {
            _isStart = false;
            UnhookWinEvent(_hook);
        }
    }
}
