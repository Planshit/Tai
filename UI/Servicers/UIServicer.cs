using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls.Window;

namespace UI.Servicers
{
    public class UIServicer : IUIServicer
    {
        private DefaultWindow _window;
        public UIServicer()
        {

        }

        public void InitWindow(DefaultWindow window_)
        {
            _window = window_;
        }

        public Task<bool> ShowConfirmDialogAsync(string title_, string message_)
        {
            return _window.ShowConfirmDialogAsync(title_, message_);
        }

        public Task<string> ShowInputModalAsync(string title_, string placeholder_, string value_ = null, Func<string, bool> validFnc_ = null)
        {
            return _window.ShowInputModalAsync(title_, placeholder_, value_, validFnc_);

        }
    }
}
