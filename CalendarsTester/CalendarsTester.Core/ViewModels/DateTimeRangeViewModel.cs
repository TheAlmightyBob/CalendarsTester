using System;
using CalendarsTester.Core.Helpers;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.Calendars.Abstractions;

namespace CalendarsTester.Core.ViewModels
{
    public class DateTimeRangeViewModel : ViewModelBase
    {
        #region Fields

        private DateTime _start;
        private DateTime _end;

        #endregion

        #region Properties

        public string Title { get { return "Select Time Range"; } }

        public Calendar Calendar { get; set; }

        public DateTime StartDate
        {
            get { return _start.Date; }
            set
            {
                if (_start.Date != value.Date)
                {
                    _start = new DateTime(value.Year, value.Month, value.Day,
                        _start.Hour, _start.Minute, _start.Second, _start.Kind);
                    HasChanged();

                    if (_end <= _start)
                    {
                        EndDate = _start.AddDays(1);
                    }
                }
            }
        }

        public TimeSpan StartTime
        {
            get { return _start.TimeOfDay; }
            set
            {
                if (_start.TimeOfDay != value)
                {
                    _start = new DateTime(_start.Year, _start.Month, _start.Day,
                        value.Hours, value.Minutes, value.Seconds, _start.Kind);
                    HasChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get { return _end.Date; }
            set
            {
                if (_end != value)
                {
                    _end = new DateTime(value.Year, value.Month, value.Day,
                        _end.Hour, _end.Minute, _end.Second, _end.Kind);
                    HasChanged();
                }
            }
        }

        public TimeSpan EndTime
        {
            get { return _end.TimeOfDay; }
            set
            {
                if (_end.TimeOfDay != value)
                {
                    _end = new DateTime(_end.Year, _end.Month, _end.Day,
                        value.Hours, value.Minutes, value.Seconds, _end.Kind);
                    HasChanged();
                }
            }
        }

        public ICommand GoToEventsCommand
        {
            get { return new Command(GoToEvents); }
        }

        #endregion

        public override void Initialize()
        {
            var today = DateTime.Today;
            StartDate = new DateTime(today.Year, today.Month, 1);
            EndDate = StartDate.AddMonths(1);
        }

        private async void GoToEvents()
        {
            try
            {
                await Navigator.PushAsync<EventsViewModel>(vm =>
                {
                    vm.Calendar = Calendar;
                    vm.Start = _start;
                    vm.End = _end;
                });
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }
    }
}