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
using UI.Views;

namespace UI.ViewModels
{
    public class MainViewModel : MainWindowModel
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAppConfig appConfig;
        private readonly ILocalizationServicer localizationServicer;
        public Command OnSelectedCommand { get; set; }
        public Command GotoPageCommand { get; set; }

        private string[] pages = { nameof(IndexPage), nameof(ChartPage), nameof(DataPage), nameof(CategoryPage) };
        public MainViewModel(
            IServiceProvider serviceProvider,
            IAppConfig appConfig,
            IMain main,
            ILocalizationServicer localizationServicer
            )
        {
            this.serviceProvider = serviceProvider;
            this.appConfig = appConfig;
            this.localizationServicer = localizationServicer;

            ServiceProvider = serviceProvider;

            OnSelectedCommand = new Command(new Action<object>(OnSelectedCommandHandle));
            GotoPageCommand = new Command(new Action<object>(OnGotoPageCommand));


            Items = new System.Collections.ObjectModel.ObservableCollection<Controls.Navigation.Models.NavigationItemModel>();

            PropertyChanged += MainViewModel_PropertyChanged;

            InitNavigation();
            
            // 订阅文化改变事件
            localizationServicer.CultureChanged += LocalizationServicer_CultureChanged;
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
                Title = localizationServicer.GetString("Navigation_Overview"),
                Uri = nameof(IndexPage),
                ID = -1

            });
            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                UnSelectedIcon = Controls.Base.IconTypes.ZeroBars,
                SelectedIcon = IconTypes.FourBars,
                Title = localizationServicer.GetString("Navigation_Statistics"),
                ID = 1,
                Uri = nameof(ChartPage),

            });
            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                UnSelectedIcon = Controls.Base.IconTypes.Calendar,
                SelectedIcon = IconTypes.CalendarSolid,
                //Icon = Controls.Base.IconTypes.BIDashboard,
                Title = localizationServicer.GetString("Navigation_Details"),
                ID = 2,
                Uri = nameof(DataPage),

            });
            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                UnSelectedIcon = Controls.Base.IconTypes.EndPoint,
                SelectedIcon = IconTypes.EndPointSolid,
                Title = localizationServicer.GetString("Navigation_Categories"),
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
        
        private void LocalizationServicer_CultureChanged(object sender, EventArgs e)
        {
            // 重新初始化导航项以更新语言
            Items.Clear();
            InitNavigation();
        }
    }
}
