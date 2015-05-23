using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{

    public class MapperActivityParser : IActivityParser
	{
        public Activity Parse (XElement inputElement)
		{
            var mapperActivity = new MapperActivity ();

            mapperActivity.Name = inputElement.Attribute ("name").Value;
            mapperActivity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoPrefix + "type").Value;
			var configElement = inputElement.Element ("config");

            mapperActivity.XsdReference = configElement.Element("element").Attribute("ref").Value;
			
            mapperActivity.InputBindings = inputElement.Element (XmlnsConstant.tibcoPrefix + "inputBindings").Nodes();

            return mapperActivity;
		}
	}

}

