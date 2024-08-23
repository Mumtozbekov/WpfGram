using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace WpfGram
{
    /// <summary>
    /// Логика взаимодействия для MediaViewerWindow.xaml
    /// </summary>
    public partial class MediaViewerWindow : Window
    {


        public ObservableCollection<TdApi.File> Photos
        {
            get { return (ObservableCollection<TdApi.File>)GetValue(PhotosProperty); }
            set { SetValue(PhotosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PhotosProperty =
            DependencyProperty.Register("Photos", typeof(ObservableCollection<TdApi.File>), typeof(MediaViewerWindow), new PropertyMetadata(null));


        public MediaViewerWindow()
        {
            InitializeComponent();
            Photos = new();
        }
        public MediaViewerWindow(TdApi.File photo) : this()
        {
            Photos.Add(photo);
        }

        private void CenteredListbox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void CenteredListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (imageList.SelectedIndex > 0)
                imageList.SelectedIndex--;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (imageList.SelectedIndex < imageList.Items.Count - 1)
                imageList.SelectedIndex++;
        }
    }
}
