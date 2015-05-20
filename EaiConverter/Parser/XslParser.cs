using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.CodeDom;

namespace EaiConverter.Mapper
{
    public class XslParser
    {


        XNamespace xslNameSpace = "http://w3.org/1999/XSL/Transform";


        public List<ClassParameter> Build (IEnumerable<XNode> inputNodes){

            var paramaters = new List<ClassParameter>();
            if (inputNodes == null)
            {
                return paramaters;
            }
            foreach(var inputNode in inputNodes)
            {
                var element = (XElement) inputNode;
                if (element.Name.NamespaceName != this.xslNameSpace) {
                    string returnType = this.DefineReturnType(element);
                    if (this.IsBasicReturnType(returnType))
                    {
                        paramaters.Add(new ClassParameter{Type=returnType,Name = element.Name.LocalName});
                    }
                    else
                    {
                        paramaters.Add(new ClassParameter{Type=returnType,Name = element.Name.LocalName, ChildProperties=this.Build(element.Nodes())});
                    }
                } else {
                    if (element.Name.LocalName =="value-of"){
                    } else if (element.Name.LocalName =="if"){
                        return this.Build(element.Nodes());
              
                    }
                    else if (element.Name.LocalName =="choose")
                    {
                        return this.Build(element.Nodes());
                    }
                    else if (element.Name.LocalName =="when")
                    {
                        return this.Build(element.Nodes());
                    }
                    else if (element.Name.LocalName =="otherwise")
                    {
                        return this.Build(element.Nodes());
                    }
                    else if (element.Name.LocalName =="for-each")
                    {
                        return this.Build(element.Nodes());
                    }
                }

            }
            return paramaters;

        }

        public bool IsBasicReturnType(string returnType)
        {
            switch (returnType)
            {
                case "string":
                    return true;
                case "double":
                    return true;
                case "bool":
                    return true;
                case "DateTime":
                    return true;
                default:
                    return false;
            }

        }

        public string DefineReturnType(XElement inputedElement)
        {
            var elementTypes = new List<string>();
            var nodes = new List<XNode>();
            nodes.Add(inputedElement);
            this.RetrieveAllTypeInTheElement(nodes, elementTypes);
            if (IsBasicReturnType(elementTypes[1]))
            {
                return elementTypes[1];
            }
            return elementTypes[0];
        }


        private void RetrieveAllTypeInTheElement(IEnumerable<XNode> inputedElement, List<string> elementTypes)
        {
            foreach (XElement item in inputedElement)
            {
                if (item.Name.NamespaceName != this.xslNameSpace)
                {
                    elementTypes.Add(item.Name.ToString());
                }
                else if (item.Name.LocalName =="value-of")
                {
                    if (item.Attribute("select").Value.Contains("tib:parse-date"))
                    {
                        elementTypes.Add("DateTime");

                    } else if (item.Attribute("select").Value.StartsWith("number(")){
                        elementTypes.Add("double");
                    }
                    else{
                        elementTypes.Add("string");
                    }

                }

                if (item.HasElements)
                {
                    this.RetrieveAllTypeInTheElement(item.Nodes(), elementTypes);
                }

            }
        }

        string GenerateCode(List<string> codeStatement)
        {
            var generatedCode = new StringBuilder();
            foreach (var item in codeStatement)
            {
                generatedCode.Append(item);
            }
            return generatedCode.ToString();
        }

    }
}

