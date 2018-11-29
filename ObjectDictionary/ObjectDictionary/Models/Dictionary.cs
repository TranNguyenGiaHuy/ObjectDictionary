using System;
using System.Collections.Generic;
using System.Text;
using Realms;

namespace ObjectDictionary.Models
{
    public class Dictionary : RealmObject
    {
        public String en { get; set; }
        public String vi { get; set; }
        public String originalWord { get; set; }
    }
}
