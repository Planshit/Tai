using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Select;
using UI.Servicers;

namespace UI.Models
{
    public class ModelBase : LocalizedView
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
                    Name=LocalizationServicer.Instance.Translated("page.apps")
                },
                new SelectItemModel()
                {
                    Id=1,
                    Name=LocalizationServicer.Instance.Translated("page.websites")
                }
        };
        
        protected ModelBase(ILocalizationServicer localizationServicer) : base(localizationServicer)
        {
            ShowType = ShowTypeOptions[0];
        }
        
        public virtual void Dispose() { }
    }
}
