using System;
using System.Linq;
using System.Threading.Tasks;
using CalendarsTester.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CalendarsTester.Controls;
using CalendarsTester.Enums;
using UIKit;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationRendererEx))]

namespace CalendarsTester.iOS.Renderers
{
    /// <summary>
    /// Adds support for ToolbarItemEx, with special handling/placement for Cancel/Done buttons
    /// </summary>
    public class NavigationRendererEx : NavigationRenderer
    {
        protected override Task<bool> OnPushAsync(Page page, bool animated)
        {
            // We don't await this because doing so would mean the buttons don't
            // update until after the view finishes animating into position
            //
            var ret = base.OnPushAsync(page, animated);

            var toolbarItems = page.ToolbarItems.OfType<ToolbarItemEx>();

            // If ToolbarItemEx is in use, then we take over the top toolbar
            //
            if (toolbarItems.Any())
            {
                var navItem = TopViewController.NavigationItem;

                // If you ask for more than one cancel button.. well, you're weird.. so I'll just ignore the others. You're welcome.
                //
                var cancelButton = toolbarItems.FirstOrDefault(tbi => tbi.ToolbarItemType == ToolbarItemType.Cancel);

                if (cancelButton != null)
                {
                    navItem.SetLeftBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Cancel,
                        (s, e) => cancelButton.Activate()), false);
                }

                // TODO: Respect Priority
                //
                var rightItems = toolbarItems.Where(tbi => tbi.ToolbarItemType != ToolbarItemType.Cancel && tbi.Order != ToolbarItemOrder.Secondary)
                    .Select(tbi => CreateUIBarButtonItem(tbi)).ToArray();

                navItem.SetRightBarButtonItems(rightItems, false);
            }

            return ret;
        }

        /// <summary>
        /// Creates system-style buttons if requested and hooks up CanExecuteChanged for the ToolbarItem command.
        /// (after writing this I noticed that the latter will be in the next version of Forms... oh well)
        /// </summary>
        /// <param name="toolbarItem"></param>
        /// <returns></returns>
        private UIBarButtonItem CreateUIBarButtonItem(ToolbarItemEx toolbarItem)
        {
            UIBarButtonSystemItem? systemItem = null;

            switch (toolbarItem.ToolbarItemType)
            {
                case ToolbarItemType.Done:
                    systemItem = UIBarButtonSystemItem.Done;
                    break;
                case ToolbarItemType.Add:
                    systemItem = UIBarButtonSystemItem.Add;
                    break;
                case ToolbarItemType.Refresh:
                    systemItem = UIBarButtonSystemItem.Refresh;
                    break;
                case ToolbarItemType.Search:
                    systemItem = UIBarButtonSystemItem.Search;
                    break;
            }

            var buttonItem = systemItem != null ? new UIBarButtonItem(systemItem.Value, (s, e) => toolbarItem.Activate()) : toolbarItem.ToUIBarButtonItem();

            // Xamarin.Forms 1.3.2 adds support for CanExecute, so we only need this for
            // our custom system buttons
            //
            if (systemItem != null)
            {
                var command = toolbarItem.Command;
                EventHandler canExecuteHandler = (s, e) => UpdateButtonState(toolbarItem, buttonItem);

                if (command != null)
                {
                    command.CanExecuteChanged += canExecuteHandler;
                }

                toolbarItem.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == ToolbarItem.CommandProperty.PropertyName)
                    {
                        if (command != null)
                        {
                            command.CanExecuteChanged -= canExecuteHandler;
                        }

                        UpdateButtonState(toolbarItem, buttonItem);

                        if (toolbarItem.Command != null)
                        {
                            toolbarItem.Command.CanExecuteChanged += canExecuteHandler;
                        }

                        command = toolbarItem.Command;
                    }
                };

                UpdateButtonState(toolbarItem, buttonItem);
            }

            return buttonItem;
        }

        private static void UpdateButtonState(ToolbarItemEx toolbarItem, UIBarButtonItem buttonItem)
        {
            buttonItem.Enabled = toolbarItem.Command?.CanExecute(toolbarItem.CommandParameter) ?? false;
        }
    }
}