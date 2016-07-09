using System;
using System.Threading.Tasks;
using CalendarsTester.Core.Helpers;

namespace CalendarsTester.Core.Services
{
    public interface INavigator
    {
        Task PushAsync<TViewModel>(TViewModel viewmodel)
            where TViewModel : ViewModelBase;
        Task PushAsync<TViewModel>(Action<TViewModel> customInit = null)
            where TViewModel : ViewModelBase, new();

        Task PopAsync();

        Task PushModalAndWaitAsync<TViewModel>(TViewModel viewmodel)
            where TViewModel : ViewModelBase;
        Task<TViewModel> PushModalAndWaitAsync<TViewModel>(Action<TViewModel> customInit = null)
            where TViewModel : ViewModelBase, new();

        Task PushModalAsync<TViewModel>(TViewModel viewmodel)
            where TViewModel : ViewModelBase;
        Task PushModalAsync<TViewModel>(Action<TViewModel> customInit = null)
            where TViewModel : ViewModelBase, new();

        Task PopModalAsync();
    }
}