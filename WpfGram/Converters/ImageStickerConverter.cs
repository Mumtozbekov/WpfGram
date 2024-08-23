using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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
                            var img = LoadWebP(msg.Sticker.StickerValue.Local.Path);
                            return img;
                        });
                        return new TaskCompletionNotifier<BitmapSource>(task);
                     

                    }
                    else
                    {
                        var task = Task.Run(async () =>
                        {

                            var stickerPath = await TgClientHelper.DownloadFile(msg.Sticker.StickerValue.Id);
                            if (string.IsNullOrEmpty(stickerPath))
                                return null;

                            return LoadWebP(stickerPath); 
                        });
                        return new TaskCompletionNotifier<BitmapSource>(task);
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

        BitmapSource LoadWebP(string path)
        {
            var decoder = BitmapDecoder.Create(new Uri(path, UriKind.Relative), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            var converted = new FormatConvertedBitmap(decoder.Frames[0], PixelFormats.Bgra32, null, 0);
            converted.Freeze();
            return converted;
        }
    }
}
