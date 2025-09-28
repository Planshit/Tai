using Core.Librarys;
using Core.Models.Config;
using Core.Models.Config.Link;
using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UI.Controls;
using UI.Models;
using UI.Servicers;

namespace UI.ViewModels
{
    public class SettingPageVM : SettingPageModel
    {
        private ConfigModel config;
        private readonly IAppConfig appConfig;
        private readonly MainViewModel mainVM;
        private readonly IData data;
        private readonly IWebData _webData;
        private readonly IUIServicer _uiServicer;
        public Command OpenURL { get; set; }
        public Command CheckUpdate { get; set; }
        public Command DelDataCommand { get; set; }
        public Command ExportDataCommand { get; set; }

        public SettingPageVM(
            ILocalizationServicer localizationServicer,
            IAppConfig appConfig,
            MainViewModel mainVM,
            IData data,
            IWebData webData,
            IUIServicer uiServicer_) : base(localizationServicer)
        {
            this.appConfig = appConfig;
            this.mainVM = mainVM;
            this.data = data;
            _webData = webData;
            _uiServicer = uiServicer_;

            OpenURL = new Command(new Action<object>(OnOpenURL));
            CheckUpdate = new Command(new Action<object>(OnCheckUpdate));
            DelDataCommand = new Command(new Action<object>(OnDelData));
            ExportDataCommand = new Command(new Action<object>(OnExportData));

            Init();
        }



        private void OnCheckUpdate(object obj)
        {
            try
            {
                CheckUpdateBtnVisibility = System.Windows.Visibility.Collapsed;
                string updaterExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Updater.exe");
                string updaterCacheExePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Update",
                    "Updater.exe");
                string updateDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Update");
                if (!Directory.Exists(updateDirPath))
                {
                    Directory.CreateDirectory(updateDirPath);
                }

                if (!File.Exists(updaterExePath))
                {
                    mainVM.Toast(Translated("page.settings.updaterRemoved"), Controls.Window.ToastType.Error, Controls.Base.IconTypes.None);
                    return;
                }
                File.Copy(updaterExePath, updaterCacheExePath, true);

                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Newtonsoft.Json.dll"), Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Update",
                    "Newtonsoft.Json.dll"), true);

                ProcessHelper.Run(updaterCacheExePath, new string[] { Version });
            }
            catch (Exception ex)
            {
                CheckUpdateBtnVisibility = System.Windows.Visibility.Visible;

                Logger.Error(ex.Message);
                mainVM.Toast(Translated("page.settings.updaterError"), Controls.Window.ToastType.Error, Controls.Base.IconTypes.None);
            }
        }

        private void OnOpenURL(object obj)
        {
            Process.Start(new ProcessStartInfo(obj.ToString()));
        }

        private void Init()
        {
            config = appConfig.GetConfig();

            Data = config.General;

            TabbarData = new System.Collections.ObjectModel.ObservableCollection<string>()
            {
                Translated("page.settings.category.general"),
                Translated("page.settings.category.associations"),
                Translated("page.settings.category.behavior"),
                Translated("page.settings.category.data"),
                Translated("page.settings.category.about")
            };

            PropertyChanged += SettingPageVM_PropertyChanged;

            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            DelDataStartMonthDate = DateTime.Now;
            DelDataEndMonthDate = DateTime.Now;

            ExportDataStartMonthDate = DateTime.Now;
            ExportDataEndMonthDate = DateTime.Now;
        }



        private void SettingPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Data))
            {
                if (TabbarSelectedIndex == 0)
                {
                    config.General = Data as GeneralModel;
                }
                else if (TabbarSelectedIndex == 1)
                {
                    if (Data != null)
                    {
                        var newData = new List<LinkModel>();
                        foreach (var item in Data as IEnumerable<object>)
                        {
                            newData.Add(item as LinkModel);
                            Debug.WriteLine(item.ToString());
                        }
                        config.Links = newData;
                    }

                }
                else if (TabbarSelectedIndex == 2)
                {
                    config.Behavior = Data as BehaviorModel;
                }

                appConfig.Save();
            }

            if (e.PropertyName == nameof(TabbarSelectedIndex))
            {
                if (TabbarSelectedIndex == 0)
                {
                    //  常规
                    Data = config.General;
                }
                else if (TabbarSelectedIndex == 1)
                {
                    //  关联
                    Data = config.Links;
                }
                else if (TabbarSelectedIndex == 2)
                {
                    //  行为
                    Data = config.Behavior;
                }
            }
        }

        private async void OnDelData(object obj)
        {
            if (DelDataStartMonthDate > DelDataEndMonthDate)
            {
                mainVM.Toast("时间范围选择错误", Controls.Window.ToastType.Error, Controls.Base.IconTypes.IncidentTriangle);
                return;
            }

            bool isConfirm = await _uiServicer.ShowConfirmDialogAsync("删除确认", "是否执行此操作？");
            if (isConfirm)
            {
                data.ClearRange(DelDataStartMonthDate, DelDataEndMonthDate);
                _webData.Clear(DelDataStartMonthDate, DelDataEndMonthDate);
                mainVM.Toast("操作已完成", Controls.Window.ToastType.Success);
            }
        }

        private void OnExportData(object obj)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择导出位置";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    data.ExportToExcel(dialog.SelectedPath, ExportDataStartMonthDate, ExportDataEndMonthDate);
                    _webData.Export(dialog.SelectedPath, ExportDataStartMonthDate, ExportDataEndMonthDate);
                    mainVM.Toast("导出数据完成", Controls.Window.ToastType.Success);
                }
            }
            catch (Exception ec)
            {
                Logger.Error(ec.ToString());
                mainVM.Toast("导出数据失败", Controls.Window.ToastType.Error, Controls.Base.IconTypes.IncidentTriangle);
            }
        }
    }
}
