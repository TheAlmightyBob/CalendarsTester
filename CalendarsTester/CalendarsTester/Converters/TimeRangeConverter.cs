using Plugin.Calendars.Abstractions;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CalendarsTester.Converters
{
    public class TimeRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ev = value as CalendarEvent;

            if (ev == null)
            {
                return null;
            }

            if (ev.AllDay)
            {
                // For an AllDay event, the End is typically returned as being the following day
                //
                if (ev.Start.Date == ev.End.Date.AddDays(-1))
                {
                    return ev.Start.ToString("m");
                }
                else
                {
                    return $"{ev.Start.ToString("m")} - {ev.End.AddDays(-1).ToString("m")}";
                }
            }
            else
            {
                if (ev.Start.Date == ev.End.Date)
                {
                    return $"{ev.Start.ToString("t")} - {ev.End.ToString("t")}";
                }
                else
                {
                    return $"{ev.Start.ToString("MMMM d, h:mm tt")} - {ev.End.ToString("MMMM d, h:mm tt")}";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}