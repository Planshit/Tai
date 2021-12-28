using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls;
using UI.Controls.Base;
using UI.Controls.Navigation.Models;
using UI.Models;
using UI.Views;

namespace UI.ViewModels
{
    public class MainViewModel : MainWindowModel
    {
        private readonly IServiceProvider serviceProvider;

        public Command OnSelectedCommand { get; set; }
        public Command GotoPageCommand { get; set; }

        public MainViewModel(
            IServiceProvider serviceProvider
            )
        {
            this.serviceProvider = serviceProvider;

            ServiceProvider = serviceProvider;
            Uri = nameof(IndexPage);
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

        public void SelectGroup(int ID)
        {
            NavSelectedItem = Items.Where(m => m.ID == ID).FirstOrDefault();
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
            IndexUriList.Add(nameof(IndexPage));
            IndexUriList.Add(nameof(DataPage));


            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                Icon = Controls.Base.IconTypes.AppIconDefaultList,
                Title = "概览",
                Uri = nameof(IndexPage),
                ID = -1

            });
            Items.Add(new Controls.Navigation.Models.NavigationItemModel()
            {
                Icon = Controls.Base.IconTypes.BIDashboard,
                Title = "详细",
                ID = 1,
                Uri = nameof(DataPage),

            });
            NavSelectedItem = Items[0];
        }

        public void Toast(string content, IconTypes icon = IconTypes.Accept)
        {
            ToastContent = content;
            ToastIcon = icon;
            IsShowToast = true;
        }
    }
}
