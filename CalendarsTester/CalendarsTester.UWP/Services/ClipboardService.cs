using CalendarsTester.Core.Services;
using Windows.ApplicationModel.DataTransfer;
using Xamarin.Forms;

[assembly: Dependency(typeof(CalendarsTester.UWP.Services.ClipboardService))]

namespace CalendarsTester.UWP.Services
{
    public class ClipboardService : IClipboardService
    {
        public void CopyToClipboard(string text)
        {
            var data = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            data.SetText(text);
            Clipboard.SetContent(data);
        }
    }
}
