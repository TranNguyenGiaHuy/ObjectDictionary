using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ObjectDictionary.Models
{
    public class Dictionary : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsUpdated { get; set; } = false;
        public String en { get; set; }
        public String vi { get; set; }
        public String originalWord { get; set; }
    }
}
