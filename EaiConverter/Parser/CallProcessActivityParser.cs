using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;
using EaiConverter.Mapper;

namespace EaiConverter.Parser
{

    public class CallProcessActivityParser : IActivityParser
	{
        public Activity Parse (XElement inputElement)
		{
            var callProcessActivity = new CallProcessActivity ();

            callProcessActivity.Name = inputElement.Attribute ("name").Value;
            callProcessActivity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoPrefix + "type").Value;
			var configElement = inputElement.Element ("config");

            callProcessActivity.ProcessName = XElementParserUtils.GetStringValue(configElement.Element("processName"));


            if (inputElement.Element(XmlnsConstant.tibcoPrefix + "inputBindings") != null)
            {
                callProcessActivity.InputBindings = inputElement.Element(XmlnsConstant.tibcoPrefix + "inputBindings").Nodes();
                callProcessActivity.Parameters = new XslParser().Build(callProcessActivity.InputBindings);
            }

            return callProcessActivity;
		}
	}

}

