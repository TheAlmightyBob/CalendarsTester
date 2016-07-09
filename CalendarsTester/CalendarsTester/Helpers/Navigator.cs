using System;
using System.Threading.Tasks;
using CalendarsTester.Core.Helpers;
using CalendarsTester.Core.Services;
using Xamarin.Forms;

namespace CalendarsTester.Helpers
{
    public class Navigator : INavigator
    {
        #region Fields

        private INavigation _navigation;
        private ViewProvider _viewProvider;
        private Page _sourcePage;

        #endregion

        #region Constructor

        public Navigator(Page sourcePage)
        {
            _navigation = sourcePage.Navigation;
            _viewProvider = DependencyService.Get<ViewProvider>();
            _sourcePage = sourcePage;
        }

        #endregion

        #region INavigator

        public async Task PushAsync<TViewModel>(TViewModel viewmodel)
            where TViewModel : ViewModelBase
        {
            var page = _viewProvider.GetView(viewmodel) as Page;

            if (page == null)
            {
                throw new ArgumentException("viewmodel does not correspond to a page that can be navigated to");
            }

            await _navigation.PushAsync(page);
        }

        public async Task PushAsync<TViewModel>(Action<TViewModel> customInit = null)
            where TViewModel : ViewModelBase, new()
        {
            var viewmodel = ViewModelProvider.GetViewModel<TViewModel>(customInit);

            // TODO: Throw new custom exception if unable to create VM?

            if (viewmodel != null)
            {
                await PushAsync(viewmodel);
            }
        }

        public async Task PopAsync()
        {
            await _navigation.PopAsync();
        }

        public async Task PushModalAndWaitAsync<TViewModel>(TViewModel viewmodel)
            where TViewModel : ViewModelBase
        {
            var page = _viewProvider.GetView(viewmodel) as Page;

            if (page == null)
            {
                throw new ArgumentException("viewmodel does not correspond to a page that can be navigated to");
            }

            await PushModalAndWaitAsync(page);
        }

        public async Task<TViewModel> PushModalAndWaitAsync<TViewModel>(Action<TViewModel> customInit = null)
            where TViewModel : ViewModelBase, new()
        {
            var viewmodel = ViewModelProvider.GetViewModel<TViewModel>(customInit);

            // TODO: Throw new custom exception if unable to create VM?

            if (viewmodel != null)
            {
                await PushModalAndWaitAsync(viewmodel);
            }

            return viewmodel;
        }

        public async Task PushModalAsync<TViewModel>(TViewModel viewmodel)
            where TViewModel : ViewModelBase
        {
            var page = _viewProvider.GetView(viewmodel) as Page;

            if (page == null)
            {
                throw new ArgumentException("viewmodel does not correspond to a page that can be navigated to");
            }

            await _navigation.PushModalAsync(page);
        }

        public async Task PushModalAsync<TViewModel>(Action<TViewModel> customInit = null)
            where TViewModel : ViewModelBase, new()
        {
            var viewmodel = ViewModelProvider.GetViewModel<TViewModel>(customInit);

            // TODO: Throw new custom exception if unable to create VM?

            if (viewmodel != null)
            {
                await PushModalAsync(viewmodel);
            }
        }

        public async Task PopModalAsync()
        {
            await _navigation.PopModalAsync();
        }

        #endregion

        #region Public Methods

        public async Task PushModalAndWaitAsync(Page page)
        {
            var tcs = new TaskCompletionSource<object>();

            // Always use NavigationPage... both in case we need navigation,
            // and because it just looks better (adds title/etc)
            //
            var navigationPage = new NavigationPage(page);

            // Relying on "Appearing" event is risky in iOS where a ViewController
            // can be dismissed while it's presenting another view controller, but:
            // 1) Xamarin.Forms doesn't currently expose that functionality
            // 2) Don't use this function if that's a concern?
            // 
            // Could also fire a "Done" event from the viewmodel, but that
            // then requires that the viewmodels actually fire it...
            // (one of the problems being that case where the root modal view
            //  pushes other views onto its navigation stack... and if one
            //  of *those* says that everything is done, then the viewmodel we're listening
            //  to is the wrong one..)
            // (also there's the case of simply using the back buttons on Android/Windows)
            //
            // The "Appearing" event is apparently unreliable on Android. It
            // is not guaranteed to fire with the way Xamarin.Forms navigation
            // works on Android.
            //
            // Some have reported success with ModalPopped... although I'm not sure
            // what the behavior here is if the modal view pushes another view onto
            // its navigation stack and then *that* view pops the modal... would
            // the parameter to ModalPopped be the root modal view or the child?
            // - oh, it seems it's just the NavigationPage. Makes sense. Means I need a reference to it though.
            //
            // ModalPopped seems to help on Android, but then it doesn't fire at all on UWP or WinPhone!
            //
            // So... we do both. Register both events, first one wins.
            //
            EventHandler appearingHandler = null;
            EventHandler<ModalPoppedEventArgs> modalPoppedHandler = null;
            Action handler = () =>
            {
                tcs.SetResult(null);
                _sourcePage.Appearing -= appearingHandler;
                App.Current.ModalPopped -= modalPoppedHandler;
            };
            appearingHandler = (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Got Appearing event");
                handler();
            };
            modalPoppedHandler = (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Got ModalPopped event");

                if (e.Modal == navigationPage)
                {
                    handler();
                }
            };

            _sourcePage.Appearing += appearingHandler;

            App.Current.ModalPopped += modalPoppedHandler;

            await _navigation.PushModalAsync(navigationPage);

            await tcs.Task;
        }

        #endregion
    }
}