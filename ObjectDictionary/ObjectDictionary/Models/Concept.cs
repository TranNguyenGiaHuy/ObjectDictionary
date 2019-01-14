using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ObjectDictionary.Models
{
    public class Concept : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsUpdated { get; set; } = false;
        public string value { get; set; }
        public ImageData imageData { get; set; }
    }
}
