namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class EngineCommandActivityParser : IActivityParser
    {
        public Activity Parse(XElement inputElement)
        {
            var activity = new EngineCommandActivity
                               {
                                   Name = inputElement.Attribute("name").Value,
                                   Type = (ActivityType) inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "type").Value
                               };

            var configElement = inputElement.Element("config");

            if (configElement != null)
            {
                activity.Command = XElementParserUtils.GetStringValue(configElement.Element("command"));
            }

            return activity;
        }
    }

}

