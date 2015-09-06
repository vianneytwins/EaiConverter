using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{
    public class RdvPublishActivityParser :IActivityParser
	{
        #region IActivityParser implementation
        public Activity Parse(XElement inputElement)
        {
            var activity = new RdvPublishActivity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            var configElement = inputElement.Element ("config");

            activity.Subject = XElementParserUtils.GetStringValue(configElement.Element("subject"));
            activity.SharedChannel = XElementParserUtils.GetStringValue(configElement.Element("sharedChannel"));
            activity.isXmlEncode = XElementParserUtils.GetBoolValue(configElement.Element("xmlEncoding"));

            if (configElement.Element("XsdString").Attribute("ref") != null)
            {
                activity.XsdString = configElement.Element("XsdString").Attribute("ref").ToString();
            }
            else
            {
                activity.ObjectXNodes = configElement.Element("XsdString").Nodes();
            }

            if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null )
            {
                activity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();
                activity.Parameters = new XslParser().Parse(activity.InputBindings);
            }

            return activity;
        }
        #endregion
        
	}

}

