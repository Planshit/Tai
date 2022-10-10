using Core.Librarys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.Controls.Base
{
    public class IconSelect : Control
    {
        public List<string> Icons
        {
            get { return (List<string>)GetValue(DaysProperty); }
            set { SetValue(DaysProperty, value); }
        }
        public static readonly DependencyProperty DaysProperty =
            DependencyProperty.Register("Icons",
                typeof(List<string>),
                typeof(IconSelect)
                );

        /// <summary>
        /// 图片链接
        /// </summary>
        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }
        public static readonly DependencyProperty URLProperty =
            DependencyProperty.Register("URL",
                typeof(string),
                typeof(IconSelect), new PropertyMetadata(new PropertyChangedCallback(OnURLPropertyChanged)));

        private static void OnURLPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as IconSelect;
            if (string.IsNullOrEmpty(control.URL))
            {
                control.Reset();
            }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen",
                typeof(bool),
                typeof(IconSelect), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as IconSelect;
            if (e.Property == IsOpenProperty)
            {
                control.HandleHook();
            }
        }
        private Win32API.LowLevelKeyboardProc mouseProc;
        private static IntPtr hookMouseID = IntPtr.Zero;
        private IntPtr hook;
        private bool IsFirstClick = false;
        private Border SelectContainer;


        #region hook
        private void HandleHook()
        {
            if (IsOpen)
            {
                //  设置键盘钩子
                hook = Win32API.SetMouseHook(mouseProc);
            }
            else
            {
                Win32API.UnhookWindowsHookEx(hook);
            }
        }
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0)
            {
                if (wParam == (IntPtr)Win32API.WM_LBUTTONDBLCLK || wParam == (IntPtr)Win32API.WM_WHEEL)
                {
                    if (!IsFirstClick)
                    {
                        bool isInControl = IsInControl();

                        if (!isInControl)
                        {
                            IsOpen = false;
                        }
                    }
                    IsFirstClick = false;
                }

            }

            return Win32API.CallNextHookEx(hookMouseID, nCode, wParam, lParam);
        }
        private bool IsInControl()
        {
            var p = Mouse.GetPosition(SelectContainer);
            if (p.X < 0 || p.Y < 0 || p.X > SelectContainer.ActualWidth || p.Y > SelectContainer.ActualHeight)
            {
                return false;
            }


            return true;
        }
        #endregion

        public Command ShowSelectCommand { get; set; }
        public Command FileSelectCommand { get; set; }

        public IconSelect()
        {
            DefaultStyleKey = typeof(IconSelect);
            URL = "pack://application:,,,/Tai;component/Resources/Emoji/(1).png";
            ShowSelectCommand = new Command(new Action<object>(OnShowSelect));
            FileSelectCommand = new Command(new Action<object>(OnFileSelect));
            mouseProc = HookCallback;

            Unloaded += IconSelect_Unloaded;
            LoadIcons();
        }

        private void LoadIcons()
        {
            var list = new List<string>();
            for (int i = 1; i < 45; i++)
            {
                list.Add($"pack://application:,,,/Tai;component/Resources/Emoji/({i}).png");
            }
            Icons = list;
        }

        private void Reset()
        {
            URL = Icons[0];
        }
        private void OnShowSelect(object obj)
        {
            IsOpen = true;
            IsFirstClick = true;
        }

        private void OnFileSelect(object obj)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            dlg.Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";


            bool? result = dlg.ShowDialog();


            if (result == true)
            {
                URL = dlg.FileName;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SelectContainer = GetTemplateChild("SelectContainer") as Border;
        }

        private void IconSelect_Unloaded(object sender, RoutedEventArgs e)
        {
            Win32API.UnhookWindowsHookEx(hook);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }


    }
}
