using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Select;

namespace UI.Models
{
    public class ModelBase : UINotifyPropertyChanged
    {
        private SelectItemModel ShowType_;
        /// <summary>
        /// 展示类型（0=应用/1=网站）
        /// </summary>
        public SelectItemModel ShowType { get { return ShowType_; } set { ShowType_ = value; OnPropertyChanged(); } }


        /// <summary>
        /// 展示类型选项
        /// </summary>
        public List<SelectItemModel> ShowTypeOptions { get; } = new List<SelectItemModel>()
        {
            new SelectItemModel()
                {
                    Id=0,
                    Name="应用"
                },
                new SelectItemModel()
                {
                    Id=1,
                    Name="网站"
                }
        };
        public ModelBase()
        {
            ShowType = ShowTypeOptions[0];
        }
        public virtual void Dispose() { }
    }
}
