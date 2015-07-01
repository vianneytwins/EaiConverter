using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{

    public class JavaActivityParser : IActivityParser
	{
        public Activity Parse (XElement inputElement)
		{
            var activity = new JavaActivity ();

            activity.Name = inputElement.Attribute ("name").Value;
            activity.Type = (ActivityType) inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "type").Value;

			var configElement = inputElement.Element ("config");

            activity.FileName = XElementParserUtils.GetStringValue(configElement.Element("fileName"));
            activity.PackageName = XElementParserUtils.GetStringValue(configElement.Element("packageName"));
            activity.FullSource = XElementParserUtils.GetStringValue(configElement.Element("fullsource"));

            activity.InputData = this.GetInputOrOutputData(configElement.Element("inputData"));
            activity.OutputData = this.GetInputOrOutputData(configElement.Element("outputData"));
            

            activity.InputBindings = inputElement.Element (XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Nodes();

            if (inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings") != null && inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element("javaCodeActivityInput") != null)
            {
                activity.InputBindings = inputElement.Element(XmlnsConstant.tibcoProcessNameSpace + "inputBindings").Element("javaCodeActivityInput").Nodes();
                activity.Parameters = new XslParser().Build(activity.InputBindings);
            }

            return activity;
		}

        public List<ClassParameter> GetInputOrOutputData(XElement dataElement)
        {
            if (dataElement == null) {
                return null;
            }

            IEnumerable<XElement> rows = from element in dataElement.Elements("row")
                select element;
            
            if (rows == null) {
                return null;
            }

            var datas = new List<ClassParameter>();
            foreach (var row in rows)
            {
                var data = new ClassParameter{
                    Name = XElementParserUtils.GetStringValue(row.Element("fieldName")),
                    Type = XElementParserUtils.GetStringValue(row.Element("fieldType"))
                };
                datas.Add(data);
            }
            return datas;
        }
	}

}

