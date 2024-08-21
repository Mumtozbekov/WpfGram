using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WpfGram.Utils;
namespace WpfGram.Controls
{
    public class KeepItemsInViewListBox : ListBox
    {


        public bool InverScroll
        {
            get { return (bool)GetValue(InverScrollProperty); }
            set { SetValue(InverScrollProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InverScroll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InverScrollProperty =
            DependencyProperty.Register("InverScroll", typeof(bool), typeof(KeepItemsInViewListBox), new PropertyMetadata(false));


        public event ScrollChangedEventHandler ScrollChanged;
        private ScrollViewer ScrollViewer { get; set; }

        #region Overrides of FrameworkElement

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (UIHelper.TryFindVisualChildElement(this, out ScrollViewer scrollViewer))
            {
                this.ScrollViewer = scrollViewer;
                this.ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (InverScroll && e.Delta != 0)
                e = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta * -1);

            base.OnPreviewMouseWheel(e);

        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollChanged?.Invoke(sender, e);
        }

        #endregion

        #region Overrides of ListView

        /// <inheritdoc />
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (this.ScrollViewer == null)
            {
                return;
            }

            double verticalOffset;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add when e.NewItems != null:
                    // Check if insert or add
                    verticalOffset = e.NewStartingIndex < this.ScrollViewer.VerticalOffset
                      ? this.ScrollViewer.VerticalOffset + e.NewItems.Count
                      : this.ScrollViewer.VerticalOffset;
                    break;
                case NotifyCollectionChangedAction.Remove when e.OldItems != null:
                    verticalOffset = this.ScrollViewer.VerticalOffset - e.OldItems.Count;
                    break;
                default:
                    verticalOffset = this.ScrollViewer.VerticalOffset;
                    break;
            }

            this.ScrollViewer?.ScrollToVerticalOffset(verticalOffset);
        }
        #endregion

    }
}

