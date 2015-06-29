using System;
using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Model
{
    public class RdvPublishActivity : Activity
	{

        public RdvPublishActivity (string name, ActivityType type) : base (name, type)
        {
        }

        public RdvPublishActivity () 
        {
        }

        public string Subject
        {
            get;
            set;
        }

        public string SharedChannel
        {
            get;
            set;
        }

        public bool? isXmlEncode
        {
            get;
            set;
        }

        public string XsdString
        {
            get;
            set;
        }
	}

}

