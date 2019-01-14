using Acr.UserDialogs;
using ObjectDictionary.Models;
using ObjectDictionary.Services;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ObjectDictionary.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private readonly Realm realm;
        public IEnumerable<ImageData> ImageDatas { get; }
        public Command AddImageCommand { get; }
        public Command<ImageData> ShowConceptsCommand { get; }
        public IEnumerable<CarouselItem> Items { get; set; }

        private double _slidePosition;
        public int _currentIndex;
        public List<Color> _backgroundColors = new List<Color>();
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            realm = Realm.GetInstance();
            ImageDatas = realm.All<ImageData>().OrderByDescending(i => i.Id);
            AddImageCommand = new Command(async () => await AddImageAsync());
            ShowConceptsCommand = new Command<ImageData>(ShowConcept);
            //Items = ImageDatas.Select(
            //    i => new CarouselItem
            //    {
            //        Position = 0,
            //        Type = i.displayDateTime,
            //        ImageSrc = i.path,
            //        BackgroundColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
            //        StartColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
            //        EndColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
            //    });
            ReloadItems();

            var itemList = Items.ToList();
            for (int i = 0; i < itemList.Count; i++)
            {
                var current = itemList[i];
                var next = itemList.Count > i + 1 ? itemList[i + 1] : null;

                if (next != null)
                    _backgroundColors.AddRange(SetGradients(current.BackgroundColor, next.BackgroundColor, 100));
                else
                    _backgroundColors.Add(current.BackgroundColor);
            }
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
                AddItem(imageData);

                await NetworkService.RecognizeImage(photo.Path, imageData);
            }
        }

        private void ShowConcept(ImageData imageData)
        {
            if (imageData == null || !imageData.IsUpdated) return;
            var detailViewModel = new DetailViewModel(imageData);
            NavigationService.Navigate(detailViewModel);
        }
        public double SlidePosition
        {
            get => _slidePosition; set
            {
                if (_slidePosition != value)
                {
                    _slidePosition = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static IEnumerable<Color> SetGradients(Color start, Color end, int steps)
        {
            var colorList = new List<Color>();

            double aStep = ((end.A * 255) - (start.A * 255)) / steps;
            double rStep = ((end.R * 255) - (start.R * 255)) / steps;
            double gStep = ((end.G * 255) - (start.G * 255)) / steps;
            double bStep = ((end.B * 255) - (start.B * 255)) / steps;

            for (int i = 0; i < 100; i++)
            {
                var a = (start.A * 255) + (int)(aStep * i);
                var r = (start.R * 255) + (int)(rStep * i);
                var g = (start.G * 255) + (int)(gStep * i);
                var b = (start.B * 255) + (int)(bStep * i);

                colorList.Add(Color.FromRgba(r / 255, g / 255, b / 255, a / 255));
            }

            return colorList;
        }
        private void ReloadItems()
        {
            var random = new Random();
            Items = ImageDatas.Select(
                    i => new CarouselItem
                    {
                        Position = 0,
                        Type = i.displayDateTime,
                        ImageSrc = i.path,
                        BackgroundColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
                        StartColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
                        EndColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
                    });
        }
        private void AddItem(ImageData imageData)
        {
            var random = new Random();
            Items.Concat(new[]
            {
                new CarouselItem
                {
                    Position = 0,
                        Type = imageData.displayDateTime,
                        ImageSrc = imageData.path,
                        BackgroundColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
                        StartColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
                        EndColor = Color.FromHex(String.Format("#{0:X6}", random.Next(0x1000000))),
                }
            });
            OnPropertyChanged();
        }
    }

}
