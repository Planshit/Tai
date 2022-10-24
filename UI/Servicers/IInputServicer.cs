using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Servicers
{
    public interface IInputServicer
    {
        void Start();
        /// <summary>
        /// 当键盘按下时发生
        /// </summary>
        event InputEventHandler OnKeyDownInput;
        /// <summary>
        /// 当键盘抬起时发生
        /// </summary>
        event InputEventHandler OnKeyUpInput;
    }
}
