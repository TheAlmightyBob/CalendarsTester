using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CalendarsTester.Core.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHex(this Color color)
        {
            var hex = $"#{ColorAsInt(color.R):X2}{ColorAsInt(color.G):X2}{ColorAsInt(color.B):X2}";
            return hex;
        }

        private static int ColorAsInt(double color)
        {
            return (int)(color * 255);
        }
    }
}
