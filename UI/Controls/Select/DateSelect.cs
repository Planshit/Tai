using Core.Librarys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using UI.Controls.DatePickerBar;

namespace UI.Controls.Select
{
    public class DateSelect : Control
    {
        public DateSelectType SelectType
        {
            get { return (DateSelectType)GetValue(SelectTypeProperty); }
            set { SetValue(SelectTypeProperty, value); }
        }
        public static readonly DependencyProperty SelectTypeProperty =
            DependencyProperty.Register("SelectType",
                typeof(DateSelectType),
                typeof(DateSelect),
                new PropertyMetadata(DateSelectType.Day)
                );
        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date",
                typeof(DateTime),
                typeof(DateSelect),
                new PropertyMetadata(DateTime.Now)
                );

        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }
        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year",
                typeof(int),
                typeof(DateSelect)
                );

        public int Month
        {
            get { return (int)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }
        public static readonly DependencyProperty MonthProperty =
            DependencyProperty.Register("Month",
                typeof(int),
                typeof(DateSelect)
                );

        public string Day
        {
            get { return (string)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }
        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day",
                typeof(string),
                typeof(DateSelect)
                );
        public string DateStr
        {
            get { return (string)GetValue(DateStrProperty); }
            set { SetValue(DateStrProperty, value); }
        }
        public static readonly DependencyProperty DateStrProperty =
            DependencyProperty.Register("DateStr",
                typeof(string),
                typeof(DateSelect)
                );
        public List<string> Days
        {
            get { return (List<string>)GetValue(DaysProperty); }
            set { SetValue(DaysProperty, value); }
        }
        public static readonly DependencyProperty DaysProperty =
            DependencyProperty.Register("Days",
                typeof(List<string>),
                typeof(DateSelect)
                );

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen",
                typeof(bool),
                typeof(DateSelect), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DateSelect;
            if (e.Property == IsOpenProperty)
            {
                control.HandleHook();
            }
        }

        private Win32API.LowLevelKeyboardProc mouseProc;
        private static IntPtr hookMouseID = IntPtr.Zero;
        private IntPtr hook;
        public Command ShowSelectCommand { get; set; }
        public Command SetYearCommand { get; set; }
        public Command SetMonthCommand { get; set; }
        public Command DoneCommand { get; set; }

        private bool IsFirstClick = false;
        private Border SelectContainer;
        public DateSelect()
        {
            DefaultStyleKey = typeof(DateSelect);

            ShowSelectCommand = new Command(new Action<object>(OnShowSelect));
            SetYearCommand = new Command(new Action<object>(OnSetYear));
            SetMonthCommand = new Command(new Action<object>(OnSetMonth));
            DoneCommand = new Command(new Action<object>(OnDone));

            MouseLeftButtonUp += Select_MouseLeftButtonUp;

            Year = Date.Year;

            Month = Date.Month;

            Day = Date.Day.ToString();

            mouseProc = HookCallback;

            Unloaded += DateSelect_Unloaded;

        }



        private void DateSelect_Unloaded(object sender, RoutedEventArgs e)
        {
            Win32API.UnhookWindowsHookEx(hook);
        }

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
        private void OnDone(object obj)
        {
            Date = new DateTime(Year, Month, int.Parse(Day));

            UpdateDateStr();

            IsOpen = false;
        }

        private void OnSetMonth(object obj)
        {
            int newMonth = Month + int.Parse(obj.ToString());
            if (newMonth < 1 || newMonth > 12)
            {
                return;
            }
            Month = newMonth;

            Day = "1";

            UpdateDays();


        }

        private void OnSetYear(object obj)
        {
            int newYear = Year + int.Parse(obj.ToString());
            if (newYear < 2020 || newYear > DateTime.Now.Year)
            {
                return;
            }
            Year = newYear;

            Day = "1";

            UpdateDays();
        }

        private void OnShowSelect(object obj)
        {
            IsOpen = true;

            IsFirstClick = true;
        }

        private void SelectContainer_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("加载");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SelectContainer = GetTemplateChild("SelectContainer") as Border;

            UpdateDays();

            UpdateDateStr();


        }


        private void Select_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void UpdateDays()
        {
            var list = new List<string>();
            int days = DateTime.DaysInMonth(Year, Month);
            for (int i = 1; i < days + 1; i++)
            {
                list.Add(i.ToString());
            }
            Days = list;

        }

        private void UpdateDateStr()
        {
            DateStr = Date.ToString("yyyy年MM月dd日");
            if (Date.Date == DateTime.Now.Date)
            {
                DateStr = "今天";
            }
            if (SelectType == DateSelectType.Month)
            {
                DateStr = Date.ToString("yyyy年MM月");

            }
            else if (SelectType == DateSelectType.Year)
            {
                DateStr = Date.ToString("yyyy年");

            }
        }
    }

}
