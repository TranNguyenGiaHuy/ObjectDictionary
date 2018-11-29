using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ObjectDictionary.Models
{
    public class Concept : RealmObject
    {
        public string value { get; set; }
        public ImageData imageData { get; set; }
    }
}
