using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ObjectDictionary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DictionaryPage : ContentPage
	{

		public DictionaryPage ()
		{
			InitializeComponent ();
		}

        public DictionaryPage (String originalWord)
        {
            InitializeComponent();
            var result = Realms.Realm.GetInstance().All<Models.Dictionary>().Where(it => it.originalWord == originalWord);
            var dislayList = result.ToList();
                //.Select(it => it.en + " " + it.vi).ToList();
            listView.ItemsSource = dislayList;
        }
	}
}