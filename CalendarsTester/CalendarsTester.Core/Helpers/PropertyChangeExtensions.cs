using System.ComponentModel;

namespace CalendarsTester.Core.Helpers
{
    /// <summary>
    /// A PropertyChange helper that doesn't require inheriting from a common base class
    /// (handy if adding INotifyPropertyChanged to something that already has a base class)
    /// </summary>
    public static class PropertyChangeExtensions
    {
        public static void Raise(this PropertyChangedEventHandler handler, object sender, string propertyName)
        {
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
