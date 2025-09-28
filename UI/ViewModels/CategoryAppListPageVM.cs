using Core.Models;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls;
using UI.Models;
using UI.Models.Category;
using UI.Models.CategoryAppList;
using UI.Servicers;
using UI.Views;

namespace UI.ViewModels
{
    public class CategoryAppListPageVM : CategoryAppListPageModel
    {
        private readonly IAppData appData;
        private readonly MainViewModel mainVM;
        private List<ChooseAppModel> appList;
        public Command ShowChooseCommand { get; set; }
        public Command ChoosedCommand { get; set; }
        public Command GotoDetailCommand { get; set; }
        public Command SearchCommand { get; set; }
        public Command ChooseCloseCommand { get; set; }
        public Command DelCommand { get; set; }
        public CategoryAppListPageVM(ILocalizationServicer localizationServicer, IAppData appData, MainViewModel mainVM) : base(localizationServicer)
        {
            this.appData = appData;
            this.mainVM = mainVM;


            ShowChooseCommand = new Command(new Action<object>(OnShowChoose));
            ChoosedCommand = new Command(new Action<object>(OnChoosed));
            GotoDetailCommand = new Command(new Action<object>(OnGotoDetail));
            SearchCommand = new Command(new Action<object>(OnSearch));
            ChooseCloseCommand = new Command(new Action<object>(OnChooseClose));
            DelCommand = new Command(new Action<object>(OnDel));

            LoadData();

        }
        public override void Dispose()
        {
            base.Dispose();
            Data = null;
            AppList = null;
        }
        private void OnDel(object obj)
        {
            if (SelectedItem != null)
            {
                var list = Data.ToList();

                list.Remove(SelectedItem);

                var app = appData.GetApp(SelectedItem.ID);
                if (app != null)
                {
                    app.CategoryID = 0;
                    app.Category = null;
                    appData.UpdateApp(app);
                }

                Data = list;
            }
        }

        private void OnChooseClose(object obj)
        {
            ChooseVisibility = System.Windows.Visibility.Collapsed;
            SearchInput = "";
        }

        private void OnSearch(object obj)
        {
            string keyword = obj.ToString();

            if (keyword == "vscode")
            {
                keyword = "Visual Studio Code";
            }
            else if (keyword == "ps")
            {
                keyword = "Photoshop";
            }
            Search(keyword);
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

            SearchInput = "";

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
                appList = new List<ChooseAppModel>();

                if (Category == null)
                {
                    return;
                }
                foreach (var item in appData.GetAllApps())
                {
                    var app = new ChooseAppModel();
                    app.App = item;
                    app.IsChoosed = item.CategoryID == Category.Data.ID;
                    app.Value.Name = String.IsNullOrEmpty(item.Description) ? item.Name : item.Description;
                    app.Value.Img = item.IconFile;

                    if (app.IsChoosed || item.CategoryID == 0)
                    {
                        appList.Add(app);
                    }
                }
                appList = appList.OrderBy(m => m.App.Description).ToList();

                AppList = appList;
            });
        }

        private void Search(string keyword)
        {
            Task.Run(() =>
              {
                  if (!string.IsNullOrEmpty(keyword))
                  {
                      Debug.WriteLine(keyword);
                      keyword = keyword.ToLower();

                      //var list = appList.Where(m => m.App.Description.ToLower().IndexOf(keyword) != -1 || m.App.Name.ToLower().IndexOf(keyword) != -1).ToList();
                      var list = AppList.ToList();

                      foreach (var item in list)
                      {
                          item.Visibility = item.App.Description != null && item.App.Description.ToLower().IndexOf(keyword) != -1 || item.App.Name != null && item.App.Name.ToLower().IndexOf(keyword) != -1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                      }

                      return list;

                  }
                  else
                  {
                      var list = AppList.ToList();

                      foreach (var item in list)
                      {
                          item.Visibility = System.Windows.Visibility.Visible;
                      }

                      return list;

                  }
              })

      .ContinueWith(task =>
      {
          AppList = task.Result;

      }, TaskScheduler.FromCurrentSynchronizationContext());

        }
    }
}
