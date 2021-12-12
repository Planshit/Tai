using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Charts.Model;

namespace UI.Models
{
    public class DataPageModel : UINotifyPropertyChanged
    {
        private List<ChartsDataModel> Data_;
        /// <summary>
        ///  data
        /// </summary>
        public List<ChartsDataModel> Data
        {
            get { return Data_; }
            set { Data_ = value; OnPropertyChanged(); }
        }

        private DateTime Date_;
        /// <summary>
        ///  date
        /// </summary>
        public DateTime Date
        {
            get { return Date_; }
            set { Date_ = value; OnPropertyChanged(); }
        }
    }
}
