using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.CodeDom;
using EaiConverter.Parser.Utils;
using System.Linq;
using System;

namespace EaiConverter.Builder
{
    using System.Text.RegularExpressions;

    public class XslBuilder
    {

        private IXpathBuilder xpathBuilder;
        //xsi:nil="true"

        public XslBuilder(IXpathBuilder xpathBuilder)
        {
            this.xpathBuilder = xpathBuilder;
        }

        public CodeStatementCollection Build(IEnumerable<XNode> inputNodes)
        {
            var codeInStringList = this.Build(inputNodes, null);
            var codeSnippet = new CodeSnippetStatement(codeInStringList.ToString());
            var codeStatements = new CodeStatementCollection();
            codeStatements.Add(codeSnippet);
            return codeStatements;
        }

        private StringBuilder Build(IEnumerable<XNode> inputNodes, string parent)
        {

            var codeStatements = new StringBuilder();
            if (inputNodes == null)
            {
                return codeStatements;
            }
            bool isAlistElement = false;

            var listElements = new Dictionary<string, bool>();

            foreach (var inputNode in inputNodes)
            {
                var element = (XElement)inputNode;
                if (!Regex.IsMatch(element.Name.NamespaceName, XmlnsConstant.xslNameSpace))
                {
                    string returnType = this.DefineReturnType(element);
                    string variableReference = this.DefineVariableReference(element, parent);
                    isAlistElement = this.IsAListElement(element, inputNodes);
                    var hasTheListBeenInitialised = false;

                    if (isAlistElement)
                    {
                        hasTheListBeenInitialised = listElements.ContainsKey(element.Name.ToString());
                        if (!hasTheListBeenInitialised)
                        {
                            if (string.IsNullOrEmpty(parent))
                            {
                                codeStatements.Append("List<" + returnType + "> ");
                            }

                            codeStatements.Append(variableReference + " = new List<" + returnType + ">();\n");
                            listElements.Add(element.Name.ToString(), true);
                        }
                        //recursive call to get the value
                        codeStatements.Append(variableReference + ".Add(" + this.Build(element.Nodes(), parent) + ");\n");
                    }
                    else if (returnType == null)
                    {
                        //TODO ugly thing need to find a way to get the real type
                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Append("object ");
                        }
                        codeStatements.Append(variableReference + " = null;");
                    }
                    else if (this.IsBasicReturnType(returnType))
                    {
                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Append(returnType + " ");
                        }
                        codeStatements.Append(variableReference + " = ");

                        //recursive call to get the value
                        codeStatements.Append(this.Build(element.Nodes(), parent) + ";\n");
                    }
                    else
                    {
                        // intialise the variable first
                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Append(returnType + " ");
                        }

                        codeStatements.Append(variableReference + " = new " + returnType + "();\n");


                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Append(this.Build(element.Nodes(), element.Name.ToString()));
                        }
                        else
                        {
                            codeStatements.Append(this.Build(element.Nodes(), parent + "." + element.Name.ToString()));
                        }
                    }
                }
                else
                {
                    if (element.Name.LocalName == "value-of")
                    {
                        codeStatements.Append(this.ReturnValue(element));
                    }
                    else if (element.Name.LocalName == "copy-of")
                    {
                        codeStatements.Append(this.ReturnValue(element));
                    }
                    else if (element.Name.LocalName == "if")
                    {
                        codeStatements.Append(this.ManageConditionTag(element, parent, true));
                    }
                    else if (element.Name.LocalName == "choose")
                    {
                        codeStatements.Append(this.Build(element.Nodes(), parent));
                    }
                    else if (element.Name.LocalName == "when")
                    {
                        codeStatements.Append(this.ManageConditionTag(element, parent, true));
                    }
                    else if (element.Name.LocalName == "otherwise")
                    {
                        codeStatements.Append(this.ManageConditionTag(element, parent, false));
                    }
                    else if (element.Name.LocalName == "for-each")
                    {
                        codeStatements.Append(this.ManageIterationTag(element, parent));
                    }
                }
            }

            return codeStatements;
        }

        StringBuilder ManageIterationTag(XElement element, string parent)
        {
            var codeStatements = new StringBuilder();
            string returnType = this.DefineReturnType(element);
            string variableReference = this.DefineVariableReference((XElement)element.FirstNode, null);
            string variableListReference = this.DefineVariableReference((XElement)element.FirstNode, parent) + "s";
            codeStatements.Append(variableListReference + " = new List<" + returnType + ">();\n");
            codeStatements.Append("foreach (var item in " + this.ReturnValue(element) + "){\n");
            codeStatements.Append(this.Build(element.Nodes(), null));
            codeStatements.Append(variableListReference + ".Add(" + variableReference + ");\n");
            codeStatements.Append("}\n");
            return codeStatements;
        }

        StringBuilder ManageConditionTag(XElement element, string parent, bool isIfCondition)
        {
            var codeStatements = new StringBuilder();
            var test = isIfCondition ? "if (" + this.ReturnCondition(element) + "){\n" : "else{\n";
            codeStatements.Append(test);
            codeStatements.Append(this.Build(element.Nodes(), parent));
            codeStatements.Append("}\n");
            return codeStatements;
        }

        public string ReturnValue(XElement element)
        {
            return this.xpathBuilder.Build(element.Attribute("select").Value);
        }

        public string ReturnCondition(XElement element)
        {
            return this.xpathBuilder.Build(element.Attribute("test").Value);
        }

        public bool IsBasicReturnType(string returnType)
        {
            switch (returnType)
            {
                case "string":
                    return true;
                case "double":
                    return true;
                case "int":
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
            if (inputedElement.Attribute(XmlnsConstant.xsiNameSpace + "nil") != null && inputedElement.Attribute(XmlnsConstant.xsiNameSpace + "nil").Value == "true")
            {
                return null;
            }

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
            return parent + "." + elementTypes[0];
        }

        public bool IsAListElement(XElement inputElement, IEnumerable<XNode> inputNodes)
        {
            int count = (from a in inputNodes
                         where ((XElement)a).Name == inputElement.Name
                         select a).Count();

            if (count > 1)
            {
                return true;
            }
            return false;
        }

        private void RetrieveAllTypeInTheElement(IEnumerable<XNode> inputedElement, List<string> elementTypes)
        {
            int number = 0;
            foreach (XElement item in inputedElement)
            {
                if (!Regex.IsMatch(item.Name.NamespaceName, XmlnsConstant.xslNameSpace))
                {
                    elementTypes.Add(item.Name.ToString());
                }
                else if (item.Name.LocalName == "value-of")
                {
                    if (item.Attribute("select").Value.Contains("tib:parse-date"))
                    {
                        elementTypes.Add("DateTime");
                    }
                    else if (item.Attribute("select").Value.StartsWith("number("))
                    {
                        elementTypes.Add("double");
                    }
                    else if(Int32.TryParse(item.Attribute("select").Value,out number))
                    {
                        elementTypes.Add("int");
                    }    
                    else
                    {
                        elementTypes.Add("string");
                    }
                }

                if (item.HasElements)
                {
                    this.RetrieveAllTypeInTheElement(item.Nodes(), elementTypes);
                }
            }
        }
    }
}

