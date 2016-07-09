using CalendarsTester.Core.Services;
using Windows.UI.Popups;
using Xamarin.Forms;

[assembly: Dependency(typeof(CalendarsTester.WinPhone.Services.ClipboardService))]

namespace CalendarsTester.WinPhone.Services
{
    public class ClipboardService : IClipboardService
    {
        public void CopyToClipboard(string text)
        {
            var msg = new MessageDialog("Sorry, Windows Phone 8.1 doesn't have a clipboard API. You'll just have to write it down. :(");
            msg.ShowAsync();
        }
    }
}
