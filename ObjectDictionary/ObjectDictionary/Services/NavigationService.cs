using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ObjectDictionary.Services
{
    class NavigationService
    {
        private static INavigation Navigation => Application.Current.MainPage?.Navigation;

        public static Task Navigate(object viewModel)
        {
            if (Navigation == null) throw new NotSupportedException("Set navigatable main page before calling this.");

            var page = GetPage(viewModel);
            page.BindingContext = viewModel;
            return Navigation.PushAsync(page, animated: false);
        }
        private static Page GetPage(object viewModel)
        {
            var pageType = viewModel.GetType().Name.Replace("ViewModel", "Page");
            return (Page)Activator.CreateInstance(Type.GetType($"ObjectDictionary.{pageType}"));
        }
    }
}
