using System.Windows.Input;
using Xamarin.Forms;

namespace CalendarsTester.Behaviors
{
    public class ListViewTap
    {
        // Bindable attached property for command
        public static readonly BindableProperty CommandProperty =
            BindableProperty.CreateAttached("Command", typeof(ICommand), typeof(ListViewTap), null, propertyChanged: CommandChanged);

        public static ICommand GetCommand(BindableObject child)
        {
            return (ICommand)child.GetValue(CommandProperty);
        }

        public static void SetCommand(BindableObject child, ICommand value)
        {
            child.SetValue(CommandProperty, value);
        }

        private static void CommandChanged(BindableObject ob, object oldValue, object newValue)
        {
            CommandChanged(ob, oldValue as ICommand, newValue as ICommand);
        }

        private static void CommandChanged(BindableObject ob, ICommand oldValue, ICommand newValue)
        {
            var listView = ob as ListView;

            if (listView == null)
            {
                return;
            }

            listView.ItemTapped -= ListView_ItemTapped;

            if (newValue != null)
            {
                listView.ItemTapped += ListView_ItemTapped;
            }
        }

        private static void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = sender as ListView;

            if (listView == null)
            {
                return;
            }

            var command = GetCommand(listView);

            if (command != null)
            {
                command.Execute(e.Item);
            }

            listView.ClearValue(ListView.SelectedItemProperty);
        }
    }
}