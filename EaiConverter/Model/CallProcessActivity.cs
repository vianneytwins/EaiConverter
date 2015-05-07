using System;
using System.Xml.Linq;

namespace EaiConverter.Model
{
    public class CallProcessActivity : Activity
    {
        public CallProcessActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public CallProcessActivity () 
        {
        }

        public string ProcessName {get; set;}

        public XElement InputBinding {get; set;}
    }
}

