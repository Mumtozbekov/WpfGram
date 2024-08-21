using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Telegram.Td.Api;

using WpfGram.Utils;

using TdApi = Telegram.Td.Api;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для MessagePreviewer.xaml
    /// </summary>
    public partial class MessagePreviewer : UserControl
    {


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); /*OnSelectionChanged();*/ }
        }


        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MessagePreviewer), new PropertyMetadata(false));


        public MessagePreviewer()
        {
            InitializeComponent();

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DataContextChanged += OnDataContextChanged;
            OnDataContextChanged(null, new DependencyPropertyChangedEventArgs());
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Content.Foreground = Resources["SecondaryForeground"] as SolidColorBrush;
            MediaPreview.Visibility = Visibility.Collapsed;
            switch (DataContext)
            {
                case TdApi.MessagePhoto mp:
                    Content.Text = string.IsNullOrEmpty(mp.Caption.Text) ? " Photo" : mp.Caption.Text.Trim().Replace("\n", " ");
                    MediaPreview.Source = mp.Photo.Minithumbnail.Data.ToImage();
                    MediaPreview.Visibility = Visibility.Visible;
                    // Content.Foreground = Resources["LinksForeground"] as SolidColorBrush;

                    break;
                case TdApi.MessageVideo mv:
                    Content.Text = string.IsNullOrEmpty(mv.Caption.Text) ? " Video" : mv.Caption.Text.Trim().Replace(Environment.NewLine, " ");
                    MediaPreview.Source = mv.Video.Minithumbnail.Data.ToImage();
                    MediaPreview.Visibility = Visibility.Visible;
                    //Content.Foreground = Resources["LinksForeground"] as SolidColorBrush;

                    break;
                case TdApi.MessageDocument md:
                    Content.Text = "File";
                    break;
                case TdApi.MessageVoiceNote mv:
                    Content.Text = "Voice message";
                    //Content.Foreground = Resources["LinksForeground"] as SolidColorBrush;
                    break;
                case TdApi.MessageText mt:
                    Content.Text = mt.Text.Text.Trim().Replace(Environment.NewLine, " ");
                    break;
                case TdApi.MessageAnimatedEmoji ma:
                    Content.Text = ma.Emoji;
                    break;
                case TdApi.MessageCall mc:
                    {
                        switch (mc.DiscardReason)
                        {
                            case CallDiscardReasonMissed:
                                Content.Text = "Missed call";
                                break;
                            case CallDiscardReasonDeclined:
                                Content.Text = "Cancelled call";
                                break;
                        }
                    }
                    break;
                case TdApi.MessageSticker ms:
                    Content.Text = ms.Sticker.Emoji + " Sticker";
                    //  Content.Foreground = Resources["LinksForeground"] as SolidColorBrush;
                    break;
                case TdApi.MessageAnimation ma:
                    //     MediaPreview.Source = ma.Animation.Thumbnail.File..ToImage();

                    Content.Text = "GIF";
                    //  Content.Foreground = Resources["LinksForeground"] as SolidColorBrush;
                    break;

                default:
                    break;
            }
        }


    }
}
