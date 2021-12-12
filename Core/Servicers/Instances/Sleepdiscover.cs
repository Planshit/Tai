using Core.Enums;
using Core.Event;
using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        //  播放声音开始时间
        private DateTime playSoundStartTime;
        public void Start()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 2, 0);
            timer.Tick += Timer_Tick;
            timer.Start();

            Win32API.GetCursorPos(out lastPoint);

            playSoundStartTime = DateTime.MinValue;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Point point;
            Win32API.GetCursorPos(out point);

            if (point == null)
            {
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
                            playSoundStartTime = DateTime.Now;
                        }
                        else
                        {
                            //  判断声音时间是否超过2个小时
                            TimeSpan timeSpan = DateTime.Now - playSoundStartTime;

                            if (timeSpan.TotalHours >= 2)
                            {
                                //  超过表示可能睡着了，切换状态
                                status = SleepStatus.Sleep;

                                playSoundStartTime = DateTime.MinValue;

                                SleepStatusChanged?.Invoke(status);
                            }
                        }
                    }
                    else
                    {
                        //  没有播放声音切换状态
                        status = SleepStatus.Sleep;
                        SleepStatusChanged?.Invoke(status);
                    }
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

            Win32API.GetCursorPos(out lastPoint);
        }
    }
}
