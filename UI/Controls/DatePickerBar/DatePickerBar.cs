using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UI.Controls.Navigation.Models;

namespace UI.Controls.DatePickerBar
{
    public class DatePickerBar : Control
    {
        public DatePickerShowType ShowType
        {
            get { return (DatePickerShowType)GetValue(ShowTypeProperty); }
            set { SetValue(ShowTypeProperty, value); }
        }
        public static readonly DependencyProperty ShowTypeProperty =
            DependencyProperty.Register("ShowType", typeof(DatePickerShowType), typeof(DatePickerBar), new PropertyMetadata(DatePickerShowType.Day, new PropertyChangedCallback(OnShowTypeChanged)));

        private static void OnShowTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DatePickerBar;
            if (e.NewValue != e.OldValue)
            {
                control.Render();
            }
        }

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(DatePickerBar), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DatePickerBar;
            if (e.NewValue != e.OldValue)
            {
                control.ScrollToActive(DateTime.Parse(e.NewValue.ToString()));
            }
        }


        public string SelectedDateString
        {
            get { return (string)GetValue(SelectedDateStringProperty); }
            set { SetValue(SelectedDateStringProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateStringProperty =
            DependencyProperty.Register("SelectedDateString", typeof(string), typeof(DatePickerBar));

        private StackPanel Container;
        private ScrollViewer ScrollViewer;
        private Dictionary<DateTime, DatePickerBarItem> ItemsDictionary;
        private List<DateTime> DateList;

        private int dataCount = 0;
        private int renderIndex = 0;
        //  选中标记块
        private Border ActiveBlock;
        //  动画
        Storyboard storyboard;
        public DatePickerBar()
        {
            DefaultStyleKey = typeof(DatePickerBar);
            ItemsDictionary = new Dictionary<DateTime, DatePickerBarItem>();
            DateList = new List<DateTime>();
            storyboard = new Storyboard();
        }



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Container = GetTemplateChild("Container") as StackPanel;
            ActiveBlock = GetTemplateChild("ActiveBlock") as Border;
            ScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            Render();


        }



        private void AddItem(DateTime date)
        {
            if (Container != null)
            {
                if (DateList.IndexOf(date) != -1)
                {
                    return;
                }
                renderIndex++;
                var control = new DatePickerBarItem();
                control.Title = date.Day.ToString();
                control.Date = date;
                control.MouseUp += (e, c) =>
                {
                    if (date > DateTime.Now.Date)
                    {
                        return;
                    }
                    ScrollToActive(date);
                };

                if (renderIndex == dataCount)
                {

                    control.Loaded += (e, c) =>
                    {

                        if (SelectedDate == DateTime.MinValue)
                        {
                            if (date == DateTime.Now.Date)
                            {
                                SelectedDate = DateTime.Now.Date;
                            }
                        }
                        else
                        {
                            ScrollToActive(SelectedDate);
                        }
                    };
                }
                //  后一天
                int next = DateList.IndexOf(date.AddDays(+1));

                if (next != -1)
                {
                    int index = next - 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    DateList.Insert(index, date);
                    ItemsDictionary.Add(date, control);
                    Container.Children.Insert(index, control);
                }
                else
                {
                    DateList.Add(date);
                    ItemsDictionary.Add(date, control);
                    Container.Children.Add(control);
                }
                //if (Data.IndexOf(item) == Data.Count - 1)
                //{
                //    control.Loaded += (e, c) =>
                //    {
                //        ScrollToActive();
                //    };
                //}
                //Container.Children.Add(control);
                //ItemsDictionary.Add(control);
            }
        }



        private void Render()
        {
            if (Container == null)
            {
                return;
            }
            dataCount = 0;
            renderIndex = 0;

            DateList.Clear();
            ItemsDictionary.Clear();
            Container.Children.Clear();

            if (ShowType == DatePickerShowType.Day)
            {
                dataCount = 41;

                //  前30天
                for (int i = 1; i < 31; i++)
                {
                    AddItem(DateTime.Now.Date.AddDays(-i));
                }

                AddItem(DateTime.Now.Date);

                //  后十天
                for (int i = 1; i < 11; i++)
                {
                    AddItem(DateTime.Now.Date.AddDays(+i));
                }
            }

            if (ShowType == DatePickerShowType.Month)
            {
                dataCount = 12;

                var nowDate = DateTime.Now;
                for (int i = 1; i < 13; i++)
                {
                    var date = new DateTime(nowDate.Year, i, 1);
                    AddMonthItem(date);
                }
            }



        }

        private void AddMonthItem(DateTime date)
        {

            if (Container != null)
            {
                if (DateList.IndexOf(date) != -1)
                {
                    return;
                }
                renderIndex++;

                var control = new DatePickerBarItem();
                control.Title = date.Month.ToString();
                control.Date = date;
                control.MouseUp += (e, c) =>
                {
                    if (date > DateTime.Now.Date)
                    {
                        return;
                    }
                    ScrollToActive(date);
                };

                if (renderIndex == dataCount)
                {
                    control.Loaded += (e, c) =>
                    {
                        if (SelectedDate == DateTime.MinValue)
                        {
                            if (date.Year == DateTime.Now.Date.Year
                            && date.Month == DateTime.Now.Date.Month)
                            {
                                SelectedDate = date;
                            }
                        }
                        else
                        {
                            ScrollToActive(SelectedDate);
                        }
                    };
                }


                DateList.Add(date);
                ItemsDictionary.Add(date, control);
                Container.Children.Add(control);
            }
        }

        private void ScrollToActive(DateTime date)
        {
           
            if (ItemsDictionary.Count == 0)
            {
                return;
            }
            if (ShowType == DatePickerShowType.Month)
            {
                date = new DateTime(date.Year, date.Month, 1);
            }
            else
            {
                date = new DateTime(date.Year, date.Month, date.Day);
            }

            if (!ItemsDictionary.ContainsKey(date))
            {
                return;
            }
            if (ItemsDictionary.ContainsKey(SelectedDate))
            {
                //  如果存在旧的选中，先取消
                ItemsDictionary[SelectedDate].IsSelected = false;
            }

            if (date != SelectedDate)
            {
                SelectedDate = date;
            }

            
            ItemsDictionary[date].IsSelected = true;


            SelectedDateString = SelectedDate.ToString("yyyy/MM");
            if (ShowType == DatePickerShowType.Month)
            {
                SelectedDateString = SelectedDate.ToString("yyyy");
            }
            var control = ItemsDictionary[SelectedDate];

            Point relativePoint = control.TransformToAncestor(Container).Transform(new Point(0, 0));

            double scrollTo = relativePoint.X - (ScrollViewer.ActualWidth / 2) + control.ActualWidth / 2;
            if (scrollTo < 0)
            {
                scrollTo = 0;
            }

            ScrollViewer.ScrollToHorizontalOffset(scrollTo);


            //if (oldSelectedIndex > ItemsDictionary.Count || ItemsDictionary.Count == 0)
            //{
            //    return;
            //}
            //  获取选中项

            //var item = ItemsDictionary[SelectedIndex];
            //var oldSelectedItem = ItemsDictionary[oldSelectedIndex];


            //  选中项的坐标
            //Point relativePoint = item.TransformToAncestor(this).Transform(new Point(0, 0));

            //  创建动画



            //storyboard.Children.Clear();
            //DoubleAnimation scrollAnimation = new DoubleAnimation();
            //double scrollX = relativePoint.X - 10;
            //scrollX = scrollX < 0 ? 0 : scrollX;
            //scrollAnimation.To = scrollX;

            //scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            //Storyboard.SetTarget(scrollAnimation, ActiveBlock);
            //Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("RenderTransform.Children[0].X"));

            //scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));


            //storyboard.Children.Add(scrollAnimation);


            //storyboard.Begin();
        }
    }
}
