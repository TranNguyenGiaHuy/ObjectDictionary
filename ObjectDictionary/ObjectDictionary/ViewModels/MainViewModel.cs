using Acr.UserDialogs;
using ObjectDictionary.Models;
using ObjectDictionary.Services;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ObjectDictionary.ViewModels
{
    class MainViewModel
    {
        private readonly Realm realm;
        public IEnumerable<ImageData> ImageDatas { get; }
        public Command AddImageCommand { get; }
        public Command<ImageData> ShowConceptsCommand { get; }

        public MainViewModel()
        {
            realm = Realm.GetInstance();
            ImageDatas = realm.All<ImageData>().OrderByDescending(i => i.Id);
            AddImageCommand = new Command(async () => await AddImageAsync());
            ShowConceptsCommand = new Command<ImageData>(ShowConcept);
        }

        public void AddImageData(ImageData imageData)
        {
            realm.Write(() => realm.Add(imageData));
        }

        private async Task AddImageAsync()
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
                    UserDialogs.Instance.Alert("Permissions Denied", "Unable to take photos.", "OK");
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
                AddImageData(imageData);

                await NetworkService.RecognizeImage(photo.Path, imageData);
            }
        }

        private void ShowConcept(ImageData imageData)
        {
            if (imageData == null || !imageData.IsUpdated) return;
            var detailViewModel = new DetailViewModel(imageData);
            NavigationService.Navigate(detailViewModel);
        }
    }
}
