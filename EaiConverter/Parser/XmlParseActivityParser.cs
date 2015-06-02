using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;
using EaiConverter.Mapper;

namespace EaiConverter.Parser
{

    public class XmlParseActivityParser : IActivityParser
	{
        public Activity Parse (XElement inputElement)
		{
            var xmlParseActivity = new XmlParseActivity ();

			xmlParseActivity.Name = inputElement.Attribute ("name").Value;
            xmlParseActivity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;
			var configElement = inputElement.Element ("config");

            xmlParseActivity.XsdReference = configElement.Element("term").Attribute("ref").Value;
			
            xmlParseActivity.InputBindings = inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();

            xmlParseActivity.Parameters = new XslParser().Build(xmlParseActivity.InputBindings);

			return xmlParseActivity;
		}
	}

}

