using Core.Librarys;
using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UI.Controls;
using UI.Controls.Select;
using UI.Models;
using UI.Models.Category;
using UI.Servicers;
using UI.Views;

namespace UI.ViewModels
{
    public class CategoryPageVM : CategoryPageModel
    {
        private readonly ICategorys categorys;
        private readonly MainViewModel mainVM;
        private readonly IAppData appData;
        private readonly IWebData _webData;
        private readonly IUIServicer _uiServicer;

        public Command GotoListCommand { get; set; }
        /// <summary>
        /// 打开编辑命令
        /// </summary>
        public Command EditCommand { get; set; }
        /// <summary>
        /// 完成编辑命令
        /// </summary>
        public Command EditDoneCommand { get; set; }
        /// <summary>
        /// 取消编辑
        /// </summary>
        public Command EditCloseCommand { get; set; }
        /// <summary>
        /// 删除分类
        /// </summary>
        public Command DelCommand { get; set; }
        /// <summary>
        /// 刷新
        /// </summary>
        public Command RefreshCommand { get; set; }
        /// <summary>
        /// 添加目录
        /// </summary>
        public Command AddDirectoryCommand { get; set; }
        /// <summary>
        /// 目录菜单命令
        /// </summary>
        public Command DirectoriesCommand { get; set; }


        public CategoryPageVM(
            ILocalizationServicer localizationServicer,
            ICategorys categorys,
            MainViewModel mainVM,
            IAppData appData,
            IWebData webData_,
            IUIServicer uIServicer_) : base(localizationServicer)
        {
            this.categorys = categorys;
            this.mainVM = mainVM;
            this.appData = appData;
            _webData = webData_;
            _uiServicer = uIServicer_;

            GotoListCommand = new Command(new Action<object>(OnGotoList));
            EditCommand = new Command(new Action<object>(OnEdit));
            EditDoneCommand = new Command(new Action<object>(OnEditDone));
            EditCloseCommand = new Command(new Action<object>(OnEditClose));
            DelCommand = new Command(new Action<object>(OnDel));
            RefreshCommand = new Command(new Action<object>(OnRefresh));
            AddDirectoryCommand = new Command(OnAddDirectory);
            DirectoriesCommand = new Command(OnDirectoriesCommand);

            LoadData();

            PropertyChanged += CategoryPageVM_PropertyChanged;
        }

        private void OnRefresh(object obj)
        {
            LoadData();
        }


        private void OnDel(object obj)
        {
            if (ShowType.Id == 0)
            {
                DelAppCategory();
            }
            else if (ShowType.Id == 1)
            {
                DelWebSiteCategory();
            }

        }
        private void OnEditClose(object obj)
        {
            EditVisibility = System.Windows.Visibility.Collapsed;
        }
        private void OnEditDone(object obj)
        {
            if (ShowType.Id == 0)
            {
                EditAppCategoryAction();
            }
            else if (ShowType.Id == 1)
            {
                EditWebSiteCategoryAction();
            }
        }

        private void OnAddDirectory(object obj)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "选择匹配目录";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.SelectedPath;
                    if (path.Last() != '\\')
                    {
                        path += "\\";
                    }
                    if (EditDirectories.Contains(path))
                    {
                        mainVM.Toast("目录已存在", Controls.Window.ToastType.Error);
                        return;
                    }
                    EditDirectories.Add(path);
                    mainVM.Toast("已添加", Controls.Window.ToastType.Success);
                }
            }
            catch (Exception ec)
            {
                Logger.Error(ec.ToString());
                mainVM.Toast("添加失败，请重试", Controls.Window.ToastType.Error);
            }
        }

        private void OnDirectoriesCommand(object obj)
        {
            string action = obj.ToString();
            switch (action)
            {
                case "remove":
                    if (string.IsNullOrEmpty(EditSelectedDirectory)) return;

                    EditDirectories.Remove(EditSelectedDirectory);
                    break;
            }
        }

        #region 编辑校验
        /// <summary>
        /// 编辑校验
        /// </summary>
        private bool IsEditVerify()
        {
            if (string.IsNullOrEmpty(EditName))
            {
                IsEditError = true;
                EditErrorText = "分类名称不能为空";
                return false;
            }
            if (string.IsNullOrEmpty(EditIconFile))
            {
                mainVM.Toast("请选择一个分类图标", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                return false;
            }
            if (string.IsNullOrEmpty(EditColor))
            {
                mainVM.Toast("请设置一个分类颜色", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                return false;
            }
            if (EditIconFile.IndexOf("pack://") == -1 && new FileInfo(EditIconFile).Length > 1000000)
            {
                mainVM.Toast("图标文件不能超过1MB", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                return false;
            }
            return true;
        }
        #endregion

        #region 应用分类操作
        #region 编辑应用分类
        private void EditAppCategoryAction()
        {
            try
            {
                if (!IsEditVerify()) return;

                string directoriesStr = JsonConvert.SerializeObject(EditDirectories.ToList());

                if (IsCreate)
                {
                    //  创建分类

                    //  判断重复分类名称
                    if (Data.Where(m => m.Data.Name == EditName).Any())
                    {
                        mainVM.Toast("分类名称已存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                        return;
                    }
                    //if (Data.Where(m => m.Data.Color == EditColor).Any())
                    //{
                    //    mainVM.Toast("颜色已存在，请重新选择", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                    //    return;
                    //}
                    mainVM.Toast("创建完成", Controls.Window.ToastType.Success);

                    EditVisibility = System.Windows.Visibility.Collapsed;

                    var res = categorys.Create(new Core.Models.CategoryModel()
                    {
                        Name = EditName,
                        IconFile = EditIconFile,
                        Color = EditColor,
                        IsDirectoryMath = EditIsDirectoryMath,
                        Directories = directoriesStr,
                    });

                    var item = new CategoryModel()
                    {
                        Data = res,
                        Count = 0
                    };
                    if (Data.Count == 0)
                    {
                        //Data=new System.Collections.ObjectModel.ObservableCollection<CategoryModel>()
                        var list = new System.Collections.ObjectModel.ObservableCollection<CategoryModel>();
                        list.Add(item);
                        Data = list;
                    }
                    else
                    {
                        Data.Add(item);
                    }

                }
                else
                {
                    //  编辑分类

                    //  判断重复分类名称
                    if (Data.Where(m => m.Data.Name == EditName && m.Data.ID != SelectedAppCategoryItem.Data.ID).Any())
                    {
                        mainVM.Toast("分类名称已存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                        return;
                    }
                    //if (Data.Where(m => m.Data.Color == EditColor && m.Data.ID != SelectedAppCategoryItem.Data.ID).Any())
                    //{
                    //    mainVM.Toast("颜色已存在，请重新选择", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                    //    return;
                    //}
                    //if (EditName == SelectedAppCategoryItem.Data.Name && EditIconFile == SelectedAppCategoryItem.Data.IconFile && EditColor == SelectedAppCategoryItem.Data.Color)
                    //{
                    //    mainVM.Toast("没有修改");
                    //    EditVisibility = System.Windows.Visibility.Collapsed;
                    //    return;
                    //}
                    mainVM.Toast("已更新", Controls.Window.ToastType.Success);
                    var category = categorys.GetCategory(SelectedAppCategoryItem.Data.ID);
                    if (category != null)
                    {
                        category.Name = EditName;
                        category.IconFile = EditIconFile;
                        category.Color = EditColor;
                        category.IsDirectoryMath = EditIsDirectoryMath;
                        category.Directories = directoriesStr;

                        categorys.Update(category);
                    }

                    var item = Data.Where(m => m.Data.ID == SelectedAppCategoryItem.Data.ID).FirstOrDefault();

                    var changedItem = new CategoryModel()
                    {
                        Count = item.Count,
                        Data = new Core.Models.CategoryModel()
                        {
                            ID = item.Data.ID,
                            Name = EditName,
                            IconFile = EditIconFile,
                            Color = EditColor,
                            IsDirectoryMath = EditIsDirectoryMath,
                            Directories = directoriesStr
                        }
                    };

                    int editItemIndex = Data.IndexOf(item);
                    if (editItemIndex != -1)
                    {
                        Data[editItemIndex] = changedItem;
                    }

                    EditVisibility = System.Windows.Visibility.Collapsed;
                }
            }
            catch (Exception ec)
            {
                Logger.Error(ec.ToString());
                mainVM.Toast("操作失败，请重试", Controls.Window.ToastType.Error);
            }
        }
        #endregion

        #region 删除分类
        private async void DelAppCategory()
        {
            if (SelectedAppCategoryItem == null)
            {
                return;
            }
            bool isConfirm = await _uiServicer.ShowConfirmDialogAsync("删除分类", "是否确认删除该分类？");
            if (isConfirm)
            {
                var category = categorys.GetCategory(SelectedAppCategoryItem.Data.ID);
                if (category != null)
                {
                    categorys.Delete(category);
                    var apps = appData.GetAppsByCategoryID(category.ID);
                    foreach (var app in apps)
                    {
                        app.CategoryID = 0;
                        app.Category = null;
                        appData.UpdateApp(app);
                    }

                    //  从界面移除

                    Data.Remove(SelectedAppCategoryItem);
                    if (Data.Count == 0)
                    {
                        Data = new System.Collections.ObjectModel.ObservableCollection<CategoryModel>();
                    }
                    mainVM.Toast("分类已删除", Controls.Window.ToastType.Success);

                }
            }
        }
        #endregion

        #endregion

        #region 网站分类操作
        #region 编辑网站分类
        private void EditWebSiteCategoryAction()
        {
            if (!IsEditVerify()) return;

            if (IsCreate)
            {
                //  创建分类

                //  判断重复分类名称
                if (WebCategoryData.Where(m => m.Data.Name == EditName).Any())
                {
                    mainVM.Toast("分类名称已存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                    return;
                }
                if (WebCategoryData.Where(m => m.Data.Color == EditColor).Any())
                {
                    mainVM.Toast("颜色已存在，请重新选择", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                    return;
                }

                var category = _webData.CreateWebSiteCategory(new Core.Models.Db.WebSiteCategoryModel()
                {
                    Color = EditColor,
                    IconFile = EditIconFile,
                    Name = EditName,
                });

                if (category != null)
                {
                    mainVM.Toast("创建完成", Controls.Window.ToastType.Success);
                    EditVisibility = System.Windows.Visibility.Collapsed;

                    var webCategory = new WebCategoryModel()
                    {
                        Count = 0,
                        Data = category,
                    };
                    if (WebCategoryData.Count == 0)
                    {
                        WebCategoryData = new System.Collections.ObjectModel.ObservableCollection<WebCategoryModel>()
                        {
                            webCategory
                        };
                    }
                    else
                    {
                        WebCategoryData.Add(webCategory);
                    }

                }
                else
                {
                    mainVM.Toast("创建失败", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                }



            }
            else
            {
                //  编辑分类

                //  判断重复分类名称
                if (WebCategoryData.Where(m => m.Data.Name == EditName && m.Data.ID != SelectedWebCategoryItem.Data.ID).Any())
                {
                    mainVM.Toast("分类名称已存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                    return;
                }
                if (WebCategoryData.Where(m => m.Data.Color == EditColor && m.Data.ID != SelectedWebCategoryItem.Data.ID).Any())
                {
                    mainVM.Toast("颜色已存在，请重新选择", Controls.Window.ToastType.Error, Controls.Base.IconTypes.ImportantBadge12);
                    return;
                }
                if (EditName == SelectedWebCategoryItem.Data.Name && EditIconFile == SelectedWebCategoryItem.Data.IconFile && EditColor == SelectedWebCategoryItem.Data.Color)
                {
                    mainVM.Toast("没有修改");
                    EditVisibility = System.Windows.Visibility.Collapsed;
                    return;
                }
                mainVM.Toast("已更新", Controls.Window.ToastType.Success);

                var category = SelectedWebCategoryItem.Data;
                category.Name = EditName;
                category.IconFile = EditIconFile;
                category.Color = EditColor;

                _webData.UpdateWebSiteCategory(category);

                var item = WebCategoryData.Where(m => m.Data.ID == category.ID).FirstOrDefault();
                var index = WebCategoryData.IndexOf(item);
                WebCategoryData[index] = new WebCategoryModel()
                {
                    Count = SelectedWebCategoryItem.Count,
                    Data = category
                };
                EditVisibility = System.Windows.Visibility.Collapsed;
            }
        }
        #endregion

        #region 删除分类
        private async void DelWebSiteCategory()
        {
            if (SelectedWebCategoryItem == null)
            {
                return;
            }
            bool isConfirm = await _uiServicer.ShowConfirmDialogAsync("删除分类", "是否确认删除该分类？");
            if (isConfirm)
            {
                _webData.DeleteWebSiteCategory(SelectedWebCategoryItem.Data);

                //  从界面移除
                WebCategoryData.Remove(SelectedWebCategoryItem);
                if (WebCategoryData.Count == 0)
                {
                    WebCategoryData = new System.Collections.ObjectModel.ObservableCollection<WebCategoryModel>();
                }
                mainVM.Toast("分类已删除", Controls.Window.ToastType.Success);
            }
        }
        #endregion
        #endregion

        private void OnEdit(object obj)
        {
            EditVisibility = System.Windows.Visibility.Visible;
            IsCreate = obj == null;

            if (obj != null)
            {
                var appCategory = obj as CategoryModel;
                var webCategory = obj as WebCategoryModel;

                EditName = appCategory == null ? webCategory.Data.Name : appCategory.Data.Name;
                EditIconFile = appCategory == null ? webCategory.Data.IconFile : appCategory.Data.IconFile;
                EditColor = string.IsNullOrEmpty(appCategory == null ? webCategory.Data.Color : appCategory.Data.Color) ? "#00FFAB" : appCategory == null ? webCategory.Data.Color : appCategory.Data.Color;
                if (ShowType.Id == 0)
                {
                    EditIsDirectoryMath = appCategory.Data.IsDirectoryMath;
                    if (appCategory.Data.Directories != null)
                    {
                        EditDirectories = new System.Collections.ObjectModel.ObservableCollection<string>(appCategory.Data.DirectoryList);
                    }
                    else
                    {
                        EditDirectories = new System.Collections.ObjectModel.ObservableCollection<string>();
                    }
                }
            }
            else
            {
                EditName = "";
                EditIconFile = "pack://application:,,,/Tai;component/Resources/Emoji/(1).png";
                EditColor = "#00FFAB";
                EditIsDirectoryMath = false;
                EditDirectories = new System.Collections.ObjectModel.ObservableCollection<string>();
            }
        }

        private void CategoryPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedAppCategoryItem))
            {
                //GotoAppList();
            }
        }

        private void OnGotoList(object obj)
        {
            GotoList();
        }

        private void LoadData()
        {
            Task.Run(() =>
            {
                Data = new System.Collections.ObjectModel.ObservableCollection<CategoryModel>();
                foreach (var item in categorys.GetCategories())
                {
                    Data.Add(new CategoryModel()
                    {
                        Count = appData.GetAppsByCategoryID(item.ID).Count,
                        Data = item
                    });
                }

                var webCategories = _webData.GetWebSiteCategories();
                var webCategoryData = new List<WebCategoryModel>();

                foreach (var item in webCategories)
                {
                    webCategoryData.Add(new WebCategoryModel()
                    {
                        Data = item,
                        Count = _webData.GetWebSitesCount(item.ID)
                    });
                }
                WebCategoryData = new System.Collections.ObjectModel.ObservableCollection<WebCategoryModel>(webCategoryData);
            });
        }

        private void GotoList()
        {
            if (ShowType.Id == 0 && SelectedAppCategoryItem != null)
            {
                mainVM.Data = SelectedAppCategoryItem;
                mainVM.Uri = nameof(CategoryAppListPage);
                SelectedAppCategoryItem = null;
            }
            else if (ShowType.Id == 1 && SelectedWebCategoryItem != null)
            {
                mainVM.Data = SelectedWebCategoryItem.Data;
                mainVM.Uri = nameof(CategoryWebSiteListPage);
                SelectedWebCategoryItem = null;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            PropertyChanged -= CategoryPageVM_PropertyChanged;
            Data = null;
        }
    }
}
