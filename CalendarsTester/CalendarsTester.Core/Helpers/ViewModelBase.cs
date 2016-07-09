using System;
using CalendarsTester.Core.Services;

namespace CalendarsTester.Core.Helpers
{
    public class ViewModelBase : PropertyChangeNotifier
    {
        #region Dependencies

        public IReportingService ReportingService { get; set; }
        public INavigator Navigator { get; set; }

        #endregion

        public virtual void Initialize() { }


        protected void ReportError(Exception ex)
        {
            if (ReportingService != null)
            {
                ReportingService.ReportException(ex);
            }
        }

        protected void ReportMessage(string message, string details)
        {
            if (ReportingService != null)
            {
                ReportingService.ReportMessage(message, details);
            }
        }
    }
}