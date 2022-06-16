using Core.Event;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Core.Servicers.Instances
{
    public class DateTimeObserver : IDateTimeObserver
    {
        public event DateTimeObserverEventHandler OnDateTimeChanging;
        private DispatcherTimer timer;

        public void Start()
        {
            SetTimer();
        }

        private void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            DateTime now = DateTime.Now;
            DateTime updateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 59, 59);
            int diffMilliseconds = (int)updateTime.Subtract(now).TotalMilliseconds;
            if (diffMilliseconds < 0)
            {
                diffMilliseconds = 1;
            }
            timer.Interval = new TimeSpan(0, 0, 0, 0, diffMilliseconds);
            timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            DateTime nowTime = DateTime.Now;

            OnDateTimeChanging?.Invoke(this, nowTime);

            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                SetTimer();
            });
        }
    }
}
