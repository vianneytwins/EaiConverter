using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace EaiConverter.Model
{
    public class WriteToLogActivity : Activity
    {
        public WriteToLogActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public WriteToLogActivity () 
        {
        }

        public string Role {get; set;}
    }
}

