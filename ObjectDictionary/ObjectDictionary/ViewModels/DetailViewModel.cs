using Acr.UserDialogs;
using ObjectDictionary.Models;
using ObjectDictionary.Services;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ObjectDictionary.ViewModels
{
    class DetailViewModel
    {
        private Realm Realm;
        private readonly ImageData ImageData;
        public IEnumerable<Concept> Concepts { get; }
        public Command<string> SpeakTextCommand { get; }
        //public Command SelectConceptCommand { get; }

        public DetailViewModel(ImageData imageData)
        {
            Realm = Realm.GetInstance();
            ImageData = imageData;
            Concepts = Realm.All<Concept>().Where(it => it.imageData == imageData);
            //SelectConceptCommand = new Command(async (param) => await SelectConcept((Concept)param));
            SpeakTextCommand = new Command<string>((text) => SpeakText(text));
        }

        private async Task SpeakText(string text)
        {
            await TextToSpeech.SpeakAsync(text);
        }

        //private async Task SelectConcept(Concept concept)
        //{
        //    //UserDialogs.Instance.ShowLoading("Getting Dictionary");
        //    //var dictionaries = await NetworkService.AddDictonaryOfConcept(concept);
        //    ////var value = concept.value;

        //    //if (dictionaries != null && dictionaries.Count() )
        //    //{
        //    //    var dictionaryViewModel = new DictionaryViewModel(dictionaries);
        //    //    await NavigationService.Navigate(dictionaryViewModel);
        //    //}
        //    //UserDialogs.Instance.HideLoading();
        //}
    }
}
