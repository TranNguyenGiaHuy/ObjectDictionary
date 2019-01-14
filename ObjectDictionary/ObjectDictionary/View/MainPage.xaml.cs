using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Clarifai.DTOs.Predictions;
using System.IO;
using Acr.UserDialogs;
using Realms;
using ObjectDictionary.Models;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using ObjectDictionary.Services;
using ObjectDictionary.ViewModels;

namespace ObjectDictionary
{
    public partial class MainPage : ContentPage
    {
        public Realm realm = Realm.GetInstance();
        public MainPage()
        {
            InitializeComponent();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedItem = (ImageData)e.SelectedItem;
            ((ListView)sender).SelectedItem = null;

            ((MainViewModel)BindingContext).ShowConceptsCommand.Execute(selectedItem);
        }
    }

}
