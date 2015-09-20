using System.Xml.Linq;
using EaiConverter.Builder;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
	public class XslSyntaxElement
	{
		public string Name
		{
			get;
			set;
		}

		public string ReturnType
		{
			get;
			set;
		}

		public string PackageName
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public List<XslSyntaxElement> ChildElements {
			get;
			set;
		}

		public override string ToString ()
		{
			return string.Format ("{0} {1} = {2};" , ReturnType, Name, Value);
		}

	}



}

