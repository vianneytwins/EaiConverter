namespace EaiConverter.Parser
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class XsdParser
    {
        public List<ClassParameter> Parse(IEnumerable<XNode> inputNodes, string targetNameSpace)
        {
            var classProperties = new List<ClassParameter>();

			foreach(var inputNode in inputNodes)
			{

				var element = (XElement) inputNode;
				string type = string.Empty;
                if (element.Name.LocalName == "element" && element.Name.NamespaceName == XmlnsConstant.xsdNameSpace) {
					if (element.Attribute ("type") != null)
                    {
                        type = this.ConvertToBasicType (element.Attribute ("type").Value.ToString().Remove(0,4));
						classProperties.Add (new ClassParameter {
							Name = element.Attribute("name").Value,
							Type = type
						});
					}
                    else
					{
                        if (element.Attribute("ref") == null)
					    {
					        classProperties.Add(
					            new ClassParameter
					                {
					                    Name = element.Attribute("name").Value,
					                    Type = this.ConvertToComplexType(element.Attribute("name").Value, targetNameSpace),
					                    ChildProperties = this.Parse(element.Nodes())
					                });
					    }
                        else
                        {
                            classProperties.Add(
                                new ClassParameter
                                {
                                    Name = element.Attribute("ref").Value.Split(':')[1],
                                    Type = element.Attribute("ref").Value
                                });
                        }
					}
                }
                if ((element.Name.LocalName == "complexType" || element.Name.LocalName == "sequence") && element.Name.NamespaceName == XmlnsConstant.xsdNameSpace)
				{
					return this.Parse(element.Nodes());
				}

			}
			return classProperties;

		}

        public List<ClassParameter> Parse (IEnumerable<XNode> inputNodes)
        {
            return this.Parse(inputNodes, string.Empty);
        }
        public string ConvertToBasicType (string xsdType)
        {
            return xsdType;
        }

        public string ConvertToComplexType (string type, string targetNamespace)
        {
            if (string.IsNullOrEmpty(targetNamespace))
            {
                    return type;
            }
                return targetNamespace+"."+type;

        }

	}
}

