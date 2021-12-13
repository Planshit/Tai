using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.Controls
{
    public class PageContainer : Control
    {
        #region 依赖属性
        public List<string> IndexUriList
        {
            get { return (List<string>)GetValue(IndexUriListProperty); }
            set { SetValue(IndexUriListProperty, value); }
        }
        public static readonly DependencyProperty IndexUriListProperty =
            DependencyProperty.Register("IndexUriList", typeof(List<string>), typeof(PageContainer));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title",
                typeof(string),
                typeof(PageContainer));

        public IServiceProvider ServiceProvider
        {
            get { return (IServiceProvider)GetValue(ServiceProviderProperty); }
            set { SetValue(ServiceProviderProperty, value); }
        }
        public static readonly DependencyProperty ServiceProviderProperty =
            DependencyProperty.Register("ServiceProvider", typeof(IServiceProvider), typeof(PageContainer));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(PageContainer), new PropertyMetadata("Content undefined!"));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(string), typeof(PageContainer), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnUriChanged)));

        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PageContainer;
            if (e.NewValue != e.OldValue)
            {
                control.LoadPage();
            }
        }

        public bool IsShowTilteBar
        {
            get { return (bool)GetValue(IsShowTilteBarProperty); }
            set { SetValue(IsShowTilteBarProperty, value); }
        }
        public static readonly DependencyProperty IsShowTilteBarProperty =
            DependencyProperty.Register("IsShowTilteBar",
                typeof(bool),
                typeof(PageContainer), new PropertyMetadata(false, new PropertyChangedCallback(OnIsShowTitleBarChanged)));

        private static void OnIsShowTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PageContainer;
            if (e.NewValue != e.OldValue)
            {
                control.TitleBarVisibility = control.IsShowTilteBar ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility TitleBarVisibility
        {
            get { return (Visibility)GetValue(TitleBarVisibilityProperty); }
            set { SetValue(TitleBarVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TitleBarVisibilityProperty =
            DependencyProperty.Register("TitleBarVisibility",
                typeof(Visibility),
                typeof(PageContainer), new PropertyMetadata(Visibility.Collapsed));

        public Command BackCommand
        {
            get { return (Command)GetValue(BackCommandProperty); }
            set { SetValue(BackCommandProperty, value); }
        }
        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register("BackCommand",
                typeof(Command),
                typeof(PageContainer));


        public PageContainer Instance { get { return (PageContainer)GetValue(InstanceProperty); } set { SetValue(InstanceProperty, value); } }

        public static readonly DependencyProperty InstanceProperty = DependencyProperty.Register("Instance", typeof(PageContainer), typeof(PageContainer));
        #endregion

        /// <summary>
        /// 加载页面完成后发生
        /// </summary>
        public event EventHandler OnLoadPaged;

        private readonly string ProjectName;
        private List<string> Historys;
        public int Index = 0, OldIndex = 0;
        private Dictionary<string, object> PageCache;
        public PageContainer()
        {
            DefaultStyleKey = typeof(PageContainer);
            ProjectName = "UI";
            Historys = new List<string>();
            BackCommand = new Command(new Action<object>(OnBackCommand));
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            PageCache = new Dictionary<string, object>();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += PageContainer_Loaded;
        }

        private void PageContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
        }

        private void OnBackCommand(object obj)
        {
            Back();
        }

        public void Back()
        {
            if (Index - 1 >= 0)
            {
                OldIndex = Index;
                Index--;
                string uri = Historys[Index];

                int preIndex = Index + 1;
                int count = Historys.Count - (Index + 1);
                //  从缓存中移除之前的页面
                for (int i = preIndex; i < count; i++)
                {
                    var pageUri = Historys[i];
                    if (PageCache.ContainsKey(pageUri))
                    {
                        var page = PageCache[pageUri];
                        page = null;
                        PageCache.Remove(pageUri);
                    }
                }

                //  历史记录中移除
                Historys.RemoveRange(preIndex, count);

                Uri = uri;



            }
        }

        public void ClearHistorys()
        {
            Historys.Clear();
        }
        private void LoadPage()
        {
            if (Uri != string.Empty)
            {
                Page page = null;
                var pageVM = GetPageVM(Uri);
                Type pageType = Type.GetType(ProjectName + ".Views." + Uri);
                if (pageType != null && ServiceProvider != null)
                {
                    page = ServiceProvider.GetService(pageType) as Page;
                    if (page != null)
                    {

                        page.DataContext = pageVM;

                    }

                }

                if (page != null)
                {

                    Content = page;



                    if (IndexUriList != null && IndexUriList.Contains(Uri))
                    {
                        Historys.Clear();
                        Index = 0;
                        OldIndex = 0;
                        Historys.Add(Uri);
                        PageCache.Clear();
                    }
                    else
                    {
                        //处理历史记录
                        if (OldIndex == Index)
                        {
                            //新开
                            Historys.Add(Uri);
                            Index++;
                        }
                        OldIndex = Index;
                    }

                    //  加入缓存
                    PageCache.Add(Uri, pageVM);

                    OnLoadPaged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Debug.WriteLine("找不到Page：" + Uri + "，请确认已被注入");
                }


            }
        }


        private object GetPageVM(string uri)
        {
            if (PageCache.ContainsKey(uri))
            {
                return PageCache[uri];
            }
            else
            {
                Type pageVMType = Type.GetType(ProjectName + ".ViewModels." + Uri + "VM");
                if (ServiceProvider != null && pageVMType != null)
                {

                    var pageVM = ServiceProvider.GetService(pageVMType);

                    return pageVM;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
