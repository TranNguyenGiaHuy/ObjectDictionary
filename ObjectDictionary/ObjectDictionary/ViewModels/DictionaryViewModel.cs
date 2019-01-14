using ObjectDictionary.Models;
using ObjectDictionary.Services;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDictionary.ViewModels
{
    class DictionaryViewModel
    {
        private Realm Realm;
        private readonly string OriginalWord;
        public IEnumerable<Dictionary> Dictionaries { get; }

        public DictionaryViewModel(string originalWord)
        {
            Realm = Realm.GetInstance();
            OriginalWord = originalWord;
            Dictionaries = Realm.All<Dictionary>().Where(it => it.originalWord == OriginalWord);
            //Dictionaries = dictionaries;
        }
    }
}
