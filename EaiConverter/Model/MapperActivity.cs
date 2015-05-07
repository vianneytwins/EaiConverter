using System;
using System.Xml.Linq;

namespace EaiConverter.Model
{
    public class MapperActivity : Activity
    {
        public MapperActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public MapperActivity () 
        {
        }

        public string XsdReference {get; set;}

        public XElement InputBinding {get; set;}
    }
}

