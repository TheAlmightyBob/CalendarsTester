using CalendarsTester.Core.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(CalendarsTester.iOS.Services.ClipboardService))]

namespace CalendarsTester.iOS.Services
{
    public class ClipboardService : IClipboardService
    {
        public void CopyToClipboard(string text)
        {
            var clipboard = UIPasteboard.General;
            clipboard.String = text;
        }
    }
}