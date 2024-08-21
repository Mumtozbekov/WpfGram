using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfGram.Converters
{
    public class GetFirstCharacterConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null && ((string)value).Split(",").Length < 2)
                {
                    var a = ((string)value).Trim().Split();
                    if (a.Length > 1)
                        return string.Join("", a.Select(x => x[0]).Take(2));
                    else
                        return ((string)value)[0];
                }
                else
                    return null;
            }
            catch (Exception ex) { }
            return null;
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
