namespace CalendarsTester.Core.Services
{
    /// <summary>
    /// Based on http://projectmarvin.tumblr.com/post/121101741333/implementing-copy-to-clipboard-across-mobile
    /// </summary>
    public interface IClipboardService
    {
        void CopyToClipboard(string text);
    }
}
