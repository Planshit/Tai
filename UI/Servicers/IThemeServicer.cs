using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Servicers
{
    public interface IThemeServicer
    {
        void Init();
        void LoadTheme(string themeName);
        void SetMainWindow(MainWindow mainWindow);

        void UpdateWindowStyle();
    }
}
