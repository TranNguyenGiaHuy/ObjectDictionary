using System;
using System.Linq;
using Xamarin.Forms;
using Realms;
using ObjectDictionary.ViewModels;
using CarouselView.FormsPlugin.Abstractions;

namespace ObjectDictionary
{
    public partial class MainPage : ContentPage
    {
        public Realm realm = Realm.GetInstance();
        public MainPage()
        {
            InitializeComponent();
        }

        //private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    var selectedItem = (ImageData)e.SelectedItem;
        //    ((ListView)sender).SelectedItem = null;

        //    ((MainViewModel)BindingContext).ShowConceptsCommand.Execute(selectedItem);
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Need to start somewhere...
            page.BackgroundColor = ((MainViewModel)BindingContext)._backgroundColors.FirstOrDefault();
        }

        public void Handle_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            ((MainViewModel)BindingContext)._currentIndex = e.NewValue;
            ((MainViewModel)BindingContext).SlidePosition = 0;
        }

        public void Handle_Scrolled(object sender, CarouselView.FormsPlugin.Abstractions.ScrolledEventArgs e)
        {
            int position = 0;

            if (e.Direction == ScrollDirection.Right)
                position = (int)((((MainViewModel)BindingContext)._currentIndex * 100) + e.NewValue);
            else if (e.Direction == ScrollDirection.Left)
                position = (int)((((MainViewModel)BindingContext)._currentIndex * 100) - e.NewValue);

            // Set the background color of our page to the item in the color gradient
            // array, matching the current scrollindex.
            if (position > ((MainViewModel)BindingContext)._backgroundColors.Count - 1)
                page.BackgroundColor = ((MainViewModel)BindingContext)._backgroundColors.LastOrDefault();
            else if (position < 0)
                page.BackgroundColor = ((MainViewModel)BindingContext)._backgroundColors.FirstOrDefault();
            else
                page.BackgroundColor = ((MainViewModel)BindingContext)._backgroundColors[position];

            // Save the current scroll position
            ((MainViewModel)BindingContext).SlidePosition = e.NewValue;

            if (e.Direction == ScrollDirection.Right)
            {
                // When scrolling right, we offset the current item and the next one.
                ((MainViewModel)BindingContext).Items.ToList()[((MainViewModel)BindingContext)._currentIndex].Position = -((MainViewModel)BindingContext).SlidePosition;

                if (((MainViewModel)BindingContext)._currentIndex < ((MainViewModel)BindingContext).Items.ToList().Count - 1)
                {
                    ((MainViewModel)BindingContext).Items.ToList()[((MainViewModel)BindingContext)._currentIndex + 1].Position = 100 - ((MainViewModel)BindingContext).SlidePosition;
                }
            }
            else if (e.Direction == ScrollDirection.Left)
            {
                // When scrolling left, we offset the current item and the previous one.
                ((MainViewModel)BindingContext).Items.ToList()[((MainViewModel)BindingContext)._currentIndex].Position = ((MainViewModel)BindingContext).SlidePosition;

                if (((MainViewModel)BindingContext)._currentIndex > 0)
                {
                    ((MainViewModel)BindingContext).Items.ToList()[((MainViewModel)BindingContext)._currentIndex - 1].Position = -100 + ((MainViewModel)BindingContext).SlidePosition;
                }
            }
        }

        private void RoundedButton_Clicked(object sender, EventArgs e)
        {
            var selectedItem = ((MainViewModel)BindingContext).ImageDatas.ToList()[((MainViewModel)BindingContext)._currentIndex];
            ((MainViewModel)BindingContext).ShowConceptsCommand.Execute(selectedItem);
        }
    }

}
