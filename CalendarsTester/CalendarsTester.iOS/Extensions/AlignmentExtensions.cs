using UIKit;
using Xamarin.Forms;

namespace CalendarsTester.iOS.Extensions
{
    /// <summary>
    /// This is a JustDecompiled clone of the internal Xamarin.Forms extension.
    /// </summary>
    public static class AlignmentExtensions
    {
        public static UITextAlignment ToNativeTextAlignment(this TextAlignment alignment)
        {
            if (alignment == TextAlignment.Center)
            {
                return UITextAlignment.Center;
            }
            if (alignment == TextAlignment.End)
            {
                return UITextAlignment.Right;
            }
            return UITextAlignment.Left;
        }
    }
}