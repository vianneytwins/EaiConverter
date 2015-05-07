using System;
using System.Collections.Generic;
using EaiConverter.Model;
using System.Xml.Linq;

namespace EaiConverter.Model
{

	public class XmlParseActivity : Activity
	{
        public XmlParseActivity (string name, ActivityType type) : base (name, type)
		{
		}

		public XmlParseActivity () 
		{
		}
            
        public string XsdReference {get; set;}

        public XElement InputBinding {get; set;}

	}

}

