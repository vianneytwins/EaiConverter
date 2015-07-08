using System;
using System.Xml.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{

    public class SleepActivityParser : IActivityParser
	{
        public Activity Parse(XElement inputElement)
        {
            var activity = new SleepActivity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;
 
            if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null && inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element(XmlnsConstant.sleeptibcoActivityNameSpace + "SleepInputSchema") != null)
            {
                activity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element(XmlnsConstant.sleeptibcoActivityNameSpace + "SleepInputSchema").Nodes();
                activity.Parameters = new XslParser().Build(activity.InputBindings);
            }
            return activity;
        }
	}

}

