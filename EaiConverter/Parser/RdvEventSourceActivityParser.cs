using EaiConverter.Model;
using EaiConverter.Parser.Utils;
using System.Collections.Generic;

namespace EaiConverter.Parser
{
    public class RdvEventSourceActivityParser : IActivityParser
	{
        #region IActivityParser implementation
        public Activity Parse(System.Xml.Linq.XElement inputElement)
        {
            var activity = new RdvEventSourceActivity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            var configElement = inputElement.Element ("config");

            activity.Subject = XElementParserUtils.GetStringValue(configElement.Element("subject"));
            activity.SharedChannel = XElementParserUtils.GetStringValue(configElement.Element("sharedChannel"));

            //TODO manage REF or XSD : is it really used ? Let's do something dirty and assume it's always a string name message
			/*if (configElement.Element ("XsdString").Attribute ("ref") != null) {
				activity.XsdStringReference = configElement.Element("XsdString").Attribute("ref").ToString();
			}
			else
			{
				activity.ObjectXNodes = configElement.Element("XsdString").Nodes();
				var activityParameters = new XsdParser().Parse (configElement.Element("XsdString").Nodes(), string.Empty);
				activity.Parameters = activityParameters;
			}*/


			activity.Parameters = new List<ClassParameter>
			{
				new ClassParameter{
					Name = "message",
					Type = "System.String"
				}
			};

            return activity;
        }
        #endregion
	}

}

