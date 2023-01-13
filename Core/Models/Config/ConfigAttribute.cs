using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Config
{
    [AttributeUsage(AttributeTargets.Class |
 AttributeTargets.Constructor |
 AttributeTargets.Field |
 AttributeTargets.Method |
 AttributeTargets.Property)]

    public class ConfigAttribute : System.Attribute
    {
        public string Name;
        public string NameAddition; // 用于 RenderStringPairList ，表示第二项的名称
        public string Description;
        public string ToggleTrueText;
        public string ToggleFalseText;
        public string Group;
        public int Index;
        public bool IsName;
        public string Placeholder;
        public string PlaceholderAddition; // 用于 RenderStringPairList，表示第二项的 Placeholder
        public bool IsCanRepeat;
        public bool IsCanImportExport;
        public string Options;
        public ConfigAttribute()
        {
            ToggleTrueText = "开";
            ToggleFalseText = "关";
            IsCanRepeat = true;
            Index = 0;
            IsName = false;
            IsCanImportExport = false;
            Options = string.Empty;
        }


    }
}
