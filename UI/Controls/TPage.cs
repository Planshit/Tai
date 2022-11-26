using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UI.Controls
{
    public class TPage : Page
    {
        /// <summary>
        /// 是否填充页面而不使用自适应滚动
        /// </summary>
        public bool IsFillPage
        {
            get { return (bool)GetValue(IsFillPageDependencyProperty); }
            set { SetValue(IsFillPageDependencyProperty, value); }
        }
        public static DependencyProperty IsFillPageDependencyProperty = DependencyProperty.Register("IsFillPage", typeof(bool), typeof(TPage));

        private PageContainer pageContainer;
        public TPage()
        {
            DefaultStyleKey = typeof(TPage);
            Loaded += TPage_Loaded;
        }

        private void TPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePageSize();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HandleEvent();
        }

        private void HandleEvent()
        {
            if (pageContainer != null)
            {
                pageContainer.SizeChanged -= Pc_SizeChanged;
            }

            if (IsFillPage)
            {
                //  查找父容器
                var parent = VisualTreeHelper.GetParent(this);
                if (parent != null)
                {
                    while (!(parent is PageContainer))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }

                pageContainer = parent as PageContainer;
                if (pageContainer != null)
                {
                    pageContainer.SizeChanged += Pc_SizeChanged;
                }
            }
        }

        private void Pc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePageSize();
        }

        private void UpdatePageSize()
        {
            Width = pageContainer.ActualWidth;
            Height = pageContainer.ActualHeight;

            Debug.WriteLine("UpdatePageSize");
        }
    }
}
