using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controls;
using UI.Models;
using UI.Models.Category;
using UI.Views;

namespace UI.ViewModels
{
    public class CategoryPageVM : CategoryPageModel
    {
        private readonly ICategorys categorys;
        private readonly MainViewModel mainVM;
        private readonly IAppData appData;
        public Command GotoAppListCommand { get; set; }
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

        public CategoryPageVM(ICategorys categorys, MainViewModel mainVM, IAppData appData)
        {
            this.categorys = categorys;
            this.mainVM = mainVM;
            this.appData = appData;

            GotoAppListCommand = new Command(new Action<object>(OnGotoAppList));
            EditCommand = new Command(new Action<object>(OnEdit));
            EditDoneCommand = new Command(new Action<object>(OnEditDone));
            EditCloseCommand = new Command(new Action<object>(OnEditClose));
            DelCommand = new Command(new Action<object>(OnDel));


            LoadData();

            PropertyChanged += CategoryPageVM_PropertyChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            PropertyChanged -= CategoryPageVM_PropertyChanged;
            Data = null;
        }
        private void OnDel(object obj)
        {
            if (SelectedItem == null)
            {
                return;
            }
            var category = categorys.GetCategory(SelectedItem.Data.ID);
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
                categorys.SaveChanged();

                //  从界面移除

                Data.Remove(SelectedItem);
                if (Data.Count == 0)
                {
                    Data = new System.Collections.ObjectModel.ObservableCollection<CategoryModel>();
                }
                mainVM.Toast("分类已删除", Controls.Window.ToastType.Success);

            }
        }
        private void OnEditClose(object obj)
        {
            EditVisibility = System.Windows.Visibility.Collapsed;
        }
        private void OnEditDone(object obj)
        {
            //var c = categorys.GetCategory(123);
            if (string.IsNullOrEmpty(EditName))
            {
                IsEditError = true;
                EditErrorText = "分类名称不能为空";
                return;
            }
            if (string.IsNullOrEmpty(EditIconFile))
            {
                mainVM.Toast("请选择一个分类图标", Controls.Window.ToastType.Error, Controls.Base.IconTypes.CriticalErrorSolid);
                return;
            }
            if (EditIconFile.IndexOf("pack://") ==-1 && new FileInfo(EditIconFile).Length > 1000000)
            {
                mainVM.Toast("图标文件不能超过1MB", Controls.Window.ToastType.Error, Controls.Base.IconTypes.CriticalErrorSolid);
                return;
            }

            if (IsCreate)
            {
                //  创建分类

                //  判断重复分类名称
                if (Data.Where(m => m.Data.Name == EditName).Any())
                {
                    mainVM.Toast("分类名称已存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.CriticalErrorSolid);
                    return;
                }

                mainVM.Toast("创建完成", Controls.Window.ToastType.Success);

                EditVisibility = System.Windows.Visibility.Collapsed;

                var res = categorys.Create(new Core.Models.CategoryModel()
                {
                    Name = EditName,
                    IconFile = EditIconFile,
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
                if (Data.Where(m => m.Data.Name == EditName && m.Data.ID != SelectedItem.Data.ID).Any())
                {
                    mainVM.Toast("分类名称已存在", Controls.Window.ToastType.Error, Controls.Base.IconTypes.CriticalErrorSolid);
                    return;
                }
                if (EditName == SelectedItem.Data.Name && EditIconFile == SelectedItem.Data.IconFile)
                {
                    mainVM.Toast("没有修改");
                    EditVisibility = System.Windows.Visibility.Collapsed;
                    return;
                }
                mainVM.Toast("已更新", Controls.Window.ToastType.Success);

                var category = categorys.GetCategory(SelectedItem.Data.ID);
                if (category != null)
                {
                    category.Name = EditName;
                    category.IconFile = EditIconFile;
                    categorys.Update(category);
                    categorys.SaveChanged();
                }

                var item = Data.Where(m => m.Data.ID == SelectedItem.Data.ID).FirstOrDefault();

                var changedItem = new CategoryModel()
                {
                    Count = item.Count,
                    Data = new Core.Models.CategoryModel()
                    {
                        ID = item.Data.ID,
                        Name = EditName,
                        IconFile = EditIconFile,
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

        private void OnEdit(object obj)
        {
            EditVisibility = System.Windows.Visibility.Visible;
            IsCreate = obj == null;

            if (obj != null)
            {
                var select = obj as CategoryModel;
                EditName = select.Data.Name;
                EditIconFile = select.Data.IconFile;
            }
            else
            {
                EditName = "";
                EditIconFile = "";
            }
        }

        private void CategoryPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem))
            {
                //GotoAppList();
            }
        }

        private void OnGotoAppList(object obj)
        {
            GotoAppList();
        }

        private void LoadData()
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
        }

        private void GotoAppList()
        {
            if (SelectedItem != null)
            {
                mainVM.Data = SelectedItem;
                mainVM.Uri = nameof(CategoryAppListPage);
                SelectedItem = null;
            }
        }
    }
}
