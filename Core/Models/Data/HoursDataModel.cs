using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Data
{
    public class ColumnDataModel
    {
        public int AppId { get; set; }
        public int CategoryID { get; set; }
        public double[] Values { get; set; }
    }
}
