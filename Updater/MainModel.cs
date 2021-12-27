using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public class MainModel : UINotifyPropertyChanged
    {
        private double ProcessValue_;
        public double ProcessValue { get { return ProcessValue_; } set { ProcessValue_ = value; OnPropertyChanged(); } }
        private string Version_;
        public string Version { get { return Version_; } set { Version_ = value; OnPropertyChanged(); } }

    }
}
