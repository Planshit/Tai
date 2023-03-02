using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using UI.Controls.Models;
using UI.Models;

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
                string oldUri = e.OldValue.ToString();
                if (control.PageCache.ContainsKey(oldUri))
                {
                    PageModel page = control.PageCache[oldUri];
                    page.ScrollValue = control.ScrollViewer.VerticalOffset;
                }
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
        private Dictionary<string, PageModel> PageCache;
        private bool IsBack = false;
        private ScrollViewer ScrollViewer;
        private Frame Frame;
        public PageContainer()
        {
            DefaultStyleKey = typeof(PageContainer);
            ProjectName = "UI";
            Historys = new List<string>();
            BackCommand = new Command(new Action<object>(OnBackCommand));
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
            PageCache = new Dictionary<string, PageModel>();

            CreateAnimations();
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            Frame = GetTemplateChild("Frame") as Frame;
            Frame.NavigationService.Navigated += NavigationService_Navigated;
            Frame.NavigationService.LoadCompleted += NavigationService_LoadCompleted;
            Loaded += PageContainer_Loaded;
        }

        private void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            animation.Begin();
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            Frame.NavigationService.RemoveBackEntry();
        }

        private void PageContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.XButton1)
            {
                Back();
            }
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

                //  从缓存中移除上一页

                var pageUri = Historys[preIndex];
                if (PageCache.ContainsKey(pageUri))
                {
                    var page = PageCache[pageUri];
                    var vm = page.Instance.DataContext as ModelBase;
                    vm?.Dispose();
                    page.Instance.Content = null;
                    page.Instance.DataContext = null;

                    PageCache.Remove(pageUri);
                }
                Historys.RemoveRange(preIndex, 1);

                IsBack = true;

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
                if (IndexUriList != null && IndexUriList.Contains(Uri))
                {
                    Historys.Clear();
                    Index = 0;
                    OldIndex = 0;
                    Historys.Add(Uri);
                    if (!IsBack)
                    {
                        ClearCache();
                    }
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
                PageModel page = GetPage();


                if (page != null)
                {
                    Content = page.Instance;

                    //  加入缓存
                    if (!PageCache.ContainsKey(Uri))
                    {
                        PageCache.Add(Uri, page);
                    }

                    //  滚动条位置处理
                    if (IsBack)
                    {
                        ScrollViewer.ScrollToVerticalOffset(page.ScrollValue);
                    }
                    else
                    {
                        ScrollViewer?.ScrollToVerticalOffset(0);
                    }

                    OnLoadPaged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Debug.WriteLine("找不到Page：" + Uri + "，请确认已被注入");
                }
            }
            IsBack = false;
        }

        private PageModel GetPage()
        {
            Page page = null;
            if (PageCache.ContainsKey(Uri))
            {
                return PageCache[Uri];
            }
            Type pageType = Type.GetType(ProjectName + ".Views." + Uri);
            if (pageType != null && ServiceProvider != null)
            {
                page = ServiceProvider.GetService(pageType) as Page;
            }
            var newPage = new PageModel()
            {
                Instance = page,
                ScrollValue = 0
            };

            return newPage;
        }

        private void ClearCache()
        {
            if (PageCache != null)
            {
                foreach (var key in PageCache.Keys)
                {
                    var page = PageCache[key];
                    var vm = page.Instance.DataContext as ModelBase;
                    vm?.Dispose();
                    page.Instance.Content = null;
                    page.Instance.DataContext = null;
                }
                PageCache.Clear();
            }
        }

        public void Disponse()
        {
            ClearCache();
            Content = null;
            DataContext = null;
            Instance = null;
        }

        private Storyboard animation;
        private void CreateAnimations()
        {
            ClipToBounds = true;

            ScaleTransform scaleTransform = new ScaleTransform();
            TranslateTransform translateTransform = new TranslateTransform();

            TransformGroup tfg = new TransformGroup();
            tfg.Children.Add(scaleTransform);
            tfg.Children.Add(translateTransform);
            RenderTransform = tfg;
            RenderTransformOrigin = new Point(.5, .5);

            animation = new Storyboard();

            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.From = 50;
            scrollAnimation.To = 0;
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.14));

            Storyboard.SetTarget(scrollAnimation, this);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[1].Y"));
            animation.Children.Add(scrollAnimation);
            //DoubleAnimation scaleXAnimation = new DoubleAnimation();
            //scaleXAnimation.From = .8;
            //scaleXAnimation.To = 1;
            //scaleXAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.14));

            //DoubleAnimation scaleYAnimation = new DoubleAnimation();
            //scaleYAnimation.From = .8;
            //scaleYAnimation.To = 1;
            //scaleYAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.14));

            //Storyboard.SetTarget(scaleXAnimation, this);
            //Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            //Storyboard.SetTarget(scaleYAnimation, this);
            //Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.Children[0].ScaleY"));

            //animation.Children.Add(scaleXAnimation);
            //animation.Children.Add(scaleYAnimation);
        }
    }
}
