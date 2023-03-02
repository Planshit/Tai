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
        private readonly IWebSiteContextMenuServicer _webSiteContext;
        private readonly IStatusBarIconServicer _statusBarIconServicer;
        public MainServicer(
            IMain main,
            IThemeServicer themeServicer,
            IInputServicer inputServicer,
            IAppContextMenuServicer appContextMenuServicer,
            IWebSiteContextMenuServicer webSiteContext_,
            IStatusBarIconServicer statusBarIconServicer_)
        {
            this.main = main;
            this.themeServicer = themeServicer;
            this.inputServicer = inputServicer;
            this.appContextMenuServicer = appContextMenuServicer;
            _webSiteContext = webSiteContext_;
            _statusBarIconServicer = statusBarIconServicer_;
        }
        public void Start()
        {
            _statusBarIconServicer.Init();

            main.OnStarted += Main_OnStarted;
            main.Run();
        }

        private void Main_OnStarted(object sender, EventArgs e)
        {
            themeServicer.Init();
            inputServicer.Start();
            appContextMenuServicer.Init();
            _webSiteContext.Init();
        }
    }
}
