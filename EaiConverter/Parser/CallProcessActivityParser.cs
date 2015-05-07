using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{

    public class CallProcessActivityParser
	{
        public CallProcessActivity Parse (XElement inputElement)
		{
            var callProcessActivity = new CallProcessActivity ();

            callProcessActivity.Name = inputElement.Attribute ("name").Value;
            callProcessActivity.Type = (ActivityType) inputElement.Element (TibcoBWProcessLinqParser.tibcoPrefix + "type").Value;
			var configElement = inputElement.Element ("config");

            callProcessActivity.ProcessName = XElementParserUtils.GetStringValue(configElement.Element("processName"));
			
			// TODO : retrieve the inputbinding

            return callProcessActivity;
		}
	}

}

