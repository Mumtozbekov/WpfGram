
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using WpfGram.ViewModels;

using Telegram.Td.Api;


namespace WpfGram.Utils
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item == null) return null;

            if (item is MessageViewModel vm)
            {
                Message msg = vm?.Message;

                
                switch (msg.Content)
                {
                    case MessageText:
                        return element.FindResource("textMessage") as DataTemplate;
                    case MessageAnimatedEmoji:
                    case MessageSticker:
                        {
                            if ((msg.Content is MessageSticker stick && stick.Sticker.Format is StickerFormatTgs) || msg.Content is MessageAnimatedEmoji)
                                return element.FindResource("animatedMessage") as DataTemplate;
                            else
                                return element.FindResource("stickerMessage") as DataTemplate;
                        }
                    //case TdApi.MessageAlbum:
                    //    return element.FindResource("albumMessage") as DataTemplate;
                    //case MessageDocument:
                    //    return element.FindResource("fileMessage") as DataTemplate;
                    //case MessagePhoto:
                    //    return element.FindResource("photoMessage") as DataTemplate;
                    default:
                        return element.FindResource("unsupportedMessage") as DataTemplate;
                }
            }
            return element.FindResource("unsupportedMessage") as DataTemplate;
        }
    }
}
