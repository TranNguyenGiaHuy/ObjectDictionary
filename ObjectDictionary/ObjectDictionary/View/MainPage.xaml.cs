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

namespace ObjectDictionary
{
    public partial class MainPage : ContentPage
    {
        public Realm realm = Realm.GetInstance();
        public MainPage()
        {
            InitializeComponent();
            listView.IsPullToRefreshEnabled = false;
            listView.ItemsSource = realm.All<ImageData>();
        }

        private async void ToolbarItemAdd_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                UserDialogs.Instance.Alert("No Camera", ":( No camera available.", "OK");
                return;
            }
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (cameraStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                cameraStatus = results[Permission.Camera];
                if (cameraStatus != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                    return;
                }
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions()
            {
                //savetoalbum = true,
                //directory = "my_images",
                //name = datetime.now + ".jpg"
            });
            if (photo != null)
            {
                var imageData = new ImageData
                {
                    path = photo.Path,
                    dateTimeCreated = DateTime.Now
                };
                imageData.displayDateTime = imageData.dateTimeCreated.Hour + ":" + imageData.dateTimeCreated.Minute + ":" + imageData.dateTimeCreated.Second + " " + 
                    imageData.dateTimeCreated.Day + "/" + imageData.dateTimeCreated.Month + "/" + imageData.dateTimeCreated.Year;
                RealmService.GetInstance().RealmAdd(imageData);

                NetworkService.RecognizeImage(photo.Path, imageData);
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedItem = (ImageData)e.SelectedItem;

            this.Navigation.PushAsync(
                new DetailPage(
                    selectedItem
                ));
        }
    }

}
