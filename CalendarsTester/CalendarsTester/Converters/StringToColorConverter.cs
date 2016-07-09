using System;
using System.Globalization;
using Xamarin.Forms;

namespace CalendarsTester.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        private ColorTypeConverter _converter = new ColorTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;

            if (string.IsNullOrEmpty(str))
            {
                //return null;
                return Color.Transparent;
            }

            var color = Color.Transparent;

            try
            {
                color = (Color)_converter.ConvertFromInvariantString(str);
            }
            catch
            {
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
