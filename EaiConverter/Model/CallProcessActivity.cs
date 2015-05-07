using System;
using System.Xml.Linq;
using System.Collections.Generic;

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

        public IEnumerable<XNode> InputBindings {get; set;}
    }
}

