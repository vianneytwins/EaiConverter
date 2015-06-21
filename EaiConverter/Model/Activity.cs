using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Xml.Linq;

namespace EaiConverter.Model
{
	public class Activity
	{
	public Activity (string name, ActivityType type) {
			this.Name = name;
			this.Type = type;
		}

		public Activity ()
		{
		} 

		public string Name {get; set;}

        public ActivityType Type {get; set;}

		public List<ClassParameter> Parameters {get; set;}

		public IEnumerable<XNode> ObjectXNodes {
			get;
			set;
		}

        public IEnumerable<XNode> InputBindings {get; set;}

		public override string ToString ()
		{
			return string.Format ("[Activity: Name={0}, Type={1}]", Name, Type);
		}
	}
}

