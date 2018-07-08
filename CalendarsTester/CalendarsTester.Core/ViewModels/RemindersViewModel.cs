using CalendarsTester.Core.Enums;
using CalendarsTester.Core.Helpers;
using Plugin.Calendars.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace CalendarsTester.Core.ViewModels
{
    public class RemindersViewModel : ModalViewModelBase
    {
        #region Fields

        private ObservableCollection<CalendarEventReminder> _reminders;

        #endregion

        #region Properties

        public string Title { get { return "Reminders"; } }

        public ObservableCollection<CalendarEventReminder> Reminders
        {
            get { return _reminders; }
            set
            {
                if (_reminders != value)
                {
                    _reminders = value;
                    HasChanged();
                }
            }
        }

        public bool CanEdit { get; set; } = true;

        public ICommand DoneCommand => new Command(Done, () => CanEdit);
        public ICommand AddReminderCommand { get { return new Command(AddReminder, () => CanEdit); } }
        public ICommand EditReminderCommand { get { return new Command<CalendarEventReminder>(EditReminder); } }
        public ICommand DeleteReminderCommand { get { return new Command<CalendarEventReminder>(DeleteReminder, _ => CanEdit); } }

        #endregion

        private async void AddReminder()
        {
            try
            {
                var reminderVM = await Navigator.PushModalAndWaitAsync<ReminderEditorViewModel>();

                if (reminderVM.Result != ModalResult.Canceled)
                {
                    Reminders.Add(reminderVM.Reminder);
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private async void EditReminder(CalendarEventReminder reminder)
        {
            try
            {
                var reminderVM = await Navigator.PushModalAndWaitAsync<ReminderEditorViewModel>(vm =>
                {
                    vm.Reminder = reminder;
                    vm.CanEdit = CanEdit;
                });

                if (reminderVM.Result != ModalResult.Canceled)
                {
                    var index = _reminders.IndexOf(reminder);
                    _reminders.RemoveAt(index);
                    _reminders.Insert(index, reminderVM.Reminder);
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void DeleteReminder(CalendarEventReminder reminder)
        {
            _reminders.Remove(reminder);
        }
    }
}
