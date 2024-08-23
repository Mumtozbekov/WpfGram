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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Telegram.Td.Api;

using WpfGram.ViewModels;

namespace WpfGram.Controls
{
    /// <summary>
    /// Логика взаимодействия для MessageBubble.xaml
    /// </summary>
    public partial class MessageBubble : UserControl
    {
        private Border ContentBorder;
        private MessageViewModel _message;

        public bool ShowBackground
        {
            get { return (bool)GetValue(ShowBackgroundProperty); }
            set { SetValue(ShowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowBackgroundProperty =
            DependencyProperty.Register("ShowBackground", typeof(bool), typeof(MessageBubble), new PropertyMetadata(true));
        public bool ShowPhoto
        {
            get { return (bool)GetValue(ShowPhotoProperty); }
            set { SetValue(ShowPhotoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowPhoto.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPhotoProperty =
            DependencyProperty.Register("ShowPhoto", typeof(bool), typeof(MessageBubble), new PropertyMetadata(false));


        public MessageBubble()
        {
            InitializeComponent();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ContentBorder = GetTemplateChild(nameof(ContentBorder)) as Border;

            _message = DataContext as MessageViewModel;
            if (_message != null)
            {
                UpdateContent(_message);
            }
        }

        private void UpdateContent(MessageViewModel message)
        {
            switch (message.Message.Content)
            {
                case MessageText:
                    ContentBorder.Child = new TextMessageContent(message);
                    return;
                case MessagePhoto:
                    ContentBorder.Child = new PhotoMessageContent(message);
                    break;
                case MessageAnimatedEmoji:
                case MessageSticker:
                    {
                        ShowBackground = false;
                        ContentBorder.MaxWidth = 414;
                        if ((message.Message.Content is MessageSticker stick && stick.Sticker.Format is StickerFormatTgs) || message.Message.Content is MessageAnimatedEmoji)
                            ContentBorder.Child = new AnimatedStickerView()
                            {
                                DataContext = message.Message,
                                HorizontalContentAlignment = HorizontalAlignment.Left,
                                VerticalContentAlignment = VerticalAlignment.Bottom
                            };
                        else
                            ContentBorder.Child = new StickerMessageContent(message);
                    }
                    break;
                case MessageChatAddMembers:
                case MessageChatJoinByLink:
                case MessageChatJoinByRequest:
                case MessageChatDeleteMember:
                case MessageChatDeletePhoto:
                case MessageChatSetBackground:
                case MessageChatChangeTitle:
                    ShowBackground = false;
                    ShowPhoto = false;
                    HorizontalAlignment = HorizontalAlignment.Center;
                    ContentBorder.Child = new ChatUpdatesMessageContent(message);
                    break;
                    //case MessagePhoto:
                    //    return element.FindResource("photoMessage") as DataTemplate;
                    //default:
                    //    return element.FindResource("unsupportedMessage") as DataTemplate;
            }
        }
    }
}
