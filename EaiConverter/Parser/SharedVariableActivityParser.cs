using System;
using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{
    public class SharedVariableActivityParser :IActivityParser
	{
        #region IActivityParser implementation
        public Activity Parse(XElement inputElement)
        {
            var activity = new SharedVariableActivity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            if(activity.Type == ActivityType.setSharedVariableActivityType)
            {
                activity.IsSetterActivity = true;
            }
            else
            {
                activity.IsSetterActivity = false;
            }

            var configElement = inputElement.Element ("config");

            activity.VariableConfig = XElementParserUtils.GetStringValue(configElement.Element("variableConfig"));

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

