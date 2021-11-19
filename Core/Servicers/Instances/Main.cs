using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicers.Instances
{
    public class Main : IMain
    {
        private readonly IObserver observer;
        public Main(IObserver observer)
        {
            this.observer = observer;
        }
        public void Run()
        {
            observer.Start();
        }

        public void Exit()
        {
            observer?.Stop();
        }
    }
}
