using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{
    public class SetSharedVariableActivityParser :IActivityParser
	{
        #region IActivityParser implementation
        public Activity Parse(XElement inputElement)
        {
            var activity = new SetSharedVariableActivity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            var configElement = inputElement.Element ("config");

            activity.VariableConfig = XElementParserUtils.GetStringValue(configElement.Element("variableConfig"));
            activity.ShowResult = XElementParserUtils.GetBoolValue(configElement.Element("showResult"));

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

