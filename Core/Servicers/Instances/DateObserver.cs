using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Core.Servicers.Instances
{
    public class DateObserver : IDateObserver
    {
        public event EventHandler OnDateChanging;



        private DispatcherTimer timer;

        public DateObserver()
        {
        }


        public void Start()
        {
            SetTimer();
        }

        private void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            DateTime now = DateTime.Now;
            DateTime tmorrow = new DateTime(now.Year, now.Month, now.Day, 23, 59, 30);
            int diffseconds = (int)tmorrow.Subtract(now).TotalSeconds;
            if (diffseconds < 0)
            {
                diffseconds = 0;
            }
            timer.Interval = new TimeSpan(0, 0, diffseconds);
            timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            OnDateChanging?.Invoke(this, EventArgs.Empty);

            await Task.Run(async () =>
            {
                await Task.Delay(60000);
                SetTimer();
            });
        }
    }
}
