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
    public class ColorSelect : Control
    {
        public List<string> Colors
        {
            get { return (List<string>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }
        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register("Colors",
                typeof(List<string>),
                typeof(ColorSelect)
                );


        public string Color
        {
            get { return (string)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color",
                typeof(string),
                typeof(ColorSelect), new PropertyMetadata(new PropertyChangedCallback(OnColorPropertyChanged)));

        private static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColorSelect;
            if (string.IsNullOrEmpty(control.Color))
            {
                control.Color = control.Colors[0];
            }
            control.OnSelected?.Invoke(control, EventArgs.Empty);
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen",
                typeof(bool),
                typeof(ColorSelect), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColorSelect;
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
        public Command ColorSelectCommand { get; set; }
        public event EventHandler OnSelected;

        public ColorSelect()
        {
            DefaultStyleKey = typeof(ColorSelect);
            ShowSelectCommand = new Command(new Action<object>(OnShowSelect));
            ColorSelectCommand = new Command(new Action<object>(OnColorSelect));
            mouseProc = HookCallback;

            Unloaded += ColorSelect_Unloaded;
            LoadColors();
        }

        private void LoadColors()
        {
            var list = new List<string>()
            {
                "#00FFAB",
                "#017aff",
                "#6c47ff",
                "#39c0c8",
                "#f96300",
                "#f34971",
                "#ff9382",
                "#f5c900",
                "#cdad7a",
                "#aabb5d",
                "#000000",
                "#2B20D9"
            };


            Colors = list;
            Color = list[0];
        }
        private void OnShowSelect(object obj)
        {
            IsOpen = true;
            IsFirstClick = true;
        }

        private string HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private void OnColorSelect(object obj)
        {
            IsOpen = false;
            IsFirstClick = false;

            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color = HexConverter(colorDialog.Color);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SelectContainer = GetTemplateChild("SelectContainer") as Border;
        }

        private void ColorSelect_Unloaded(object sender, RoutedEventArgs e)
        {
            Win32API.UnhookWindowsHookEx(hook);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }


    }
}
