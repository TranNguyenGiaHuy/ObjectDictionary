using Acr.UserDialogs;
using Clarifai.API;
using Clarifai.DTOs.Inputs;
using Newtonsoft.Json;
using ObjectDictionary.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObjectDictionary.Services
{
    sealed class NetworkService
    {
        private const string CLARIFY_KEY = "b657f18794b44fa7b7c2c7a159fc6cfc";

        public static async Task RecognizeImage(string path, ImageData imageData)
        {
            var clarifaiClient = new ClarifaiClient(CLARIFY_KEY);
            var res = await clarifaiClient.PublicModels.GeneralModel.Predict(
                    new ClarifaiFileImage(File.ReadAllBytes(path)
                    )
                    ).ExecuteAsync();

            if (res != null)
            {

                res.Get().Data.ForEach(async it =>
                {
                    var concept = new Concept
                    {
                        imageData = imageData,
                        value = it.Name
                    };

                    //RealmService.GetInstance().RealmAdd(concept);
                    Realm.GetInstance().Write(() =>
                    {
                        Realm.GetInstance().Add(concept);
                    });
                    //var result = Realm.GetInstance().All<ImageData>();
                    //Console.WriteLine(result.ToString());

                    //await AddDictonaryOfConcept(concept);
                });
                Realm.GetInstance().Write(() =>
                {
                    imageData.IsUpdated = true;
                });
            }
        }
        public static async Task AddDictonaryOfConcept(Concept concept)
        {
            var realm = Realms.Realm.GetInstance();
            var countDictionary = realm.All<Dictionary>().Count(it => it.originalWord == concept.value);
            if (countDictionary > 0)
            {
                return;
            }

            var client = new HttpClient();
            String url = "https://api.tracau.vn/WBBcwnwQpV89/s/" + concept.value + "/en";
            HttpResponseMessage message = await client.GetAsync(url);
            message.EnsureSuccessStatusCode();
            String responseBody = await message.Content.ReadAsStringAsync();

            if (responseBody != null && responseBody != "")
            {
                // parse json to object
                var result = JsonConvert.DeserializeObject<TraCauResult>(responseBody);


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
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

    }
}
