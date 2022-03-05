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
        public string Description;
        public string ToggleTrueText;
        public string ToggleFalseText;
        public string Group;
        public int Index;
        public bool IsName;
        public string Placeholder;
        public bool IsCanRepeat;
        public bool IsCanImportExport;
        public ConfigAttribute()
        {
            ToggleTrueText = "开";
            ToggleFalseText = "关";
            IsCanRepeat = true;
            Index = 0;
            IsName = false;
            IsCanImportExport = false;
        }


    }
}
