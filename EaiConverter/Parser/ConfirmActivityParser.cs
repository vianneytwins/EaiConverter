using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{
    public class ConfirmActivityParser : IActivityParser
    {
        #region IActivityParser implementation

        public Activity Parse(XElement inputElement)
        {
            var activity = new ConfirmActivity();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;

            var configElement = inputElement.Element ("config");

            activity.ActivityNameToConfirm = XElementParserUtils.GetStringValue(configElement.Element("ConfirmEvent")).Replace(' ','_');

            return activity;
        }

        #endregion
    }
}

