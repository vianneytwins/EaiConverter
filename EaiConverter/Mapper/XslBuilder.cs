using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.CodeDom;

namespace EaiConverter.Mapper
{
    public class XslBuilder
    {
        XNamespace xslNameSpace = "http://w3.org/1999/XSL/Transform";

        public List<string> Build (IEnumerable<XNode> inputNodes, string parent){

            var codeStatements = new List<string>();

            foreach(var inputNode in inputNodes)
            {
                var element = (XElement) inputNode;
                if (element.Name.NamespaceName != this.xslNameSpace) {
                    string returnType = this.DefineReturnType(element);
                    string variableReference = this.DefineVariableReference(element,parent);
                    if (this.IsBasicReturnType(returnType))
                    {
                        codeStatements.Add(variableReference + element.Name + " = ");
                        codeStatements.AddRange(this.Build(element.Nodes(),parent));
                    }
                    else
                    {
                        // intialise the variable first
                        codeStatements.Add(returnType + " " + element.Name + " = new " +returnType + "();\n") ;
                        codeStatements.AddRange(this.Build(element.Nodes(), element.Name.ToString()));
                    }

                } else {
                    codeStatements.Add(@""""+ element.Attribute("select").Value + @"""" + ";\n");
                }

            }
            return codeStatements;

        }

        public CodeStatementCollection Build (IEnumerable<XNode> inputNodes){
            var codeInStringList = this.Build(inputNodes, null);
            var codeSnippet = new CodeSnippetStatement (GenerateCode(codeInStringList));
            var codeStatements = new CodeStatementCollection();
            codeStatements.Add(codeSnippet);
            return codeStatements;
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

        string DefineVariableReference(XElement inputedElement, string parent)
        {
            var elementTypes = new List<string>();
            var nodes = new List<XNode>();
            nodes.Add(inputedElement);
            this.RetrieveAllTypeInTheElement(nodes, elementTypes);
            if (parent == null && elementTypes.Count == 2)
            {
                return elementTypes[1] + " ";
            } 
            return parent+ ".";
        }

        private void RetrieveAllTypeInTheElement(IEnumerable<XNode> inputedElement, List<string> elementTypes)
        {
            foreach (XElement item in inputedElement)
            {
                if (item.Name.NamespaceName != this.xslNameSpace)
                {
                    elementTypes.Add(item.Name.ToString());
                }
                else
                {
                    if (item.Attribute("select").Value.Contains("tib:parse-dateTime"))
                    {
                        elementTypes.Add("DateTime");
                    
                    } else if (item.Attribute("select").Value.StartsWith("number")){
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

