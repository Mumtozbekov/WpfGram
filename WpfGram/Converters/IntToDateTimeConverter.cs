using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfGram.Converters
{
    public class IntToDateTimeConverter : MarkupExtension, IValueConverter
    {
        public string Param { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Int32 n)
            {
                var time = DateTimeOffset.FromUnixTimeSeconds(n).LocalDateTime;
                switch (Param)
                {
                    case "Month":
                        return new DateTime(time.Year,time.Month,1);
                    default:
                        return time;
                }
            }
            return DateTime.MinValue;
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
