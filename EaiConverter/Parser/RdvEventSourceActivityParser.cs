using EaiConverter.Model;
using EaiConverter.Parser.Utils;

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

            //TODO manage REF or XSD
            if (configElement.Element("XsdString").Attribute("ref") != null)
            {
                activity.XsdString = configElement.Element("XsdString").Attribute("ref").ToString();
            }

            if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null )
            {
                activity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();
                activity.Parameters = new XslParser().Build(activity.InputBindings);
            }

            return activity;
        }
        #endregion
	}

}

