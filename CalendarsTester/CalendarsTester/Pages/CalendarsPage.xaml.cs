using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarsTester.Core.Helpers;
using CalendarsTester.Core.ViewModels;
using Xamarin.Forms;

namespace CalendarsTester.Pages
{
    public partial class CalendarsPage : ContentPage
    {
        public CalendarsPage()
        {
            InitializeComponent();

            //BindingContext = ViewModelProvider.GetViewModel<CalendarsViewModel>(vm => vm.Navigator = new Navigator(this));
        }
    }
}
