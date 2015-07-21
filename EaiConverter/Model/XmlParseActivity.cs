using System.Collections.Generic;

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
	}

}

