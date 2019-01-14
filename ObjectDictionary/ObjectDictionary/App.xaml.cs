using ObjectDictionary.Converter;
using ObjectDictionary.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ObjectDictionary
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(
                new MainPage
                {
                    BindingContext = new MainViewModel()
                })
            {
                BarBackgroundColor = Color.BlueViolet,
                BarTextColor = Color.White
            };

            Resources = new ResourceDictionary
            {
                ["BooleanToCheckConverter"] = new BooleanToCheckConverter()
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
