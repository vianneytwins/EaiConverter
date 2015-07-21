using EaiConverter.Builder.Utils;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class MapperActivityParser : IActivityParser
    {

		private readonly XsdParser xsdParser;

		public MapperActivityParser(XsdParser xsdParser)
		{
			this.xsdParser = xsdParser;
		}

        public Activity Parse(XElement inputElement)
        {
            var mapperActivity = new MapperActivity();

            mapperActivity.Name = inputElement.Attribute("name").Value;
            var xElement = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "type");
            if (xElement != null)
            {
                mapperActivity.Type = (ActivityType) xElement.Value;
            }

            var configElement = inputElement.Element("config");

            // If the ref is not null the Xsd has been define somewhere else otherwise it's define in line
            if (configElement.Element("element").Attribute("ref") != null)
            {
                mapperActivity.XsdReference = configElement.Element("element").Attribute("ref").Value;
            }
            else
            {
                mapperActivity.ObjectXNodes = configElement.Element("element").Nodes();
		    }

            mapperActivity.InputBindings = inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();

            mapperActivity.Parameters = new XslParser().Build(mapperActivity.InputBindings);

            return mapperActivity;
        }
    }

}

