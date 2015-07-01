using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{

    public class WriteToLogActivityParser : IActivityParser
	{
        public Activity Parse (XElement inputElement)
		{
            var activity = new WriteToLogActivity();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;
			var configElement = inputElement.Element ("config");

            activity.Role = XElementParserUtils.GetStringValue(configElement.Element("role"));
			

            if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null && inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element(XmlnsConstant.writeToLogActivityNameSpace + "ActivityInput") != null)
            {
                activity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element(XmlnsConstant.writeToLogActivityNameSpace + "ActivityInput").Nodes();
                activity.Parameters = new XslParser().Build(activity.InputBindings);
            }


            return activity;
		}
	}

}

