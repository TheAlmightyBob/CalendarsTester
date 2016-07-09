using CalendarsTester.Enums;
using Xamarin.Forms;

namespace CalendarsTester.Controls
{
    /// <summary>
    /// Adds support for special toolbar item types (mainly for iOS, but could be used by others...)
    /// The renderers *could* just look at item names, but I prefer a more explicit opt-in model.
    /// </summary>
    public class ToolbarItemEx : ToolbarItem
    {
        // Bindable property for the toolbar item type
        public static readonly BindableProperty ToolbarItemTypeProperty =
            BindableProperty.Create(nameof(ToolbarItemType), typeof(ToolbarItemType), typeof(ToolbarItemEx), ToolbarItemType.Standard);

        //Gets or sets the toolbar item type
        public ToolbarItemType ToolbarItemType
        {
            get { return (ToolbarItemType)GetValue(ToolbarItemTypeProperty); }
            set { SetValue(ToolbarItemTypeProperty, value); }
        }


        /// <summary>
        /// Public copy of the base class' internal method..
        /// </summary>
        public void Activate()
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
            OnClicked();
        }
    }
}