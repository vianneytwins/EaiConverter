using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;
using EaiConverter.Mapper;

namespace EaiConverter.Parser
{

    public class JavaActivityParser : IActivityParser
	{
        public Activity Parse (XElement inputElement)
		{
            var activity = new JavaActivity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoPrefix + "type").Value;
			var configElement = inputElement.Element ("config");

            activity.FileName = XElementParserUtils.GetStringValue(configElement.Element("fileName"));
            activity.PackageName = XElementParserUtils.GetStringValue(configElement.Element("packageName"));
            activity.FullSource = XElementParserUtils.GetStringValue(configElement.Element("fullsource"));

            activity.InputBindings = inputElement.Element (XmlnsConstant.tibcoPrefix + "inputBindings").Nodes();

            if (inputElement.Element(XmlnsConstant.tibcoPrefix + "inputBindings") != null && inputElement.Element(XmlnsConstant.tibcoPrefix + "inputBindings").Element("javaCodeActivityInput") != null)
            {
                activity.InputBindings = inputElement.Element(XmlnsConstant.tibcoPrefix + "inputBindings").Element("javaCodeActivityInput").Nodes();
                activity.Parameters = new XslParser().Build(activity.InputBindings);
            }

            return activity;
		}
	}

}

