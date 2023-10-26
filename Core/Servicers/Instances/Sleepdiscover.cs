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
using System.Threading;
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

        //  键盘钩子
        private Win32API.LowLevelKeyboardProc keyboardProc;
        private static IntPtr hookKeyboardID = IntPtr.Zero;


        //  鼠标钩子
        private Win32API.LowLevelKeyboardProc mouseProc;
        private static IntPtr hookMouseID = IntPtr.Zero;
        private IntPtr mouseHook;

        private int emptyPointNum = 0;
        public Sleepdiscover()
        {
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(OnPowerModeChanged);
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            keyboardProc = HookCallback;
            mouseProc = HookMouseCallback;
        }

        public void Start()
        {
            StartTimer();

            lastPoint = Win32API.GetCursorPosition();
            playSoundStartTime = DateTime.MinValue;
            pressKeyboardLastTime = DateTime.Now;

            //  设置键盘钩子
            Win32API.SetKeyboardHook(keyboardProc);
        }

        public void Stop()
        {
            StopTimer();
        }

        private void StartTimer()
        {
            StopTimer();
            timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 10);
            timer.Interval = new TimeSpan(0, 5, 0);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
                timer.Stop();
            }
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.RemoteDisconnect || e.Reason == SessionSwitchReason.ConsoleDisconnect)
            {
                //  与这台设备远程桌面连接断开
                if (status == SleepStatus.Wake)
                {
                    Logger.Warn("与这台设备远程桌面连接断开");
                    Sleep();
                }
            }
        }

        private IntPtr HookMouseCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0)
            {
                if (wParam == (IntPtr)Win32API.WM_LBUTTONDBLCLK || wParam == (IntPtr)Win32API.WM_WHEEL)
                {
                    Logger.Info("鼠标唤醒");

                    Wake();
                }

            }

            return Win32API.CallNextHookEx(hookMouseID, nCode, wParam, lParam);
        }
        private IntPtr HookCallback(
           int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)Win32API.WM_KEYDOWN)
            {
                if (status == SleepStatus.Sleep)
                {
                    Logger.Info("键盘唤醒");

                    Wake();
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
                        Logger.Info("设备已休眠");

                        Sleep();
                    }
                    break;
                case PowerModes.Resume:
                    //  电脑恢复
                    if (status == SleepStatus.Sleep)
                    {
                        Logger.Info("设备已恢复");

                        Wake();
                    }
                    break;
            }
        }



        #region 指示当前是否处于睡眠状态
        /// <summary>
        /// 指示当前是否处于睡眠状态
        /// </summary>
        /// <returns>睡眠返回true</returns>
        private async Task<bool> IsSleepAsync()
        {
            Point point = Win32API.GetCursorPosition();

            if (point.X + point.Y == 0)
            {
                emptyPointNum++;

                if (emptyPointNum == 2)
                {
                    emptyPointNum = 0;
                    return true;
                }
            }

            bool isActive = await IsActiveAsync();
            if (isActive)
            {
                return false;
            }

            //  鼠标和键盘都没有操作时判断是否在播放声音

            //  持续30秒检测当前是否在播放声音
            bool isPlaySound = await IsPlaySoundAsync();

            if (!isPlaySound)
            {
                //  没有声音判定为睡眠状态
                return true;
            }

            //  在播放声音
            if (playSoundStartTime == DateTime.MinValue)
            {
                //  第一次记录声音开始时间
                playSoundStartTime = DateTime.Now;
                return false;
            }

            //  声音播放超过两个小时视为睡眠状态
            TimeSpan timeSpan = DateTime.Now - playSoundStartTime;

            if (timeSpan.TotalHours >= 2)
            {
                //  重置声音开始时间
                playSoundStartTime = DateTime.MinValue;
                return true;
            }

            return false;
        }
        #endregion

        private async void Timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("检测");
            timer.Stop();
            bool isSleep = await IsSleepAsync();
            if (isSleep)
            {
                Sleep();
            }
            else
            {
                timer.Start();
            }
        }

        private void Sleep()
        {
            if (status == SleepStatus.Sleep)
            {
                return;
            }

            //  卸载事件
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;

            //  停止离开检测计时器
            StopTimer();

            status = SleepStatus.Sleep;

            //  设置鼠标钩子
            mouseHook = Win32API.SetMouseHook(mouseProc);

            //  状态通知
            SleepStatusChanged?.Invoke(status);
        }

        private void Wake()
        {
            if (status == SleepStatus.Wake)
            {
                return;
            }
            try
            {
                //  注册事件
                SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

                status = SleepStatus.Wake;

                //  卸载鼠标钩子
                Win32API.UnhookWindowsHookEx(mouseHook);

                //  启动离开检测
                StartTimer();



                //  重置声音播放时间
                playSoundStartTime = DateTime.MinValue;

                //  重置鼠标坐标
                lastPoint = Win32API.GetCursorPosition();

                //  状态通知
                SleepStatusChanged?.Invoke(status);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        /// <summary>
        /// 指示当前是否在播放声音（持续30秒检测）
        /// </summary>
        /// <returns>播放返回true</returns>
        private async Task<bool> IsPlaySoundAsync()
        {
            bool result = false;

            await Task.Run(() =>
            {
                //  持续30秒
                int time = 30;

                while (time > 0)
                {
                    bool isPlay = Win32API.IsWindowsPlayingSound();
                    if (isPlay)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        time--;
                        Debug.WriteLine(time);
                        Thread.Sleep(1000);
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// 指示当前是否处于活跃状态（持续30秒检测）
        /// </summary>
        /// <returns>活跃返回true</returns>
        private async Task<bool> IsActiveAsync()
        {
            bool result = false;

            await Task.Run(() =>
            {
                //  持续30秒
                int time = 30;

                while (time > 0)
                {
                    Point point = Win32API.GetCursorPosition();
                    bool isMouseMove = lastPoint.ToString() != point.ToString();
                    bool isKeyboardActive = !IsKeyboardOuttime();

                    if (isMouseMove || isKeyboardActive)
                    {
                        lastPoint = Win32API.GetCursorPosition();
                        result = true;
                        break;
                    }
                    else
                    {
                        time--;
                        Debug.WriteLine(time);
                        Thread.Sleep(1000);
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// 指示是否键盘行为超时（超过10分钟）
        /// </summary>
        /// <returns>超时返回true</returns>
        private bool IsKeyboardOuttime()
        {
            TimeSpan timeSpan = DateTime.Now - pressKeyboardLastTime;
#if DEBUG
            return timeSpan.TotalSeconds >= 10;
#endif
            return timeSpan.TotalMinutes >= 10;
        }
    }
}
