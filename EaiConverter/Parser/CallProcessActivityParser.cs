namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class CallProcessActivityParser : IActivityParser
	{
        public Activity Parse (XElement inputElement)
		{
            var callProcessActivity = new CallProcessActivity ();

            callProcessActivity.Name = inputElement.Attribute ("name").Value;
            callProcessActivity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;
			var configElement = inputElement.Element ("config");

            callProcessActivity.ProcessName = XElementParserUtils.GetStringValue(configElement.Element("processName"));


            if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null)
            {
                callProcessActivity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();
                callProcessActivity.Parameters = new XslParser().Parse(callProcessActivity.InputBindings);
            }

            return callProcessActivity;
		}
	}

}

