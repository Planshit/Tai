using Core.Librarys;
using Core.Models.Db;
using Core.Servicers.Instances;
using Core.Servicers.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;
using System.Windows.Markup;
using UI.Controls;
using UI.Controls.Charts.Model;
using UI.Controls.Select;
using UI.Models;
using UI.Servicers;

namespace UI.ViewModels
{
    public class WebSiteDetailPageVM : WebSiteDetailPageModel
    {
        private readonly IWebData _webData;
        private readonly MainViewModel _mainVM;
        private readonly IAppConfig _appConfig;
        private readonly IWebFilter _webFilter;
        private readonly IInputServicer _input;
        private readonly IUIServicer _uIServicer;

        public Command PageCommand { get; set; }

        private MenuItem _setCategoryMenuItem;
        private MenuItem _blockMenuItem;
        public WebSiteDetailPageVM(IWebData webData_, MainViewModel mainVM_, IAppConfig appConfig_, IWebFilter webFilter_, IInputServicer input_, IUIServicer uIServicer_)
        {
            _webData = webData_;
            _mainVM = mainVM_;
            _appConfig = appConfig_;
            _webFilter = webFilter_;
            _input = input_;
            _uIServicer = uIServicer_;

            Init();
        }

        private void OnPageCommand(object obj)
        {
            if (WebPageSelectedItem == null)
            {
                return;
            }

            string url = WebPageSelectedItem.Url.Url.IndexOf("://") == -1 ? "http://" + WebPageSelectedItem.Url.Url : WebPageSelectedItem.Url.Url;
            switch (obj.ToString())
            {
                case "Open":
                    Process.Start(url);
                    break;
                case "CopyURL":
                    Clipboard.SetText(WebPageSelectedItem.Url.Url);
                    break;
                case "CopyTitle":
                    Clipboard.SetText(WebPageSelectedItem.Url.Title);
                    break;
            }
        }

        private void Init()
        {
            if (!(_mainVM.Data is WebSiteModel))
            {
                _mainVM.Error("参数错误");
                return;
            }

            WebSite = _mainVM.Data as WebSiteModel;

            TabbarData = new System.Collections.ObjectModel.ObservableCollection<string>()
            {
                "按天","按周","按月","按年"
            };

            var weekOptions = new List<SelectItemModel>
            {
                new SelectItemModel()
                {
                    Name = "本周"
                },
                new SelectItemModel()
                {
                    Name = "上周"
                }
            };

            WeekOptions = weekOptions;
            SelectedWeek = weekOptions[0];
            MonthDate = DateTime.Now;
            TabbarSelectedIndex = 0;
            YearDate = DateTime.Now;
            ChartDate = DateTime.Now;

            Load();
            CreateContextMenu();

            PropertyChanged += WebSiteDetailPageVM_PropertyChanged;
            _input.OnKeyUpInput += _input_OnKeyUpInput;

            PageCommand = new Command(new Action<object>(OnPageCommand));

        }

        private void _input_OnKeyUpInput(object sender, System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.F5)
            {
                Load();
            }
        }

        private void Load()
        {
            IsIgnore = _webFilter.IsIgnore(WebSite.Domain);
            LoadData();
            LoadCategories();
        }

        private void CreateContextMenu()
        {
            WebSiteContextMenu = new System.Windows.Controls.ContextMenu();
            WebSiteContextMenu.Opened += WebSiteContextMenu_Opened;
            MenuItem open = new MenuItem();
            open.Header = "在浏览器打开网站";
            open.Click += Open_Click;

            MenuItem copyDomain = new MenuItem();
            copyDomain.Header = "复制域名";
            copyDomain.Click += CopyDomain_Click;

            MenuItem reLoadData = new MenuItem();
            reLoadData.Header = "刷新";
            reLoadData.Click += ReLoadData_Click;

            MenuItem clear = new MenuItem();
            clear.Header = "清空统计";
            clear.Click += ClearData_Click;

            _setCategoryMenuItem = new MenuItem();
            _setCategoryMenuItem.Header = "设置分类";

            MenuItem editAlias = new MenuItem();
            editAlias.Header = "编辑别名";
            editAlias.Click += EditAlias_ClickAsync;

            _blockMenuItem = new MenuItem();
            _blockMenuItem.Header = "忽略此网站";
            _blockMenuItem.Click += _blockMenuItem_Click;
            WebSiteContextMenu.Items.Add(open);
            WebSiteContextMenu.Items.Add(reLoadData);
            WebSiteContextMenu.Items.Add(new Separator());
            WebSiteContextMenu.Items.Add(_setCategoryMenuItem);
            WebSiteContextMenu.Items.Add(editAlias);
            WebSiteContextMenu.Items.Add(new Separator());
            WebSiteContextMenu.Items.Add(_blockMenuItem);
            WebSiteContextMenu.Items.Add(clear);

        }

        private async void EditAlias_ClickAsync(object sender, RoutedEventArgs e)
        {


            try
            {
                string input = await _uIServicer.ShowInputModalAsync("修改别名", "请输入别名", WebSite.Alias, (val) =>
                {
                    if (string.IsNullOrEmpty(val))
                    {
                        _mainVM.Error("请输入别名");
                        return false;
                    }
                    else if (val.Length > 10)
                    {
                        _mainVM.Error("别名最大长度为10位字符");
                        return false;
                    }
                    return true;
                });

                //  开始更新别名

                WebSite.Alias = input;
                WebSite = _webData.Update(WebSite);

                _mainVM.Success("别名已更新");
                Debug.WriteLine("输入内容：" + input);
            }
            catch
            {
                //  输入取消，无需处理异常
            }
        }

        private async void ClearData_Click(object sender, RoutedEventArgs e)
        {
            bool isConfirm = await _uIServicer.ShowConfirmDialogAsync("清空确认", "是否确定清空此站点的所有统计数据？");
            if (isConfirm)
            {
                _webData.Clear(WebSite.ID);
                Load();
                _mainVM.Success("操作已执行");
            }
        }

        private void CopyDomain_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(WebSite.Domain);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://" + WebSite.Domain);
        }

        private void ReLoadData_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void _blockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var config = _appConfig.GetConfig();

            if (IsIgnore)
            {
                //  取消忽略
                config.Behavior.IgnoreURLList.Remove(WebSite.Domain);
            }
            else
            {
                //  添加域名忽略
                config.Behavior.IgnoreURLList.Add(WebSite.Domain);
            }

            IsIgnore = !IsIgnore;

            _mainVM.Success("操作成功");
        }

        private void WebSiteContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            _setCategoryMenuItem.Items.Clear();
            foreach (var category in Categories)
            {
                var categoryMenu = new MenuItem();
                categoryMenu.Header = category.Name;
                categoryMenu.IsChecked = WebSite.CategoryID == category.Id;
                categoryMenu.Click += (s, ea) =>
                {
                    UpdateCategory(category.Id);
                };
                _setCategoryMenuItem.Items.Add(categoryMenu);
            }

            _blockMenuItem.Header = IsIgnore ? "取消忽略此网站" : "忽略此网站";

            _blockMenuItem.IsEnabled = !_webFilter.IsRegexIgnore(WebSite.Domain);
        }


        private void WebSiteDetailPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string[] updateNames = { nameof(TabbarSelectedIndex), nameof(ChartDate), nameof(SelectedWeek), nameof(MonthDate), nameof(YearDate) };
            if (updateNames.Contains(e.PropertyName))
            {
                LoadData();
            }
            if (e.PropertyName == nameof(Category) && Category != null)
            {
                UpdateCategory(Category.Id);
            }
        }

        /// <summary>
        /// 加载浏览数据
        /// </summary>
        private void LoadData()
        {
            Task.Run(() =>
            {
                var startDate = DateTime.Now;
                var endDate = DateTime.Now;
                string[] colNames = { };
                NameIndexStart = 0;
                if (TabbarSelectedIndex == 0)
                {
                    //  按天
                    startDate = endDate = ChartDate;
                }
                else if (TabbarSelectedIndex == 1)
                {
                    //  按周
                    var weekDateArr = SelectedWeek.Name == "本周" ? Time.GetThisWeekDate() : Time.GetLastWeekDate();
                    WeekDateStr = weekDateArr[0].ToString("yyyy年MM月dd日") + " 到 " + weekDateArr[1].ToString("yyyy年MM月dd日");
                    startDate = weekDateArr[0];
                    endDate = weekDateArr[1];
                    colNames = new string[] { "周一", "周二", "周三", "周四", "周五", "周六", "周日", };
                }
                else if (TabbarSelectedIndex == 2)
                {
                    //  按月
                    var dateArr = Time.GetMonthDate(MonthDate);
                    startDate = dateArr[0];
                    endDate = dateArr[1];
                    NameIndexStart = 1;
                }
                else if (TabbarSelectedIndex == 3)
                {
                    //  按年
                    startDate = new DateTime(YearDate.Year, 1, 1);
                    endDate = new DateTime(YearDate.Year, 12, DateTime.DaysInMonth(YearDate.Year, 12), 23, 59, 59);

                    colNames = new string[12];
                    for (int i = 0; i < 12; i++)
                    {
                        colNames[i] = (i + 1) + "月";
                    }
                }
                //  柱形图数据
                var list = _webData.GetBrowseDataStatistics(startDate, endDate, WebSite.ID);
                var chartData = new List<ChartsDataModel>
                {
                    new ChartsDataModel()
                    {
                        Name =!string.IsNullOrEmpty(WebSite.Alias) ? WebSite.Alias :WebSite.Title,
                        Values = list.Select(m=>m.Value).ToArray(),
                        ColumnNames= colNames,
                        Color=StateData.ThemeColor
                    }
                };
                ChartData = chartData;

                //  详细访问数据
                WebPageData = _webData.GetBrowseLogList(startDate, endDate, WebSite.ID).OrderByDescending(m => m.ID).ToList();
            });
        }
        /// <summary>
        /// 加载所有分类
        /// </summary>
        private void LoadCategories()
        {
            var data = _webData.GetWebSiteCategories();
            var list = new List<SelectItemModel>();
            foreach (var category in data)
            {
                var item = new SelectItemModel();
                item.Name = category.Name;
                item.Img = category.IconFile;
                item.Id = category.ID;
                list.Add(item);
            }

            Categories = list;
            Category = Categories.Where(m => m.Id == WebSite.CategoryID).FirstOrDefault();
        }

        /// <summary>
        /// 更新分类
        /// </summary>
        private void UpdateCategory(int categoryId_)
        {
            Task.Run(() =>
            {
                _webData.UpdateWebSitesCategory(new int[] { WebSite.ID }, categoryId_);
                WebSite.CategoryID = categoryId_;
                if (Category == null || categoryId_ != Category.Id)
                {
                    Category = Categories.Where(m => m.Id == WebSite.CategoryID).FirstOrDefault();
                }
            });
        }
    }
}
