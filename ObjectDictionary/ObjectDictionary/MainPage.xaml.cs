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

namespace ObjectDictionary
{
    public partial class MainPage : ContentPage
    {
        public Realm realm = Realm.GetInstance();
        public MainPage()
        {
            InitializeComponent();
            listView.IsPullToRefreshEnabled = true;
            listView.ItemsSource = realm.All<ImageData>();
        }

        private async void toolbarItemAdd_Clicked(object sender, EventArgs e)
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
                var imageData = new ImageData();
                imageData.path = photo.Path;
                imageData.dateTimeCreated = DateTime.Now;
                imageData.displayDateTime = imageData.dateTimeCreated.Hour + ":" + imageData.dateTimeCreated.Minute + ":" + imageData.dateTimeCreated.Second + " " + 
                    imageData.dateTimeCreated.Day + "/" + imageData.dateTimeCreated.Month + "/" + imageData.dateTimeCreated.Year;

                var clarifaiClient = new ClarifaiClient("b657f18794b44fa7b7c2c7a159fc6cfc");

                UserDialogs.Instance.ShowLoading("Loading...");

                var res = await clarifaiClient.PublicModels.GeneralModel.Predict(
                    new ClarifaiFileImage(File.ReadAllBytes(photo.Path)
                    )
                    ).ExecuteAsync();

                if (res != null)
                {
                    realmAdd(imageData);

                    res.Get().Data.ForEach(it =>
                    {
                        var concept = new Models.Concept
                        {
                            imageData = imageData,
                            value = it.Name
                        };

                        realmAdd(concept);
                    });
                }

                UserDialogs.Instance.HideLoading();
            }
        }

        private void realmAdd(RealmObject o)
        {
            realm.Write(() =>
            {
                realm.Add(o);
            });
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedItem = (ImageData)e.SelectedItem;

            this.Navigation.PushAsync(
                new DetailPage(
                    selectedItem
                ));
        }
    }

}
