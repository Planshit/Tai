using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Controls.Window
{
    public class DefaultWindowCommands
    {
        public static RoutedUICommand MinimizeWindowCommand { get; } = new RoutedUICommand("", "MinimizeWindowCommand", typeof(DefaultWindowCommands));
        public static RoutedUICommand RestoreWindowCommand { get; } = new RoutedUICommand("", "RestoreWindowCommand", typeof(DefaultWindowCommands));
        public static RoutedUICommand MaximizeWindowCommand { get; } = new RoutedUICommand("", "MaximizeWindowCommand", typeof(DefaultWindowCommands));
        public static RoutedUICommand CloseWindowCommand { get; } = new RoutedUICommand("", "CloseWindowCommand", typeof(DefaultWindowCommands));

        public static RoutedUICommand LogoButtonClickCommand { get; } = new RoutedUICommand("", "LogoButtonClickCommand", typeof(DefaultWindowCommands));
        public static RoutedUICommand BackCommand { get; } = new RoutedUICommand("", "BackCommand", typeof(DefaultWindowCommands));

    }
}
