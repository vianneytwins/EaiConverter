using System;
using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Model
{
    public class SharedVariableActivity : Activity
	{

        public SharedVariableActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public SharedVariableActivity () 
        {
        }

        public string VariableConfig {get; set;}

        public bool IsSetterActivity {get; set;}
	}

}

