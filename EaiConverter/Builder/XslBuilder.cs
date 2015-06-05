using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.CodeDom;

namespace EaiConverter.Builder
{
    public class XslBuilder
    {

        private IXpathBuilder xpathBuilder;
        XNamespace xslNameSpace = "http://w3.org/1999/XSL/Transform";

        public XslBuilder (IXpathBuilder xpathBuilder){
            this.xpathBuilder = xpathBuilder;
        }

        public List<string> Build (IEnumerable<XNode> inputNodes, string parent){

            var codeStatements = new List<string>();
            if (inputNodes == null)
            {
                return codeStatements;
            }
            foreach(var inputNode in inputNodes)
            {
                var element = (XElement) inputNode;
                if (element.Name.NamespaceName != this.xslNameSpace) {
                    string returnType = this.DefineReturnType(element);
                    string variableReference = this.DefineVariableReference(element, parent);
                    if (this.IsBasicReturnType(returnType))
                    {
                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Add(returnType+" ");
                        }
                        codeStatements.Add(variableReference + " = ");

                        //recursive call to get the value
                        codeStatements.AddRange(this.Build(element.Nodes(),parent));
                    }
                    else
                    {
                        // intialise the variable first
                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Add(returnType + " ");
                        }

                        codeStatements.Add(variableReference + " = new " +returnType + "();\n") ;


                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.AddRange(this.Build(element.Nodes(), element.Name.ToString()));
                        }
                        else
                        {
                            codeStatements.AddRange(this.Build(element.Nodes(), parent +"."+ element.Name.ToString()));
                        }

                    }

                } else {
                    if (element.Name.LocalName =="value-of"){
                        codeStatements.Add( this.ReturnValue(element) + ";\n");
                    } else if (element.Name.LocalName =="if"){
                        codeStatements.Add("if (" + ReturnCondition(element) + "){\n");
                        codeStatements.AddRange(this.Build(element.Nodes(), parent));
                        codeStatements.Add("}\n");
                    }
                    else if (element.Name.LocalName =="choose")
                    {
                        codeStatements.AddRange(this.Build(element.Nodes(), parent));
                    }
                    else if (element.Name.LocalName =="when")
                    {
                        codeStatements.Add("if (" + ReturnCondition(element) + "){\n");
                        codeStatements.AddRange(this.Build(element.Nodes(), parent));
                        codeStatements.Add("}\n");
                    }
                    else if (element.Name.LocalName =="otherwise")
                    {
                        codeStatements.Add("else{\n");
                        codeStatements.AddRange(this.Build(element.Nodes(), parent));
                        codeStatements.Add("}\n");
                    }
                    else if (element.Name.LocalName =="for-each")
                    {
                        string returnType = this.DefineReturnType(element);
                        string variableReference = this.DefineVariableReference((XElement)element.FirstNode, null);
                        string variableListReference = this.DefineVariableReference((XElement)element.FirstNode, parent) +"s";
                        codeStatements.Add(variableListReference + " = new List<" + returnType + ">();\n");
                        codeStatements.Add("foreach (var item in "+ this.ReturnValue(element) + "){\n");
                        codeStatements.AddRange(this.Build(element.Nodes(), null));
                        codeStatements.Add(variableListReference+".Add("+variableReference+");\n");
                        codeStatements.Add("}\n");
                    }
                }

            }
            return codeStatements;

        }

        public string ReturnValue(XElement element)
        {
            return this.xpathBuilder.Build(element.Attribute("select").Value);
        }

        public string ReturnCondition (XElement element)
        {
            return this.xpathBuilder.Build(element.Attribute("test").Value);
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

        public string DefineVariableReference(XElement inputedElement, string parent)
        {
            var elementTypes = new List<string>();
            var nodes = new List<XNode>();
            nodes.Add(inputedElement);
            this.RetrieveAllTypeInTheElement(nodes, elementTypes);
            if (parent == null)
            {
                return elementTypes[0];
            }
            return parent+ "." + elementTypes[0];
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

