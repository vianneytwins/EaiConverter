using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace EaiConverter.Model
{
	public class ProcessVariable
	{
        public string Name { get; set;}

        public IEnumerable<XNode> ObjectXNodes {
            get;
            set;
        }
	}

}

