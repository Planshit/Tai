using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls;
using UI.Controls.Base;
using UI.Controls.Navigation.Models;
using UI.Controls.Window;
using UI.Models;
using UI.Servicers;
using UI.Views;

namespace UI.ViewModels
{
    public class MainViewModel : MainWindowModel
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAppConfig appConfig;
        public Command OnSelectedCommand { get; set; }
        public Command GotoPageCommand { get; set; }

        private string[] pages = { nameof(IndexPage), nameof(ChartPage), nameof(DataPage), nameof(CategoryPage) };
        public MainViewModel(
            ILocalizationServicer localizationServicer,
            IServiceProvider serviceProvider,
            IAppConfig appConfig,
            IMain main
            ) : base(localizationServicer)
        {
            this.serviceProvider = serviceProvider;
            this.appConfig = appConfig;

            ServiceProvider = serviceProvider;

            OnSelectedCommand = new Command(new Action<object>(OnSelectedCommandHandle));
            GotoPageCommand = new Command(new Action<object>(OnGotoPageCommand));


            Items = new System.Collections.ObjectModel.ObservableCollection<Controls.Navigation.Models.NavigationItemModel>();

            PropertyChanged += MainViewModel_PropertyChanged;

            InitNavigation();
        }



        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Uri))
            {
                if (Uri == nameof(IndexPage))
                {
                    Data = null;
                }
            }
        }

        public void LoadDefaultPage()
        {
            int startPageIndex = appConfig.GetConfig().General.StartPage;
            NavSelectedItem = Items[startPageIndex];
            Uri = NavSelectedItem.Uri;
        }


        private void OnGotoPageCommand(object obj)
        {
            Uri = obj.ToString();
        }

        private void OnSelectedCommandHandle(object obj)
        {
            if (!string.IsNullOrEmpty(NavSelectedItem.Uri))
            {
                Uri = NavSelectedItem.Uri;
            }
        }

        private void InitNavigation()
        {
            IndexUriList = new List<string>();
            IndexUriList.AddRange(pages);

            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                UnSelectedIcon = Controls.Base.IconTypes.Home,
                SelectedIcon = IconTypes.HomeSolid,
                Title = Translated("main.overview"),
                Uri = nameof(IndexPage),
                ID = -1

            });
            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                UnSelectedIcon = Controls.Base.IconTypes.ZeroBars,
                SelectedIcon = IconTypes.FourBars,
                Title = Translated("main.statistics"),
                ID = 1,
                Uri = nameof(ChartPage),

            });
            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                UnSelectedIcon = Controls.Base.IconTypes.Calendar,
                SelectedIcon = IconTypes.CalendarSolid,
                //Icon = Controls.Base.IconTypes.BIDashboard,
                Title = Translated("main.details"),
                ID = 2,
                Uri = nameof(DataPage),

            });
            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                UnSelectedIcon = Controls.Base.IconTypes.EndPoint,
                SelectedIcon = IconTypes.EndPointSolid,
                Title = Translated("main.categories"),
                ID = 3,
                Uri = nameof(CategoryPage),

            });
        }

        public void Toast(string content, ToastType type = ToastType.Info, IconTypes icon = IconTypes.Accept)
        {
            ToastContent = content;
            ToastIcon = icon;
            ToastType = type;
            IsShowToast = true;

        }

        /// <summary>
        /// 显示一条错误的提醒消息
        /// </summary>
        /// <param name="message_">消息内容</param>
        public void Error(string message_)
        {
            Toast(message_, ToastType.Error, IconTypes.Error);
        }
        /// <summary>
        /// 显示一条普通提醒消息
        /// </summary>
        /// <param name="message_">消息内容</param>
        public void Info(string message_)
        {
            Toast(message_, ToastType.Info, IconTypes.Info);
        }
        /// <summary>
        /// 显示一条成功的提醒消息
        /// </summary>
        /// <param name="message_">消息内容</param>
        public void Success(string message_)
        {
            Toast(message_, ToastType.Success, IconTypes.Accept);
        }
    }
}
