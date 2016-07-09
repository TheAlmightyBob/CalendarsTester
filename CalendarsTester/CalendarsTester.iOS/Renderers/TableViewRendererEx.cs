using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CalendarsTester.iOS.Renderers;
using CalendarsTester.Controls;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(TableViewEx), typeof(TableViewRendererEx))]

namespace CalendarsTester.iOS.Renderers
{
    public class TableViewRendererEx : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.AllowsSelection = (Element as TableViewEx)?.AllowSelection == true;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var tableView = Element as TableViewEx;

            if (tableView == null)
            {
                return;
            }

            if (e.PropertyName == nameof(tableView.AllowSelection))
            {
                Control.AllowsSelection = tableView.AllowSelection;
            }
        }
    }
}