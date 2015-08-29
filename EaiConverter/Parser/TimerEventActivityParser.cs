using System;

namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class TimerEventActivityParser : IActivityParser
    {
        public Activity Parse(XElement inputElement)
        {
            var activity = new TimerEventActivity();

            activity.Name = inputElement.Attribute("name").Value;
            activity.Type = (ActivityType)inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            var configElement = inputElement.Element("config");

            activity.IntervalUnit = (TimerUnit)Enum.Parse(typeof(TimerUnit), XElementParserUtils.GetStringValue(configElement.Element("FrequencyIndex")));
            activity.RunOnce = XElementParserUtils.GetBoolValue(configElement.Element("Frequency"));
            activity.TimeInterval = XElementParserUtils.GetIntValue(configElement.Element("TimeInterval"));
            activity.StartTime = new DateTime(1970, 1, 1);
            activity.StartTime = activity.StartTime.AddMilliseconds(double.Parse(XElementParserUtils.GetStringValue(configElement.Element("StartTime"))));
      

            return activity;
        }
    }
}
