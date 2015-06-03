using System;
using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{
    public class NullActivityParser : IActivityParser
    {
        #region IActivityParser implementation

        public Activity Parse(XElement inputElement)
        {
            var activity = new Activity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            return activity;
        }

        #endregion
    }
}

