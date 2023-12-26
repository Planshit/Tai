using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UI.Controls.Button;
using UI.Controls.Input;

namespace UI.Controls.Window
{
    public class DefaultWindow : System.Windows.Window
    {
        #region 1.依赖属性
        #region Dialog
        public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register("DialogTitle", typeof(string), typeof(DefaultWindow));
        /// <summary>
        /// Dialog title
        /// </summary>
        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }

        public static readonly DependencyProperty DialogMessageProperty = DependencyProperty.Register("DialogMessage", typeof(string), typeof(DefaultWindow));
        /// <summary>
        /// Dialog message
        /// </summary>
        public string DialogMessage
        {
            get { return (string)GetValue(DialogMessageProperty); }
            set { SetValue(DialogMessageProperty, value); }
        }
        #endregion
        #region InputModal
        public static readonly DependencyProperty InputModalValueProperty = DependencyProperty.Register("InputModalValue", typeof(string), typeof(DefaultWindow));
        /// <summary>
        /// Dialog title
        /// </summary>
        public string InputModalValue
        {
            get { return (string)GetValue(InputModalValueProperty); }
            set { SetValue(InputModalValueProperty, value); }
        }
        #endregion
        public static readonly DependencyProperty IsShowToastDeproperty = DependencyProperty.Register("IsShowToast", typeof(bool), typeof(DefaultWindow), new PropertyMetadata(false, new PropertyChangedCallback(OnIsShowToastChanged)));
        public bool IsShowToast { get { return (bool)GetValue(IsShowToastDeproperty); } set { SetValue(IsShowToastDeproperty, value); } }
        private static void OnIsShowToastChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as DefaultWindow;
            if (that != null)
            {
                if (that.IsShowToast)
                {
                    that.ShowToast();
                }
                else
                {
                    that.HideToast();
                }
            }
        }

        public static readonly DependencyProperty ToastIconProperty = DependencyProperty.Register("ToastIcon", typeof(Base.IconTypes), typeof(DefaultWindow));
        public Base.IconTypes ToastIcon
        {
            get
            {
                return (Base.IconTypes)GetValue(ToastIconProperty);
            }
            set { SetValue(ToastIconProperty, value); }
        }
        public static readonly DependencyProperty ToastContentProperty = DependencyProperty.Register("ToastContent", typeof(string), typeof(DefaultWindow));
        /// <summary>
        /// toast content
        /// </summary>
        public string ToastContent
        {
            get { return (string)GetValue(ToastContentProperty); }
            set { SetValue(ToastContentProperty, value); }
        }
        public static readonly DependencyProperty ToastTypeProperty = DependencyProperty.Register("ToastType", typeof(ToastType), typeof(DefaultWindow), new PropertyMetadata(ToastType.Info));
        /// <summary>
        /// toast type
        /// </summary>
        public ToastType ToastType
        {
            get { return (ToastType)GetValue(ToastTypeProperty); }
            set { SetValue(ToastTypeProperty, value); }
        }
        public static readonly DependencyProperty PageContainerProperty = DependencyProperty.Register("PageContainer", typeof(PageContainer), typeof(DefaultWindow), new PropertyMetadata(null, new PropertyChangedCallback(OnPageContainerChanged)));
        public PageContainer PageContainer { get { return (PageContainer)GetValue(PageContainerProperty); } set { SetValue(PageContainerProperty, value); } }
        #region 最小化按钮显示状态

        public static readonly DependencyProperty MinimizeVisibilityProperty = DependencyProperty.Register("MinimizeVisibility", typeof(Visibility), typeof(DefaultWindow));

        private static void OnPageContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = (DefaultWindow)d;
            if (that != null)
            {
                if (e.NewValue != null)
                {
                    that.IsCanBack = that.PageContainer.Index >= 1;

                    that.PageContainer.OnLoadPaged += (s, v) =>
                    {
                        var pc = s as PageContainer;
                        that.IsCanBack = pc?.Index >= 1;
                    };
                }
            }
        }

        /// <summary>
        /// 最小化按钮显示状态
        /// </summary>
        public Visibility MinimizeVisibility
        {
            get { return (Visibility)GetValue(MinimizeVisibilityProperty); }
            set { SetValue(MinimizeVisibilityProperty, value); }
        }
        #endregion   

        #region 最大化按钮显示状态

        public static readonly DependencyProperty MaximizeVisibilityProperty = DependencyProperty.Register("MaximizeVisibility", typeof(Visibility), typeof(DefaultWindow));

        /// <summary>
        /// 最大化按钮显示状态
        /// </summary>
        public Visibility MaximizeVisibility
        {
            get { return (Visibility)GetValue(MaximizeVisibilityProperty); }
            set { SetValue(MaximizeVisibilityProperty, value); }
        }
        #endregion

        #region 关闭按钮显示状态

        public static readonly DependencyProperty CloseVisibilityProperty = DependencyProperty.Register("CloseVisibility", typeof(Visibility), typeof(DefaultWindow));

        /// <summary>
        /// 关闭按钮显示状态
        /// </summary>
        public Visibility CloseVisibility
        {
            get { return (Visibility)GetValue(CloseVisibilityProperty); }
            set { SetValue(CloseVisibilityProperty, value); }
        }
        #endregion

        #region 拓展内容
        public static readonly DependencyProperty ExtElementProperty = DependencyProperty.Register("ExtElement", typeof(object), typeof(DefaultWindow));
        /// <summary>
        /// 拓展内容
        /// </summary>
        public object ExtElement
        {
            get { return GetValue(ExtElementProperty); }
            set { SetValue(ExtElementProperty, value); }
        }
        #endregion



        #region 是否穿透窗口

        public static readonly DependencyProperty IsThruWindowProperty = DependencyProperty.Register("IsThruWindow", typeof(bool), typeof(DefaultWindow), new PropertyMetadata(false));

        /// <summary>
        /// 是否穿透窗口
        /// </summary>
        public bool IsThruWindow
        {
            get { return (bool)GetValue(IsThruWindowProperty); }
            set { SetValue(IsThruWindowProperty, value); }
        }
        #endregion


        public static readonly DependencyProperty IsCanBackProperty = DependencyProperty.Register("IsCanBack", typeof(bool), typeof(DefaultWindow), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCanBackChanged)));

        private static void OnIsCanBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var that = d as DefaultWindow;
            if (that != null)
            {
                if (that.IsCanBack)
                {
                    VisualStateManager.GoToState(that, "CanBackState", true);
                }
                else
                {
                    VisualStateManager.GoToState(that, "Normal", true);
                }
            }
        }

        /// <summary>
        /// 是否可以返回
        /// </summary>
        public bool IsCanBack { get { return (bool)GetValue(IsCanBackProperty); } set { SetValue(IsCanBackProperty, value); } }
        #endregion

        private bool IsWindowClosed_ = false;
        /// <summary>
        /// 指示当前窗口是否已被关闭
        /// </summary>
        public bool IsWindowClosed { get { return IsWindowClosed_; } }

        private Border ToastBorder, Masklayer, DialogBorder, InputModalBorder;
        private Grid ToastGrid;
        private DispatcherTimer toastTimer;
        private bool IsDialogConfirm;
        private bool IsShowConfirmDialog, IsShowInputModal;
        private Button.Button CancelBtn, ConfirmBtn, InputModalCancelBtn, InputModalConfirmBtn;
        private InputBox InputModalInputBox;
        private string InputValue;
        private Func<string, bool> InputModalValidFnc;
        #region 3.初始化
        public DefaultWindow()
        {
            this.DefaultStyleKey = typeof(DefaultWindow);
            //添加当前窗体到窗体集合

            //命令绑定
            this.CommandBindings.Add(new CommandBinding(DefaultWindowCommands.MinimizeWindowCommand, OnMinimizeWindowCommand));
            this.CommandBindings.Add(new CommandBinding(DefaultWindowCommands.MaximizeWindowCommand, OnMaximizeWindowCommand));
            this.CommandBindings.Add(new CommandBinding(DefaultWindowCommands.RestoreWindowCommand, OnRestoreWindowCommand));
            this.CommandBindings.Add(new CommandBinding(DefaultWindowCommands.CloseWindowCommand, OnCloseWindowCommand));
            this.CommandBindings.Add(new CommandBinding(DefaultWindowCommands.BackCommand, OnBackCommand));

            //  获取程序图标
            if (Icon == null)
            {
                string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Icon = ToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(exeFilePath));
            }

            Loaded += window_Loaded;
            Unloaded += DefaultWindow_Unloaded;

            toastTimer = new DispatcherTimer();
            toastTimer.Tick += (s, e) =>
            {
                toastTimer.Stop();
                IsShowToast = false;
            };
            toastTimer.Interval = new TimeSpan(0, 0, 3);
        }

        private void DefaultWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= DefaultWindow_Unloaded;
            Loaded -= window_Loaded;

        }

        private void OnBackCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (PageContainer != null)
            {
                PageContainer.Back();
                if (PageContainer.Index == 0)
                {
                    IsCanBack = false;
                }
            }
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            OnSystemButtonsVisibility();
        }




        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ToastBorder = GetTemplateChild("ToastBorder") as Border;
            Masklayer = GetTemplateChild("Masklayer") as Border;
            ToastGrid = GetTemplateChild("ToastGrid") as Grid;
            DialogBorder = GetTemplateChild("DialogBorder") as Border;
            CancelBtn = GetTemplateChild("CancelBtn") as Button.Button;
            ConfirmBtn = GetTemplateChild("ConfirmBtn") as Button.Button;
            InputModalBorder = GetTemplateChild("InputModalBorder") as Border;
            InputModalCancelBtn = GetTemplateChild("InputModalCancelBtn") as Button.Button;
            InputModalConfirmBtn = GetTemplateChild("InputModalConfirmBtn") as Button.Button;
            InputModalInputBox = GetTemplateChild("InputModalInputBox") as InputBox;

            if (PageContainer != null)
            {
                IsCanBack = PageContainer.Index >= 1;
                if (IsCanBack)
                {
                    VisualStateManager.GoToState(this, "CanBackState", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }

            if (CancelBtn != null)
            {
                CancelBtn.Click += (e, c) =>
                {
                    IsDialogConfirm = false;
                    HideDialog();
                };
            }

            if (ConfirmBtn != null)
            {
                ConfirmBtn.Click += (e, c) =>
                {
                    IsDialogConfirm = true;
                    HideDialog();
                };
            }

            if (InputModalCancelBtn != null)
            {
                InputModalCancelBtn.Click += (e, c) =>
                {
                    IsDialogConfirm = false;
                    HideInputModal();
                };
            }

            if (InputModalConfirmBtn != null)
            {
                InputModalConfirmBtn.Click += (e, c) =>
                {
                    if (InputModalValidFnc != null && !InputModalValidFnc(InputValue))
                    {
                        return;
                    }
                    IsDialogConfirm = true;
                    HideInputModal();
                };
            }

            if (InputModalInputBox != null)
            {
                InputModalInputBox.TextChanged += (e, c) =>
                {
                    InputValue = InputModalInputBox.Text;
                };
            }
        }


        /// <summary>
        /// 根据窗口ResizeMode设置系统按钮的可视状态
        /// </summary>
        private void OnSystemButtonsVisibility()
        {
            switch (ResizeMode)
            {
                case ResizeMode.NoResize:
                    //仅显示关闭按钮
                    MinimizeVisibility = Visibility.Collapsed;
                    MaximizeVisibility = Visibility.Collapsed;
                    CloseVisibility = Visibility.Visible;
                    break;
                case ResizeMode.CanResize:
                    MinimizeVisibility = Visibility.Visible;
                    MaximizeVisibility = Visibility.Visible;
                    CloseVisibility = Visibility.Visible;
                    break;
                case ResizeMode.CanMinimize:
                    MinimizeVisibility = Visibility.Visible;
                    MaximizeVisibility = Visibility.Collapsed;
                    CloseVisibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsWindowClosed_ = true;
        }
        #region 5.命令

        private void OnCloseWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnRestoreWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void OnMaximizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void OnMinimizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        private void ShowToast()
        {
            ToastGrid.Visibility = Visibility.Visible;
            DialogBorder.Visibility = Visibility.Collapsed;
            if (!IsShowInputModal)
            {
                InputModalBorder.Visibility = Visibility.Collapsed;
            }
            ToastBorder.Visibility = Visibility.Visible;
            Storyboard storyboard = new Storyboard();

            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.From = -150;
            scrollAnimation.To = 0;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, ToastBorder);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.35));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.6;
            opacityAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(opacityAnimation, Masklayer);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            storyboard.Completed += (e, c) =>
            {
                toastTimer.Start();
                ToastGrid.MouseLeftButtonDown += ToastGrid_MouseLeftButtonDown;
            };
            storyboard.Children.Add(scrollAnimation);
            if (!IsShowInputModal)
            {
                storyboard.Children.Add(opacityAnimation);
            }
            storyboard.Begin();
        }

        private void ToastGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            toastTimer.Stop();
            IsShowToast = false;
            ToastGrid.MouseLeftButtonDown -= ToastGrid_MouseLeftButtonDown;
        }

        private void HideToast()
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.To = -150;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, ToastBorder);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.35));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0;
            opacityAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(opacityAnimation, Masklayer);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.25));

            storyboard.Children.Add(scrollAnimation);
            if (!IsShowInputModal)
            {
                storyboard.Children.Add(opacityAnimation);
                storyboard.Completed += (e, c) =>
                {
                    ToastGrid.Visibility = Visibility.Collapsed;
                };
            }

            storyboard.Begin();
        }


        #region Dialog
        public Task<bool> ShowConfirmDialogAsync(string title_, string message_)
        {
            IsShowConfirmDialog = true;
            ToastGrid.Visibility = Visibility.Visible;
            ToastBorder.Visibility = Visibility.Collapsed;
            DialogBorder.Visibility = Visibility.Visible;
            InputModalBorder.Visibility = Visibility.Collapsed;

            DialogMessage = message_;
            DialogTitle = title_;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.From = -150;
            scrollAnimation.To = 10;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, DialogBorder);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.6;
            opacityAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(opacityAnimation, Masklayer);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));
            storyboard.Children.Add(scrollAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();

            return Task.Run(() =>
             {

                 while (IsShowConfirmDialog)
                 {
                     Thread.Sleep(10);
                 }

                 return IsDialogConfirm;
             });
        }
        private void HideDialog()
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.To = -150;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, DialogBorder);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.12));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0;
            opacityAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(opacityAnimation, Masklayer);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));

            storyboard.Children.Add(scrollAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Completed += (e, c) =>
            {
                ToastGrid.Visibility = Visibility.Collapsed;
                IsShowConfirmDialog = false;
            };
            storyboard.Begin();
        }
        #endregion

        #region InputModal
        public Task<string> ShowInputModalAsync(string title_, string message_, string value_, Func<string, bool> validFnc_)
        {
            IsShowInputModal = true;
            ToastGrid.Visibility = Visibility.Visible;
            ToastBorder.Visibility = Visibility.Collapsed;
            DialogBorder.Visibility = Visibility.Collapsed;
            InputModalBorder.Visibility = Visibility.Visible;

            DialogMessage = message_;
            DialogTitle = title_;
            InputModalValue = value_;
            InputModalValidFnc = validFnc_;

            InputModalInputBox.Text = InputModalValue;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.From = -150;
            scrollAnimation.To = 10;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, InputModalBorder);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.6;
            opacityAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(opacityAnimation, Masklayer);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));
            storyboard.Children.Add(scrollAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Begin();

            return Task.Run(() =>
            {

                while (IsShowInputModal)
                {
                    Thread.Sleep(10);
                }

                if (IsDialogConfirm)
                {
                    return InputValue;
                }
                else
                {
                    throw new Exception("Input cancel");
                }
            });
        }
        private void HideInputModal()
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation scrollAnimation = new DoubleAnimation();
            scrollAnimation.To = -150;

            scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(scrollAnimation, InputModalBorder);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));
            scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.12));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0;
            opacityAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            Storyboard.SetTarget(opacityAnimation, Masklayer);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));

            storyboard.Children.Add(scrollAnimation);
            storyboard.Children.Add(opacityAnimation);
            storyboard.Completed += (e, c) =>
            {
                ToastGrid.Visibility = Visibility.Collapsed;
                IsShowInputModal = false;
            };
            storyboard.Begin();
        }
        #endregion

        #region 6.事件

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (IsThruWindow)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle; uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
        }
        #endregion

        #region win32
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
        #endregion
    }
}
