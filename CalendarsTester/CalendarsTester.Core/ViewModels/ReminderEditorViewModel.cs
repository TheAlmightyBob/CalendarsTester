using System;
using Plugin.Calendars.Abstractions;
using CalendarsTester.Core.Helpers;
using CalendarsTester.Core.Enums;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.Generic;

namespace CalendarsTester.Core.ViewModels
{
    public class ReminderEditorViewModel : ModalViewModelBase
    {
        #region Fields

        private string _value = "15";
        private TimeUnits _units = TimeUnits.Minutes;
        private CalendarReminderMethod _method;

        private CalendarEventReminder _reminder;

        #endregion

        #region Properties

        public string Title => "New Reminder";

        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    HasChanged();
                }
            }
        }

        public TimeUnits Units
        {
            get { return _units; }
            set
            {
                if (_units != value && Enum.IsDefined(typeof(TimeUnits), value))
                {
                    _units = value;
                    HasChanged();
                }
            }
        }

        public IList<string> UnitOptions { get; } = Enum.GetNames(typeof(TimeUnits));

        public CalendarReminderMethod Method
        {
            get { return _method; }
            set
            {
                if (_method != value && Enum.IsDefined(typeof(CalendarReminderMethod), value))
                {
                    _method = value;
                    HasChanged();
                }
            }
        }

        public IList<string> MethodOptions { get; } = Enum.GetNames(typeof(CalendarReminderMethod));

        public CalendarEventReminder Reminder
        {
            get { return _reminder; }
            set
            {
                if (_reminder != value)
                {
                    _reminder = value;

                    if (_reminder == null)
                        return;

                    if (_reminder.TimeBefore.Minutes > 0)
                    {
                        Value = _reminder.TimeBefore.TotalMinutes.ToString();
                        Units = TimeUnits.Minutes;
                    }
                    else if (_reminder.TimeBefore.Hours > 0)
                    {
                        Value = _reminder.TimeBefore.TotalHours.ToString();
                        Units = TimeUnits.Hours;
                    }
                    else if (_reminder.TimeBefore.Days > 0)
                    {
                        Value = _reminder.TimeBefore.TotalDays.ToString();
                        Units = TimeUnits.Days;
                    }

                    Method = _reminder.Method;
                }
            }
        }

        public bool CanEdit { get; set; } = true;

        public ICommand DoneCommand => new Command(Done, () => CanEdit);

        #endregion

        protected override void Done()
        {
            _reminder = new CalendarEventReminder
            {
                TimeBefore = GetTimeSpan(_value, _units),
                Method = Method
            };
            
            base.Done();
        }

        private static TimeSpan GetTimeSpan(string strValue, TimeUnits units)
        {
            if (!double.TryParse(strValue, out double value))
            {
                return TimeSpan.Zero;
            }

            switch (units)
            {
                case TimeUnits.Minutes:
                    return TimeSpan.FromMinutes(value);
                case TimeUnits.Hours:
                    return TimeSpan.FromHours(value);
                case TimeUnits.Days:
                    return TimeSpan.FromDays(value);
                default:
                    throw new ArgumentException("Unsupported TimeUnits value", nameof(units));
            }
        }
    }
}
