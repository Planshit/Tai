using Core.Enums;
using Core.Event;
using Core.Librarys;
using Core.Servicers.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Core.Servicers.Instances
{
    public class Sleepdiscover : ISleepdiscover
    {
        public event SleepdiscoverEventHandler SleepStatusChanged;

        private DispatcherTimer timer;

        private Point lastPoint;

        private SleepStatus status = SleepStatus.Wake;

        /// <summary>
        /// 播放声音开始时间
        /// </summary>
        private DateTime playSoundStartTime;

        /// <summary>
        /// 最后一次按键时间
        /// </summary>
        private DateTime pressKeyboardLastTime;

        private readonly IObserver observer;

        private Win32API.LowLevelKeyboardProc keyboardProc;
        private static IntPtr hookKeyboardID = IntPtr.Zero;
        private int emptyPointNum = 0;
        public Sleepdiscover(IObserver observer)
        {
            this.observer = observer;
            observer.OnAppActive += Observer_OnAppActive;
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerModeChanged);
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            keyboardProc = HookCallback;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.RemoteDisconnect || e.Reason == SessionSwitchReason.ConsoleDisconnect)
            {
                //  与这台设备远程桌面连接断开
                if (status == SleepStatus.Wake)
                {
                    status = SleepStatus.Sleep;
                    SleepStatusChanged?.Invoke(status);
                    Logger.Warn("与这台设备远程桌面连接断开");
                }
            }
        }

        private IntPtr HookCallback(
           int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)Win32API.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (status == SleepStatus.Sleep)
                {
                    status = SleepStatus.Wake;
                    SleepStatusChanged?.Invoke(status);
                }
                else
                {
                    playSoundStartTime = DateTime.MinValue;
                    pressKeyboardLastTime = DateTime.Now;
                }
            }
            return Win32API.CallNextHookEx(hookKeyboardID, nCode, wParam, lParam);
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    //  电脑休眠
                    if (status == SleepStatus.Wake)
                    {
                        status = SleepStatus.Sleep;
                        SleepStatusChanged?.Invoke(status);
                        Logger.Warn("设备已休眠");
                    }
                    break;
                case PowerModes.Resume:
                    //  电脑恢复
                    if (status == SleepStatus.Sleep)
                    {
                        playSoundStartTime = DateTime.MinValue;
                        status = SleepStatus.Wake;
                        SleepStatusChanged?.Invoke(status);
                        Logger.Warn("设备已恢复");
                    }
                    break;
            }
        }

        private void Observer_OnAppActive(string processName, string description, string file)
        {
            if (status == SleepStatus.Sleep)
            {
                TimeSpan timeSpan = DateTime.Now - pressKeyboardLastTime;

                Point point = Win32API.GetCursorPosition();
                if (lastPoint.ToString() == point.ToString() && timeSpan.TotalSeconds > 10)
                {
                    //  非鼠标或键盘激活
                    return;
                }

                playSoundStartTime = DateTime.MinValue;
                status = SleepStatus.Wake;
                SleepStatusChanged?.Invoke(status);
            }
        }

        public void Start()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 2, 0);
#if DEBUG
            //timer.Interval = new TimeSpan(0, 0, 10);
#endif
            timer.Tick += Timer_Tick;
            timer.Start();

            lastPoint = Win32API.GetCursorPosition();

            playSoundStartTime = DateTime.MinValue;

            pressKeyboardLastTime = DateTime.Now;

            //  设置键盘钩子
            Win32API.SetKeyboardHook(keyboardProc);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Point point = Win32API.GetCursorPosition();

            if (point.X + point.Y == 0)
            {
                emptyPointNum++;

                if (emptyPointNum == 2)
                {
                    emptyPointNum = 0;
                    status = SleepStatus.Sleep;
                    SleepStatusChanged?.Invoke(status);
                    Logger.Info("empty point num:" + point.ToString());
                }
                return;
            }

            if (lastPoint == null)
            {
                lastPoint = point;
                return;
            }

            if (status == SleepStatus.Wake)
            {

                if (lastPoint.ToString() == point.ToString())
                {
                    //  处于活跃状态
                    bool isPlaySound = Win32API.IsWindowsPlayingSound();
                    //  超过时间未活动鼠标

                    if (isPlaySound)
                    {
                        //  在播放声音

                        if (playSoundStartTime == DateTime.MinValue)
                        {
                            playSoundStartTime = DateTime.Now;//没有正确更新时间导致没超过2个小时就进入睡眠状态了
                        }
                        else
                        {
                            //  判断声音时间是否超过2个小时
                            TimeSpan timeSpan = DateTime.Now - playSoundStartTime;

                            if (timeSpan.TotalHours >= 2 && IsPressKeyboardOuttime())
                            {
                                //  超过表示可能睡着了，切换状态
                                status = SleepStatus.Sleep;
                                Logger.Info("[sleep] [1] time:" + timeSpan.TotalHours + ",start:" + playSoundStartTime);

                                playSoundStartTime = DateTime.MinValue;
                                SleepStatusChanged?.Invoke(status);
                            }
                        }
                    }
                    else
                    {
                        if (IsPressKeyboardOuttime())
                        {
                            //  没有播放声音且键盘休眠超时
                            status = SleepStatus.Sleep;
                            Logger.Info("[sleep] [2] " + pressKeyboardLastTime);

                            SleepStatusChanged?.Invoke(status);
                        }
                    }
                }
                else
                {
                    playSoundStartTime = DateTime.MinValue;
                }
            }
            else
            {
                //  处于休眠状态
                if (lastPoint.ToString() != point.ToString())
                {
                    //  鼠标活动了切换状态
                    status = SleepStatus.Wake;
                    SleepStatusChanged?.Invoke(status);
                }
            }

            lastPoint = point;
        }


        /// <summary>
        /// 通过按键时间判断（超过5分钟未有键盘操作视为超时）
        /// </summary>
        /// <returns></returns>
        private bool IsPressKeyboardOuttime()
        {
            TimeSpan timeSpan = DateTime.Now - pressKeyboardLastTime;
            return timeSpan.TotalMinutes >= 5;
        }
    }
}
