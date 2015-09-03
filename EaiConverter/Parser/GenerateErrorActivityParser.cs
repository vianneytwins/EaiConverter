namespace EaiConverter.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class GenerateErrorActivityParser : IActivityParser
	{
        public Activity Parse(XElement inputElement)
		{
            var activity = new GenerateErrorActivity();

            activity.Name = inputElement.Attribute("name").Value;
            activity.Type = (ActivityType) inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "type").Value;
			var configElement = inputElement.Element("config");

            activity.FaultName = XElementParserUtils.GetStringValue(configElement.Element("faultName"));
			
            if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null && inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element(XmlnsConstant.generateErrorActivityNameSpace + "ActivityInput") != null)
            {
                activity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element(XmlnsConstant.generateErrorActivityNameSpace + "ActivityInput").Nodes();
                activity.Parameters = new XslParser().Build(activity.InputBindings);
            }
            
            return activity;
		}
	}

}

