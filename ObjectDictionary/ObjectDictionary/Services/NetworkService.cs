using Clarifai.API;
using Clarifai.DTOs.Inputs;
using ObjectDictionary.Models;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ObjectDictionary.Services
{
    sealed class NetworkService
    {
        private const string CLARIFY_KEY = "b657f18794b44fa7b7c2c7a159fc6cfc";

        public static async void RecognizeImage(string path, ImageData imageData)
        {
            var clarifaiClient = new ClarifaiClient(CLARIFY_KEY);
            var res = await clarifaiClient.PublicModels.GeneralModel.Predict(
                    new ClarifaiFileImage(File.ReadAllBytes(path)
                    )
                    ).ExecuteAsync();

            if (res != null)
            {

                res.Get().Data.ForEach(it =>
                {
                    var concept = new Models.Concept
                    {
                        imageData = imageData,
                        value = it.Name
                    };

                    RealmService.GetInstance().RealmAdd(concept);
                });
            }
        }

    }
}
