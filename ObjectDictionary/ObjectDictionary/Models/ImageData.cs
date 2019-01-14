using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ObjectDictionary.Models
{
    public class ImageData : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsUpdated { get; set; } = false;
        public string path { get; set; }
        public DateTimeOffset dateTimeCreated { get; set; }
        public string displayDateTime { get; set; }
        public IList<Concept> concepts { get; }
    }
}
