using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для CenteredListBox.xaml
    /// </summary>
    public partial class CenteredListBox : ListBox
    {
        ScrollViewer _scroll;
        double _scrollVerticalOffset;
        double _scrollHorizontalOffset;
        double Step=57.0;
        Storyboard scrollAnimation;
        int OldIndex = -1;
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(CenteredListBox), new PropertyMetadata(Orientation.Horizontal));



        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(CenteredListBox), new PropertyMetadata(57.0));


        public CenteredListBox()
        {
            InitializeComponent();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollAnimation = Resources["SlideRight"] as Storyboard;
            Storyboard.SetTarget(scrollAnimation, this);
            scrollAnimation.Completed += ScrollAnimation_Completed;
        }

        private void ScrollAnimation_Completed(object? sender, EventArgs e)
        {
            (this.RenderTransform as TranslateTransform).X = Step;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var newIndex = this.SelectedIndex;
            if (scrollAnimation != null)
            {

                scrollAnimation.Stop();

                Step = (this.RenderTransform as TranslateTransform).X + (OldIndex - newIndex) * ItemWidth;

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        (scrollAnimation.Children[0] as DoubleAnimation).To = Step;
                        scrollAnimation.Begin();
                        break;
                    case Orientation.Vertical:
                        //_scroll.ScrollToVerticalOffset(listBox.SelectedIndex - _scrollVerticalOffset);
                        break;
                    default:
                        break;
                }

            }
            OldIndex = this.SelectedIndex;
        }


        private void listBox_Loaded(object sender, RoutedEventArgs e)
        {
            ////_scroll = (ScrollViewer)(sender as FrameworkElement).GetTemplateChild("ScrollView1");
            //_scrollHorizontalOffset = Math.Floor(_scroll.ViewportWidth / 2);
            //_scrollVerticalOffset = Math.Floor(_scroll.ViewportHeight / 2);
            this.SelectedIndex = 0;


        }
    }
}

