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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Telegram.Td.Api;

using WpfGram.Helpers;
using WpfGram.Utils;
using WpfGram.ViewModels;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для PhotoMessageContent.xaml
    /// </summary>
    public partial class PhotoMessageContent : UserControl
    {
        private Photo Photo;
        private MessageViewModel _message;

        private Image Preview;
        private Border BlurBorder;
        private Button DownloadButton;

        public bool IsSaving
        {
            get { return (bool)GetValue(IsSavingProperty); }
            set { SetValue(IsSavingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSaving.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSavingProperty =
            DependencyProperty.Register("IsSaving", typeof(bool), typeof(PhotoMessageContent), new PropertyMetadata(false));

        public PhotoMessageContent()
        {
            InitializeComponent();
        }
        public PhotoMessageContent(ViewModels.MessageViewModel message) : this()
        {
            _message = message;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Preview = GetTemplateChild(nameof(Preview)) as Image;
            DownloadButton = GetTemplateChild(nameof(DownloadButton)) as Button;
            if (DownloadButton != null)
                DownloadButton.Click += DownloadButtonClick;
            BlurBorder = GetTemplateChild(nameof(BlurBorder)) as Border;

            if (_message.Message.Content is MessagePhoto mp)
            {
                Photo = mp.Photo;
                if (Photo.Sizes.Length > 0)
                {

                    var f = Photo.Sizes[^1];
                    if (string.IsNullOrEmpty(f?.Photo?.Local?.Path) || !f.Photo.Local.IsDownloadingCompleted)
                        Preview.Source = Photo.Minithumbnail.Data.ToImage();
                    else
                    {
                        Preview.Source = new BitmapImage(new Uri(f.Photo.Local.Path));
                        BlurBorder.Effect = null;
                        DownloadButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {

                if (Photo?.Sizes?.Length > 0)
                {
                    var f = Photo.Sizes[^1];
                    if (f.Photo.Local.CanBeDownloaded)
                    {
                        IsSaving = true;
                        var imgPath = await TgClientHelper.DownloadFile(f.Photo.Id);
                        IsSaving = imgPath == "";
                        Preview.Source = new BitmapImage(new Uri(imgPath));

                        Photo.Sizes[^1].Photo.Local.Path = imgPath;
                        if (imgPath != null)
                        {
                            DownloadButton.Visibility = Visibility.Collapsed;
                            BlurBorder.Effect = null;
                        }
                    }
                }
            }
            catch { }


        }
        private void Preview_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var win = new MediaViewerWindow(Photo.Sizes[^1].Photo);
            win.Show();
        }
    }
}
