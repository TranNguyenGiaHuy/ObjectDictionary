using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ObjectDictionary.Models
{
    public class ImageData : RealmObject
    {
        public string path { get; set; }
        public DateTimeOffset dateTimeCreated { get; set; }
        public string displayDateTime { get; set; }
        public IList<Concept> concepts { get; }
    }
}
