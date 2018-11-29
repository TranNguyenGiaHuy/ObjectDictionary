using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectDictionary.Models
{
    class TraCauResult
    {
        public String language { get; set; }
        public List<Sentence> sentences { get; set; }
        public List<Object> suggestions { get; set; }
        public List<Object> tratu { get; set; }
    }

    class Sentence
    {
        public String _id { get; set; }
        public Field fields { get; set; }
    }

    class Field
    {
        public String en;
        public String vi;
    }
}
