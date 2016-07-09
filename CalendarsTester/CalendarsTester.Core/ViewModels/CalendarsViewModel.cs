using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using CalendarsTester.Core.Helpers;
using Plugin.Calendars.Abstractions;
using Plugin.Calendars;
using CalendarsTester.Core.Enums;
using CalendarsTester.Core.Extensions;
using Acr.UserDialogs;

namespace CalendarsTester.Core.ViewModels
{
    public class CalendarsViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<Grouping<string, Calendar>> _groupedCalendars;
        private bool _isBusy;
        private Command _fetchCalendarsCommand;

        #endregion

        #region Properties

        public string Title { get { return "Select Calendar"; } }

        public ObservableCollection<Grouping<string, Calendar>> GroupedCalendars
        {
            get { return _groupedCalendars; }
            set
            {
                if (_groupedCalendars != value)
                {
                    _groupedCalendars = value;
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

                    if (_fetchCalendarsCommand != null)
                    {
                        _fetchCalendarsCommand.ChangeCanExecute();
                    }
                }
            }
        }

        public ICommand FetchCalendarsCommand { get { return _fetchCalendarsCommand ??
                    (_fetchCalendarsCommand = new Command(FetchCalendars, () => !IsBusy)); } }
        public ICommand AddCalendarCommand { get { return new Command(AddCalendar); } }
        public ICommand EditCalendarCommand { get { return new Command<Calendar>(EditCalendar); } }
        public ICommand DeleteCalendarCommand { get { return new Command<Calendar>(DeleteCalendar,
            calendar => calendar?.CanEditCalendar ?? false); } }
        public ICommand GoToIDCommand => new Command(GoToID);
        public ICommand SelectCalendarCommand { get { return new Command<Calendar>(SelectCalendar); } }

        #endregion

        public override void Initialize()
        {
            FetchCalendars();
        }

        private async void FetchCalendars()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var calendars = await CrossCalendars.Current.GetCalendarsAsync();

                GroupedCalendars = new ObservableCollection<Grouping<string, Calendar>>(
                    calendars.GroupBy(c => GetAccessLevelDescription(c))
                    .OrderBy(g => g.Key)
                    .Select(g => new Grouping<string, Calendar>(g.Key, g)));
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }

            IsBusy = false;
        }

        private async void AddCalendar()
        {
            try
            {
                var newCalVM = await Navigator.PushModalAndWaitAsync<CalendarEditorViewModel>();

                if (newCalVM.Result != ModalResult.Canceled)
                {
                    await CrossCalendars.Current.AddOrUpdateCalendarAsync(newCalVM.Calendar);

                    // Refresh
                    FetchCalendars();
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void EditCalendar(Calendar calendar)
        {
            try
            {
                var newCalVM = await Navigator.PushModalAndWaitAsync<CalendarEditorViewModel>(vm => vm.Calendar = calendar);

                if (newCalVM.Result != ModalResult.Canceled)
                {
                    await CrossCalendars.Current.AddOrUpdateCalendarAsync(newCalVM.Calendar);

                    // Refresh
                    FetchCalendars();
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void DeleteCalendar(Calendar calendar)
        {
            try
            {
                await CrossCalendars.Current.DeleteCalendarAsync(calendar);

                // Refresh
                FetchCalendars();
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
                var result = await UserDialogs.Instance.PromptAsync("Go to calendar ID:");

                if (result.Ok)
                {
                    var calendar = await CrossCalendars.Current.GetCalendarByIdAsync(result.Text);

                    if (calendar == null)
                    {
                        await UserDialogs.Instance.AlertAsync("Calendar not found");
                    }
                    else
                    {
                        EditCalendar(calendar);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void SelectCalendar(Calendar calendar)
        {
            try
            {
                await Navigator.PushAsync<DateTimeRangeViewModel>(vm => vm.Calendar = calendar);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private string GetAccessLevelDescription(Calendar calendar)
        {
            if (calendar.CanEditCalendar && calendar.CanEditEvents)
            {
                return "Can Edit Calendar & Events";
            }
            else if (calendar.CanEditCalendar)
            {
                return "Can Edit Calendar";
            }
            else if (calendar.CanEditEvents)
            {
                return "Can Edit Events";
            }
            else
            {
                return "Read-only";
            }
        }
    }
}