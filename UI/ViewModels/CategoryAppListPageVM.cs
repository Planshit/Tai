using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls;
using UI.Models;
using UI.Models.Category;
using UI.Models.CategoryAppList;
using UI.Views;

namespace UI.ViewModels
{
    public class CategoryAppListPageVM : CategoryAppListPageModel
    {
        private readonly IAppData appData;
        private readonly MainViewModel mainVM;

        public Command ShowChooseCommand { get; set; }
        public Command ChoosedCommand { get; set; }
        public Command GotoDetailCommand { get; set; }
        public CategoryAppListPageVM(IAppData appData, MainViewModel mainVM)
        {
            this.appData = appData;
            this.mainVM = mainVM;

            ShowChooseCommand = new Command(new Action<object>(OnShowChoose));
            ChoosedCommand = new Command(new Action<object>(OnChoosed));
            GotoDetailCommand = new Command(new Action<object>(OnGotoDetail));

            LoadData();
        }

        private void OnGotoDetail(object obj)
        {
            if (SelectedItem != null)
            {
                mainVM.Data = SelectedItem;
                mainVM.Uri = nameof(DetailPage);
            }
        }

        private void OnChoosed(object obj)
        {
            ChooseVisibility = System.Windows.Visibility.Collapsed;

            var data = new List<AppModel>();
            foreach (var item in AppList)
            {

                var app = appData.GetApp(item.App.ID);

                if (item.IsChoosed)
                {
                    //  处理选中
                    if (app.CategoryID != Category.Data.ID)
                    {
                        app.CategoryID = Category.Data.ID;
                        app.Category = Category.Data;
                        appData.UpdateApp(app);
                    }

                    data.Add(app);
                }
                else
                {
                    //  处理取选
                    bool isHas = Data.Where(m => m.ID == item.App.ID).Any();
                    if (isHas)
                    {
                        app.CategoryID = 0;
                        app.Category = null;
                        appData.UpdateApp(app);
                    }
                }
            }
            Data = data;
            //LoadData();
        }

        private void OnShowChoose(object obj)
        {
            ChooseVisibility = System.Windows.Visibility.Visible;


            LoadApps();

        }


        private void LoadData()
        {
            Task.Run(() =>
            {
                Category = mainVM.Data as UI.Models.Category.CategoryModel;
                Data = appData.GetAppsByCategoryID(Category.Data.ID);

                //var appList = new List<ChooseAppModel>();
                //foreach (var item in appData.GetAllApps())
                //{
                //    var app = new ChooseAppModel();
                //    app.App = item;
                //    app.IsChoosed = item.CategoryID == Category.Data.ID;
                //    app.Value.Name = item.Name;
                //    app.Value.Img = item.IconFile;

                //    appList.Add(app);
                //}

                //AppList = appList;
            });
        }

        private void LoadApps()
        {
            Task.Run(() =>
            {
                //Category = mainVM.Data as UI.Models.Category.CategoryModel;
                if (Category == null)
                {
                    return;
                }
                var appList = new List<ChooseAppModel>();
                foreach (var item in appData.GetAllApps())
                {
                    var app = new ChooseAppModel();
                    app.App = item;
                    app.IsChoosed = item.CategoryID == Category.Data.ID;
                    app.Value.Name = item.Name;
                    app.Value.Img = item.IconFile;

                    appList.Add(app);
                }

                AppList = appList;
            });
        }
    }
}
