using Core.Librarys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Servicers
{
    public class InputServicer : IInputServicer
    {
        public event InputEventHandler OnKeyDownInput;
        public event InputEventHandler OnKeyUpInput;

        private Win32API.LowLevelKeyboardProc keyboardProc;
        private static IntPtr hookKeyboardID = IntPtr.Zero;
        private IntPtr keyboardHook;
        public InputServicer()
        {
            keyboardProc = HookCallback;
        }
        public void Start()
        {
            //  设置键盘钩子
            keyboardHook = Win32API.SetKeyboardHook(keyboardProc);
        }

        private IntPtr HookCallback(
         int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                Win32API.KeyboardHookStruct MyKeyboardHookStruct = (Win32API.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32API.KeyboardHookStruct));

                Keys key = (Keys)MyKeyboardHookStruct.vkCode;

                //  键盘按下
                if (wParam == (IntPtr)Win32API.WM_KEYDOWN)
                {
                    OnKeyDownInput?.Invoke(this, key);
                }

                //  键盘抬起
                if ((wParam == (IntPtr)Win32API.WM_KEYUP || wParam == (IntPtr)Win32API.WM_SYSKEYUP))
                {
                    OnKeyUpInput?.Invoke(this, key);
                }
            }
            return Win32API.CallNextHookEx(hookKeyboardID, nCode, wParam, lParam);

        }
    }
}
