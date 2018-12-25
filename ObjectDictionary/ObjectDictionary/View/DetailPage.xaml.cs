using Acr.UserDialogs;
using Newtonsoft.Json;
using ObjectDictionary.Models;
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
		public DetailPage ()
		{
			InitializeComponent ();
		}

        public DetailPage (ImageData imageData)
        {
            InitializeComponent();
            listView.ItemsSource = Realms.Realm.GetInstance().All<Concept>().Where(it => it.imageData == imageData);
        }

        private async void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var concept = (Concept)e.SelectedItem;
            var value = concept.value;

            var getDictionaryResult = Realms.Realm.GetInstance().All<Dictionary>().Count(it => it.originalWord == value);

            if (getDictionaryResult == 0)
            {
                await addDictonaryOfConcept(concept);
            }
            await this.Navigation.PushAsync(
                new DictionaryPage(concept.value)
                );
        }

        private async Task addDictonaryOfConcept(Concept concept)
        {
            UserDialogs.Instance.ShowLoading("Get Dictionary...");

            var client = new HttpClient();
            String url = "https://api.tracau.vn/WBBcwnwQpV89/s/" + concept.value + "/en";
            HttpResponseMessage message = await client.GetAsync(url);
            message.EnsureSuccessStatusCode();
            String responseBody = await message.Content.ReadAsStringAsync();

            if (responseBody != null && responseBody != "")
            {
                // parse json to object
                var result = JsonConvert.DeserializeObject<TraCauResult>(responseBody);

                var realm = Realms.Realm.GetInstance();

                result.sentences.ForEach(it =>
                {
                    var dictonary = new Dictionary
                    {
                        originalWord = concept.value,
                        en = StripHTML(it.fields.en),
                        vi = it.fields.vi
                    };
                    realm.Write(() =>
                    {
                        realm.Add(dictonary);
                    });
                });
            }

            UserDialogs.Instance.HideLoading();
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}