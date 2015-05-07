using System;
using System.Collections.Generic;
using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{

    public class AssignActivityParser
	{
        public AssignActivity Parse (XElement inputElement)
		{
            var assignActivity = new AssignActivity ();

            assignActivity.Name = inputElement.Attribute ("name").Value;
            assignActivity.Type = (ActivityType) inputElement.Element (TibcoBWProcessLinqParser.tibcoPrefix + "type").Value;
			var configElement = inputElement.Element ("config");

            assignActivity.VariableName = XElementParserUtils.GetStringValue(configElement.Element("variableName"));
			
			// TODO : retrieve the inputbinding

            return assignActivity;
		}
	}

}

