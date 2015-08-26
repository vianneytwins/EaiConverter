namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class TimerActivityParser : IActivityParser
    {
        public Activity Parse(XElement inputElement)
        {
            var activity = new TimerActivity();

            activity.Name = inputElement.Attribute("name").Value;
            activity.Type = (ActivityType)inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            var configElement = inputElement.Element("config");

            activity.FrequencyIndex = XElementParserUtils.GetStringValue(configElement.Element("FrequencyIndex"));
            activity.Frequency = XElementParserUtils.GetBoolValue(configElement.Element("Frequency"));
            activity.TimeInterval = XElementParserUtils.GetIntValue(configElement.Element("TimeInterval"));
            activity.StartTime = XElementParserUtils.GetStringValue(configElement.Element("StartTime"));

            return activity;
        }
    }
}
