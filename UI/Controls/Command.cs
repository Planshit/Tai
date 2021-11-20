using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Controls
{
    public class Command : ICommand
    {
        private Action<object> _action;

        public Command(Action<object> action)
        {
            _action = action;
        }


        #region ICommand Members  
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            _action(parameter);
            OnExecuted();
        }
        public delegate void ExecutedHandler(object parameter);
        public event ExecutedHandler Executed;
        public void OnExecuted()
        {
            Executed?.Invoke(null);
        }
        #endregion
    }
}
