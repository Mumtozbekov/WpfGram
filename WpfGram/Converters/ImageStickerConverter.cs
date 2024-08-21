using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

using Telegram.Td.Api;

using WpfGram.Helpers;

namespace WpfGram.Converters
{
    internal class ImageStickerConverter : MarkupExtension, IValueConverter
    {
        public  object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageSticker msg)
            {
                try
                {


                    if (!string.IsNullOrEmpty(msg.Sticker.StickerValue.Local.Path) && System.IO.File.Exists(msg.Sticker.StickerValue.Local.Path))
                    {
                        var task = Task.Run(async () =>
                        {
                            var path = msg.Sticker.StickerValue.Local.Path;
                            return path;
                        });
                        return new TaskCompletionNotifier<string>(task); ;
                     

                    }
                    else
                    {
                        var task = Task.Run(async () =>
                        {

                            var stickerPath = await TgClientHelper.DownloadFile(msg.Sticker.StickerValue.Id);
                            if (string.IsNullOrEmpty(stickerPath))
                                return string.Empty;

                            return stickerPath;
                        });
                        return new TaskCompletionNotifier<string>(task);
                    }
                }
                catch (Exception ex) { }

            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
