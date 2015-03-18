using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace EaiConverter.Parser
{
	public class XsdParser
	{
		public XsdParser ()
		{
		}
			
		public List<ClassParameter> Parse (IEnumerable<XNode> inputNodes)
		{
			XNamespace xsdNameSpace = "http://www.w3.org/2001/XMLSchema";

			var classProperties = new List <ClassParameter> ();

			foreach(var inputNode in inputNodes)
			{

				var element = (XElement) inputNode;
				string type = string.Empty;
				if (element.Name.LocalName == "element" && element.Name.NamespaceName == xsdNameSpace) {
					if (element.Attribute ("type") != null) {
						type = element.Attribute ("type").Value.ToString().Remove(0,4);
						classProperties.Add (new ClassParameter {
							Name = element.Attribute ("name").Value,
							Type = type
						});
					} else {
						classProperties.Add (new ClassParameter {
							Name = element.Attribute ("name").Value,
							Type = element.Attribute ("name").Value,
							ChildProperties = this.Parse (element.Nodes())
						});
					}

				}
				if ((element.Name.LocalName == "complexType" || element.Name.LocalName == "sequence") && element.Name.NamespaceName == xsdNameSpace)
				{
					return this.Parse (element.Nodes ());
				}

			}
			return classProperties;

		}

	}
}

