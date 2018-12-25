using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectDictionary.Services
{
    class RealmService
    {
        private static RealmService _instance;
        private static object syncLock = new object();

        private Realm realm;

        protected RealmService()
        {
            realm = Realm.GetInstance();
        }

        public static RealmService GetInstance()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new RealmService();
                    }
                }
            }

            return _instance;
        }

        public void RealmAdd(RealmObject o)
        {
            realm.Write(() =>
            {
                realm.Add(o);
            });
        }
    }
}
