namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;
	using EaiConverter.Builder.Utils;
	using EaiConverter.CodeGenerator.Utils;


    public class XmlParseActivityParser : IActivityParser
    {      
		private readonly XsdParser xsdParser;

		public XmlParseActivityParser(XsdParser xsdParser)
		{
			this.xsdParser = xsdParser;
		}

		public Activity Parse(XElement inputElement)
        {
            var xmlParseActivity = new XmlParseActivity();
            xmlParseActivity.Name = inputElement.Attribute("name").Value;
            xmlParseActivity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;
            var configElement = inputElement.Element("config");

            if (configElement.Element("term").Attribute("ref") != null)
            {
                xmlParseActivity.XsdReference = configElement.Element("term").Attribute("ref").Value;
            }
            else
            {
                xmlParseActivity.ObjectXNodes = configElement.Element("term").Nodes();
            }

            xmlParseActivity.InputBindings = inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();
            xmlParseActivity.Parameters = new XslParser().Parse(xmlParseActivity.InputBindings);
            return xmlParseActivity;
        }
    }
}

