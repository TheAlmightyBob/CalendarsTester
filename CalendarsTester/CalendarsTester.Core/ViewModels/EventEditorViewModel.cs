using System;
using System.Windows.Input;
using CalendarsTester.Core.Enums;
using CalendarsTester.Core.Helpers;
using Xamarin.Forms;
using Plugin.Calendars.Abstractions;
using CalendarsTester.Core.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace CalendarsTester.Core.ViewModels
{
    public class EventEditorViewModel : ModalViewModelBase
    {
        #region Fields

        private string _name;
        private string _description;
        private string _location;
        private DateTime _startDate;
        private DateTime _endDate;
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private bool _allDay;
        private IList<CalendarEventReminder> _reminders;

        private CalendarEvent _event;

        private Command _doneCommand;

        #endregion

        #region Properties

        public string Title { get { return "Edit Event"; } }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    HasChanged();
                    _doneCommand.ChangeCanExecute();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    HasChanged();
                }
            }
        }

        public string Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    HasChanged();
                }
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    HasChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    HasChanged();
                }
            }
        }

        public TimeSpan StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    HasChanged();
                }
            }
        }

        public TimeSpan EndTime
        {
            get { return _endTime; }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    HasChanged();
                }
            }
        }

        public bool AllDay
        {
            get { return _allDay; }
            set
            {
                if (_allDay != value)
                {
                    _allDay = value;
                    HasChanged();
                }
            }
        }

        public CalendarEvent Event
        {
            get { return _event; }
            set
            {
                if (_event != value)
                {
                    _event = value;

                    Name = _event.Name;
                    Description = _event.Description;
                    Location = _event.Location;
                    StartDate = _event.Start.Date;
                    EndDate = _event.End.Date;
                    StartTime = _event.Start.TimeOfDay;
                    EndTime = _event.End.TimeOfDay;
                    AllDay = _event.AllDay;

                    _reminders = _event.Reminders;
                }
            }
        }

        public bool CanEdit { get; set; } = true;
        public bool IsCreating => _event == null;
        public bool CanDelete => CanEdit && !IsCreating;

        public ICommand DoneCommand { get { return _doneCommand; } }
        public ICommand CopyIDCommand => new Command(CopyID, () => !string.IsNullOrEmpty(_event?.ExternalID));
        public ICommand EditRemindersCommand => new Command(EditReminders);

        #endregion

        #region Constructor

        public EventEditorViewModel()
        {
            _doneCommand = new Command(Done, () => !string.IsNullOrWhiteSpace(_name) && CanEdit);
            _startDate = DateTime.Today;
            _endDate = _startDate.AddDays(1);
        }

        #endregion

        #region Private Methods

        protected override void Done()
        {
            var start = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day,
                _startTime.Hours, _startTime.Minutes, _startTime.Seconds, DateTimeKind.Local);
            var end = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day,
                _endTime.Hours, _endTime.Minutes, _endTime.Seconds, DateTimeKind.Local);

            // Not validating input because the purpose of this project is to test the API...
            // which includes testing invalid input.
            //
            //if (end <= start)
            //{
            //    ReportMessage("Start must precede End", "Time travel is not supported");
            //    return;
            //}

            Result = ModalResult.Done;

            if (_event == null)
            {
                _event = new CalendarEvent { AllDay = true };
            }

            _event.Name = Name;
            _event.Description = Description;
            _event.Location = Location;
            _event.Start = start;
            _event.End = end;
            _event.AllDay = AllDay;

            _event.Reminders = _reminders;

            Navigator.PopModalAsync();
        }

        private void CopyID()
        {
            var clipboard = DependencyService.Get<IClipboardService>();
            if (clipboard != null && !string.IsNullOrEmpty(_event?.ExternalID))
            {
                clipboard.CopyToClipboard(_event.ExternalID);
            }
        }

        private async void EditReminders()
        {
            try
            {
                var remindersVM = await Navigator.PushModalAndWaitAsync<RemindersViewModel>(vm =>
                {
                    vm.Reminders = new ObservableCollection<CalendarEventReminder>(_reminders ?? Enumerable.Empty<CalendarEventReminder>());
                    vm.CanEdit = CanEdit;
                });

                if (remindersVM.Result != ModalResult.Canceled)
                {
                    _reminders = remindersVM.Reminders;
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        #endregion
    }
}