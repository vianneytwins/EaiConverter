using System.Collections.Generic;
using System.Xml.Linq;

namespace EaiConverter.Model
{
	public class ProcessVariable
	{
        public ClassParameter Parameter { get; set;}

        public IEnumerable<XNode> ObjectXNodes
        {
            get;
            set;
        }
	}

}

