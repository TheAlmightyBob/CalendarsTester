using Humanizer;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CalendarsTester.Converters
{
    public class TimeBeforeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                return $"{ts.Humanize(5)} before";
            }

            return $"Invalid input: {value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
