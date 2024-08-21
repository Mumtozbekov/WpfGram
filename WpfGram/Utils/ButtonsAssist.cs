using System.Windows;
using System.Windows.Controls;


namespace WpfGram.Utils
{
    public class ButtonsAssist : UIElement
    {


        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(ButtonsAssist), new PropertyMetadata(false));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static bool GetIsBusy(DependencyObject element) => (bool)element.GetValue(IsBusyProperty);
        public static void SetIsBusy(DependencyObject element, bool value) => element.SetValue(IsBusyProperty, value);

        // Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ButtonsAssist), new PropertyMetadata(false));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static bool GetIsIndeterminate(DependencyObject element) => (bool)element.GetValue(IsIndeterminateProperty);
        public static void SetIsIndeterminate(DependencyObject element, bool value) => element.SetValue(IsIndeterminateProperty, value);


        // Using a DependencyProperty as the backing store for ProgressValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(ButtonsAssist), new PropertyMetadata(0.0));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static double GetProgressValue(DependencyObject element) => (double)element.GetValue(ProgressValueProperty);
        public static void SetProgressValue(DependencyObject element, double value) => element.SetValue(ProgressValueProperty, value);

    }

}
