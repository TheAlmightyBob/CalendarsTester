using System;
using System.Collections.Generic;
using CalendarsTester.Core.Helpers;
using CalendarsTester.Extensions;
using Xamarin.Forms;

namespace CalendarsTester.Helpers
{
    public class ViewProvider
    {
        private Dictionary<Type, Type> _viewmodelsToViews = new Dictionary<Type, Type>();

        public void Register<TViewModel, TView>()
            where TViewModel : ViewModelBase
            where TView : VisualElement, new()
        {
            _viewmodelsToViews.Add(typeof(TViewModel), typeof(TView));
        }

        public VisualElement GetView<TViewModel>()
            where TViewModel : ViewModelBase
        {
            Type viewType = null;
            VisualElement view = null;

            if (_viewmodelsToViews.TryGetValue(typeof(TViewModel), out viewType))
            {
                view = Activator.CreateInstance(viewType) as VisualElement;
            }

            return view;
        }

        public VisualElement GetView<TViewModel>(TViewModel viewmodel)
            where TViewModel : ViewModelBase
        {
            var view = GetView<TViewModel>();

            if (view != null)
            {
                view.BindingContext = viewmodel;
                viewmodel.Navigator = new Navigator(view.GetPage());
            }

            return view;
        }
    }
}