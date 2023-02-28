using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UI.Servicers
{
    public interface IWebSiteContextMenuServicer
    {
        void Init();
        ContextMenu GetContextMenu();
    }
}
