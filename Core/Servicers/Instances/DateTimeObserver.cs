using Core.Event;
using Core.Librarys;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Core.Servicers.Instances
{
    public class DateTimeObserver : IDateTimeObserver
    {
        public event DateTimeObserverEventHandler OnDateTimeChanging;
        private Timer timer;

        public void Start()
        {
            Stop();
            SetTimer();
        }

        private void SetTimer()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Tick;
            DateTime now = DateTime.Now;
            DateTime updateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 59, 59);
            double diffMilliseconds = updateTime.Subtract(now).TotalMilliseconds;
            if (diffMilliseconds < 0)
            {
                diffMilliseconds = 1;
            }
            timer.Interval = diffMilliseconds;
            timer.Start();
            Logger.Info("datetime observer start,interval(s):" + updateTime.Subtract(now).TotalSeconds);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            DateTime nowTime = DateTime.Now;
            OnDateTimeChanging?.Invoke(this, nowTime);

            Thread.Sleep(2000);
            SetTimer();
        }

        public void Stop()
        {
            if (timer != null && timer.Enabled)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}
