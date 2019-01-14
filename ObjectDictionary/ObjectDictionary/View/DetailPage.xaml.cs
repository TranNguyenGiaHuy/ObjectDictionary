using Acr.UserDialogs;
using Newtonsoft.Json;
using ObjectDictionary.Models;
using ObjectDictionary.Services;
using ObjectDictionary.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ObjectDictionary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ContentPage
    {
        public DetailPage()
        {
            InitializeComponent();
        }

        public DetailPage(ImageData imageData)
        {
            InitializeComponent();
            //listView.ItemsSource = Realms.Realm.GetInstance().All<Concept>().Where(it => it.imageData == imageData);
        }

        private async Task listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var concept = (Concept)e.SelectedItem;
            ((ListView)sender).SelectedItem = null;
            var value = concept.value;

            var getDictionaryResult = Realms.Realm.GetInstance().All<Dictionary>().Count(it => it.originalWord == value);

            if (getDictionaryResult == 0)
            {
                UserDialogs.Instance.ShowLoading("Getting Dictionary");
                await NetworkService.AddDictonaryOfConcept(concept);
                UserDialogs.Instance.HideLoading();
            }

            var dictionaryViewModel = new DictionaryViewModel(concept.value);
            await NavigationService.Navigate(dictionaryViewModel);

        }
    }
}