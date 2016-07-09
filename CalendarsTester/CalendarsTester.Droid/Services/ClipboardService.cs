using Android.Content;
using CalendarsTester.Core.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(CalendarsTester.Droid.Services.ClipboardService))]

namespace CalendarsTester.Droid.Services
{
    /// <summary>
    /// Taken from http://forums.xamarin.com/discussion/41374/
    /// </summary>
    public class ClipboardService : IClipboardService
    {
        public void CopyToClipboard(string text)
        {
            var clipboardManager = (ClipboardManager)Forms.Context.GetSystemService(Context.ClipboardService);

            var clipData = ClipData.NewPlainText("text", text); // text

            clipboardManager.PrimaryClip = clipData;
        }
    }
}