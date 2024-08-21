using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfGram.Utils
{
    public class UIElementExtensions
    {
        #region VerticalOffset attached property

        /// <summary>
        /// Gets the vertical offset value
        /// </summary>
        public static double GetVerticalOffset(DependencyObject depObj)
        {
            return (double)depObj.GetValue(VerticalOffsetProperty);
        }

        /// <summary>
        /// Sets the vertical offset value
        /// </summary>
        public static void SetVerticalOffset(DependencyObject depObj, double value)
        {
            depObj.SetValue(VerticalOffsetProperty, value);
        }

        /// <summary>
        /// VerticalOffset attached property
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double),
            typeof(UIElementExtensions), new PropertyMetadata(0.0, OnVerticalOffsetPropertyChanged));

        private static readonly DependencyProperty VerticalScrollBarProperty =
    DependencyProperty.RegisterAttached("VerticalScrollBar", typeof(ScrollBar),
    typeof(UIElementExtensions), new PropertyMetadata(null));

        #endregion
        public static readonly DependencyProperty ScrollChangedCommandProperty = DependencyProperty.RegisterAttached(
            "ScrollChangedCommand", typeof(ICommand), typeof(UIElementExtensions),
            new PropertyMetadata(default(ICommand), OnScrollChangedCommandChanged));

        private static void OnScrollChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListBox listBox = d as ListBox;
            if (listBox == null)
                return;
            if (e.NewValue != null)
            {
                listBox.Loaded += UIElementGridOnLoaded;
                //UIElementGridOnLoaded(listBox, null);
            }
            else if (e.OldValue != null)
            {
                listBox.Loaded -= UIElementGridOnLoaded;
            }
        }

        private static void UIElementGridOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            DependencyObject listBox = sender as ListBox;
            if (listBox == null)
                return;

            ScrollViewer scrollViewer = UIHelper.FindChildren<ScrollViewer>(listBox).FirstOrDefault();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;
            }
        }

        private static void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ListBox listBox = UIHelper.FindParent<ListBox>(sender as ScrollViewer);
            if (listBox != null)
            {
                ICommand command = GetScrollChangedCommand(listBox);
                command.Execute(sender);
            }
        }

        public static void SetScrollChangedCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(ScrollChangedCommandProperty, value);
        }

        public static ICommand GetScrollChangedCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(ScrollChangedCommandProperty);
        }

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d,
    DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer sv = d as ScrollViewer;
            if (sv != null)
            {
                // check whether we have a reference to the vertical scrollbar
                if (sv.GetValue(VerticalScrollBarProperty) == null)
                {
                    // if not, handle LayoutUpdated, which will be invoked after the
                    // template is applied and extract the scrollbar
                    sv.LayoutUpdated += (s, ev) =>
                    {
                        if (sv.GetValue(VerticalScrollBarProperty) == null)
                        {
                            GetScrollBarsForScrollViewer(sv);
                        }
                    };
                }
                else
                {
                    // update the scrollviewer offset
                    sv.ScrollToVerticalOffset((double)e.NewValue);
                }
            }
        }
        private static void GetScrollBarsForScrollViewer(ScrollViewer scrollViewer)
        {
            ScrollBar scroll = GetScrollBar(scrollViewer, Orientation.Vertical);
            if (scroll != null)
            {
                // save a reference to this scrollbar on the attached property
                scrollViewer.SetValue(VerticalScrollBarProperty, scroll);

                // scroll the scrollviewer
                scrollViewer.ScrollToVerticalOffset(
                  UIElementExtensions.GetVerticalOffset(scrollViewer));


                // handle the changed event to update the exposed VerticalOffset
                scroll.ValueChanged += (s, e) =>
                {
                    SetVerticalOffset(scrollViewer, e.NewValue);
                };
            }
        }

        /// <summary>
        /// Searches the descendants of the given element, looking for a scrollbar
        /// with the given orientation.
        /// </summary>
        private static ScrollBar GetScrollBar(FrameworkElement fe, Orientation orientation)
        {
            return UIHelper.FindChildren<ScrollBar>(fe).FirstOrDefault();
            

        }
    }

}
