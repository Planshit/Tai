using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Servicers
{
    public class MainServicer : IMainServicer
    {
        private readonly IMain main;
        private readonly IThemeServicer themeServicer;
        private readonly IInputServicer inputServicer;
        private readonly IAppContextMenuServicer appContextMenuServicer;
        public MainServicer(IMain main, IThemeServicer themeServicer, IInputServicer inputServicer, IAppContextMenuServicer appContextMenuServicer)
        {
            this.main = main;
            this.themeServicer = themeServicer;
            this.inputServicer = inputServicer;
            this.appContextMenuServicer = appContextMenuServicer;
        }
        public void Start()
        {
            main.OnStarted += Main_OnStarted;
            main.Run();
        }

        private void Main_OnStarted(object sender, EventArgs e)
        {
            themeServicer.Init();
            inputServicer.Start();
            appContextMenuServicer.Init();
        }
    }
}
