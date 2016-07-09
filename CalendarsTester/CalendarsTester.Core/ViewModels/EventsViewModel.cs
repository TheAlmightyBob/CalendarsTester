using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CalendarsTester.Core.Enums;
using CalendarsTester.Core.Helpers;
using Xamarin.Forms;
using Plugin.Calendars.Abstractions;
using System.Collections.Generic;
using Plugin.Calendars;
using Acr.UserDialogs;

namespace CalendarsTester.Core.ViewModels
{
    public class EventsViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<Grouping<DateTime, CalendarEvent>> _eventsByDay;

        private Command _fetchEventsCommand;
        private bool _isBusy;

        #endregion

        #region Properties

        public string Title { get { return "Events"; } }

        public Calendar Calendar { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public ObservableCollection<Grouping<DateTime, CalendarEvent>> EventsByDay
        {
            get { return _eventsByDay; }
            set
            {
                if (_eventsByDay != value)
                {
                    _eventsByDay = value;
                    HasChanged();
                }
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    HasChanged();

                    if (_fetchEventsCommand != null)
                    {
                        _fetchEventsCommand.ChangeCanExecute();
                    }
                }
            }
        }

        public ICommand FetchEventsCommand { get { return _fetchEventsCommand ??
                    (_fetchEventsCommand = new Command(FetchEvents, () => !IsBusy)); } }
        public ICommand AddEventCommand { get { return new Command(AddEvent, () => Calendar.CanEditEvents); } }
        public ICommand EditEventCommand { get { return new Command<CalendarEvent>(EditEvent); } }
        public ICommand DeleteEventCommand { get { return new Command<CalendarEvent>(DeleteEvent, ev => Calendar.CanEditEvents); } }
        public ICommand AddReminderCommand { get { return new Command<CalendarEvent>(AddReminder, ev => Calendar.CanEditEvents); } }
        public ICommand GoToIDCommand => new Command(GoToID);

        #endregion

        #region Overrides

        public override void Initialize()
        {
            FetchEvents();
        }

        #endregion

        #region Private Methods

        private async void FetchEvents()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var events = await CrossCalendars.Current.GetEventsAsync(Calendar, Start, End);
                EventsByDay = GroupEventsByDay(events);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }

            IsBusy = false;
        }

        private async void AddEvent()
        {
            try
            {
                var eventVM = await Navigator.PushModalAndWaitAsync<EventEditorViewModel>();

                if (eventVM.Result != ModalResult.Canceled)
                {
                    await CrossCalendars.Current.AddOrUpdateEventAsync(Calendar, eventVM.Event);
                    FetchEvents();
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void EditEvent(CalendarEvent ev)
        {
            try
            {
                var eventVM = await Navigator.PushModalAndWaitAsync<EventEditorViewModel>(vm =>
                {
                    vm.Event = ev;
                    vm.CanEdit = Calendar.CanEditEvents;
                });

                if (eventVM.Result == ModalResult.Deleted)
                {
                    DeleteEvent(eventVM.Event);
                }
                else if (eventVM.Result != ModalResult.Canceled)
                {
                    await CrossCalendars.Current.AddOrUpdateEventAsync(Calendar, eventVM.Event);
                    FetchEvents();
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void DeleteEvent(CalendarEvent ev)
        {
            try
            {
                await CrossCalendars.Current.DeleteEventAsync(Calendar, ev);
                FetchEvents();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void AddReminder(CalendarEvent ev)
        {
            try
            {
                var reminderVM = await Navigator.PushModalAndWaitAsync<ReminderEditorViewModel>();
                
                if (reminderVM.Result != ModalResult.Canceled)
                {
                    if (!(await CrossCalendars.Current.AddEventReminderAsync(ev, reminderVM.Reminder)))
                    {
                        ReportMessage("Unable to add reminder", string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void GoToID()
        {
            try
            {
                var result = await UserDialogs.Instance.PromptAsync("Go to event ID:");

                if (result.Ok)
                {
                    var ev = await CrossCalendars.Current.GetEventByIdAsync(result.Text);

                    if (ev == null)
                    {
                        await UserDialogs.Instance.AlertAsync("Event not found");
                    }
                    else
                    {
                        EditEvent(ev);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private static ObservableCollection<Grouping<DateTime, CalendarEvent>> GroupEventsByDay(IEnumerable<CalendarEvent> events)
        {
            var eventsByDay = new ObservableCollection<Grouping<DateTime, CalendarEvent>>();

            foreach (var ev in events)
            {
                AddEventToGroup(ev, eventsByDay);
            }

            return eventsByDay;
        }

        #endregion

        #region Static Helpers

        private static void AddEventToGroup(CalendarEvent ev,
            ObservableCollection<Grouping<DateTime, CalendarEvent>> eventsByDay)
        {
            var end = ev.AllDay ? ev.End.Date : ev.End;

            for (var date = ev.Start.Date; date < end; date = date.AddDays(1))
            {
                var day = eventsByDay.FirstOrDefault(d => d.Key == date);

                if (day != null)
                {
                    day.Add(ev);
                }
                else
                {
                    day = new Grouping<DateTime, CalendarEvent>(date, new List<CalendarEvent> { ev });

                    // Insert in chronological order (rather than following each add with an OrderBy...)
                    //
                    int index = eventsByDay.IndexOf(eventsByDay.FirstOrDefault(e => e.Key > day.Key));
                    if (index < 0)
                    {
                        index = eventsByDay.Count;
                    }
                    eventsByDay.Insert(index, day);
                }
            }
        }

        #endregion
    }
}