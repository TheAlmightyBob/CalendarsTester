using System;
using CalendarsTester.Core.Services;
using Xamarin.Forms;

namespace CalendarsTester.Services
{
    public class ReportingService : IReportingService
    {
        public void ReportException(Exception ex)
        {
            ReportMessage("Error", ex.Message);
        }

        public void ReportMessage(string message, string details)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // TODO: Make the Report methods async?

                //var dlg = new MessageDialog(details, message);
                //await dlg.ShowAsync();

                await Application.Current.MainPage.DisplayAlert(message, details, "OK");
            });
        }
    }
}