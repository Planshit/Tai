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
        #endregion

        private readonly string ProjectName;
        private List<string> Historys;
        private int Index = -1, OldIndex = -1;
        private Frame Frame;
        public PageContainer()
        {
            DefaultStyleKey = typeof(PageContainer);
            ProjectName = App.Current.GetType().Assembly.GetName().Name;
            Historys = new List<string>();
            BackCommand = new Command(new Action<object>(OnBackCommand));
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();
        }

        private void OnBackCommand(object obj)
        {
            if (Index - 1 >= 0)
            {
                OldIndex = Index;
                Index--;
                Uri = Historys[Index];
                Historys.RemoveRange(Index + 1, Historys.Count - (Index + 1));
            }
        }

        private void LoadPage()
        {
            if (Uri != string.Empty)
            {

                Type pageType = Type.GetType(ProjectName + ".Views." + Uri);
                Type pageVMType = Type.GetType(ProjectName + ".ViewModels." + Uri + "VM");
                if (pageType != null && ServiceProvider != null)
                {
                    var page = ServiceProvider.GetService(pageType) as Page;
                    if (page != null)
                    {
                        var pageVM = ServiceProvider.GetService(pageVMType);
                        if (pageVM != null)
                        {
                            page.DataContext = pageVM;
                        }
                        Content = page;
                        //处理历史记录
                        if (OldIndex == Index)
                        {
                            //新开
                            Historys.Add(Uri);
                            Index++;
                        }
                        OldIndex = Index;
                    }
                    else
                    {
                        Debug.WriteLine("找不到Page：" + Uri + "，请确认已被注入");
                    }

                }

            }
        }
    }
}
