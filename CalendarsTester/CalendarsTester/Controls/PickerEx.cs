using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CalendarsTester.Controls
{
    public class PickerEx : Picker
    {
        // Bindable property for the items source
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(PickerEx), null, propertyChanged: ItemsSourceChanged);

        //Gets or sets the items source
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = bindable as PickerEx;
            var items = newValue as IList;

            if (picker == null)
            {
                return;
            }

            picker.Items.Clear();
            
            if (items != null)
            {
                foreach (var item in items)
                {
                    picker.Items.Add(item.ToString());
                }
            }
        }
    }
}
