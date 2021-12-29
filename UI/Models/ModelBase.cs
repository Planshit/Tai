using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models
{
    public class ModelBase : UINotifyPropertyChanged
    {
        public virtual void Dispose() { }
    }
}
