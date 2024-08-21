using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfGram.Converters
{
    public class IdToColorConverter : MarkupExtension, IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {
                long id = 0;
                long.TryParse(value.ToString(), out id);
                var converter = new BrushConverter();
                var brush = (Brush)converter.ConvertFromString(Constants.AvatarColors.ElementAt((int)(Math.Abs(id)%7)).Value);
                return brush;
            }
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
