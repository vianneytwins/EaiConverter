using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;

namespace EaiConverter.Parser
{
	public class GlobalVariableParser
	{
        public List<GlobalVariable> Parse(XElement allFileElement)
        {
            //var repoElement = allFileElement.Element(XmlnsConstant.globalVariableNameSpace + "repository");
            var globalVariablesElement = allFileElement.Element(XmlnsConstant.globalVariableNameSpace + "globalVariables");

            IEnumerable<XElement> xElement = from element in globalVariablesElement.Elements (XmlnsConstant.globalVariableNameSpace + "globalVariable")
                select element;
            if (xElement == null) {
                return null;
            }

            var globalVariables = new List<GlobalVariable>();
            foreach (var element in xElement)
            {
                var globalVariable = new GlobalVariable
                    {
                        Name = element.Element(XmlnsConstant.globalVariableNameSpace + "name").Value,
                        Value = element.Element(XmlnsConstant.globalVariableNameSpace + "value").Value,
                        Type = element.Element(XmlnsConstant.globalVariableNameSpace + "type").Value
                    };
                globalVariables.Add(globalVariable);
            }
            return globalVariables;

        }
	}

}

