using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfGram.Utils
{
    public static class ScrollExt
    {
        public static readonly DependencyProperty SmoothScrollingProperty =
            DependencyProperty.RegisterAttached("SmoothScrolling", typeof(bool), typeof(ScrollExt),
                                                new UIPropertyMetadata(false, SmoothScrollingChanged));

        public static bool GetSmoothScrolling(DependencyObject o)
            => (bool)o.GetValue(SmoothScrollingProperty);

        public static void SetSmoothScrolling(DependencyObject o, bool val)
            => o.SetValue(SmoothScrollingProperty, val);

        private static void SmoothScrollingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is VirtualizingPanel || o is ItemsControl)
            {
                var enum_type = typeof(Window).Assembly.GetType("System.Windows.Controls.ScrollUnit");
                var field_flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
                var field = typeof(VirtualizingPanel).GetField("ScrollUnitProperty", field_flags);
                if (enum_type != null && field?.GetValue(null) is DependencyProperty dp)
                {
                    o.SetValue(dp, Enum.Parse(enum_type, (bool)e.NewValue ? "Pixel" : "Item"));
                }
                else if (o is VirtualizingPanel)
                {
                    var prop_flags = BindingFlags.NonPublic | BindingFlags.Instance;
                    var prop = o.GetType().GetProperty("IsPixelBased", prop_flags);
                    prop?.SetValue(o, (bool)e.NewValue, null);
                }
            }
        }
    }
}
