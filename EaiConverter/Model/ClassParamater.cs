using System.Collections.Generic;

namespace EaiConverter.Model
{
	public class ClassParameter
	{
		public string Type { get; set; }
		public string Name { get; set; }

		public string DefaultValue {
			get;
			set;
		}

		public bool IsAConstant {
			get;
			set;
		}

		public bool IsReadOnly {
			get;
			set;
		}

		public string SpecialOption {
			get;
			set;
		}

		public List<ClassParameter> ChildProperties { get; set;}
	}

}

