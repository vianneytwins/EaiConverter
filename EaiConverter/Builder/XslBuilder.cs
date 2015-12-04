using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Parser.Utils;

    public class XslBuilder
    {

        private IXpathBuilder xpathBuilder;
        //xsi:nil="true"
        private Tab tab = new Tab();

        public XslBuilder(IXpathBuilder xpathBuilder)
        {
            this.xpathBuilder = xpathBuilder;
        }

        public CodeStatementCollection Build(IEnumerable<XNode> inputNodes)
        {
            return this.Build(string.Empty, inputNodes);
        }

        public CodeStatementCollection Build(string packageName, IEnumerable<XNode> inputNodes)
        {
            tab = new Tab();
            var newPackageName = FormatCorrectlyPackageName(packageName);
            var codeInStringList = this.Build(newPackageName, inputNodes, null);
            
            string codeinString = codeInStringList.ToString();
            // TODO : remove this ugly fix !!!
            codeinString = codeinString.Replace("NTMMessage.NTMTrade.", "((NTMTrade)NTMMessage.Items[0]).");
            codeinString = codeinString.Replace("NTMMessage.NTMTrades", "NTMMessage.Items");
            codeinString = codeinString.Replace("NTMMessage.NTMTrade", "NTMMessage.Items[0]");

            var codeSnippet = new CodeSnippetStatement(codeinString);
            var codeStatements = new CodeStatementCollection();
            codeStatements.Add(codeSnippet);
            return codeStatements;
        }

        private StringBuilder Build(IEnumerable<XNode> inputNodes, string parent)
        {
            return Build(string.Empty, inputNodes, parent);
        }

        private StringBuilder Build(string packageName, IEnumerable<XNode> inputNodes, string parent)
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
                    string returnType = DefineReturnType(element);
                    if (IsBasicReturnType(returnType))
                    {
                        packageName = string.Empty;
                    }
                    string variableReference = this.DefineVariableReference(element, parent);
                    isAlistElement = this.IsAListElement(element, inputNodes);
                    var hasTheListBeenInitialised = false;

                    if (isAlistElement)
                    {
                        hasTheListBeenInitialised = listElements.ContainsKey(element.Name.LocalName);
                        if (!hasTheListBeenInitialised)
                        {
                            if (string.IsNullOrEmpty(parent))
                            {
                                codeStatements.Append(this.tab + "List<" + returnType + "> ");
                            }

                            codeStatements.Append(variableReference + " = new List<" + returnType + ">();\n");
                            listElements.Add(element.Name.LocalName, true);
                            if (!IsBasicReturnType(returnType))
                            {
                                codeStatements.Append(
                                    returnType + " temp" + element.Name.LocalName + " = new " + returnType + "();\n");
                            }
                            else
                            {
                                codeStatements.Append(
                                    returnType + " temp" + element.Name.LocalName + ";\n");
                            }
                        }

                        codeStatements.Append(this.Build(element.Nodes(), "temp" + element.Name.LocalName));
                    }
                    else if (returnType == null)
                    {
                        //TODO ugly thing need to find a way to get the real type
                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Append("object ");
                        }

                        codeStatements.Append(variableReference + " = null;\n");
                    }
                    else
                    {
                        // intialise the variable first
                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Append(this.tab + packageName + returnType + " ");
                            if (!IsBasicReturnType(returnType))
                            {
                                codeStatements.Append(
                                    variableReference + " = new " + packageName + returnType + "();\n");
                            }
                            else
                            {
                                codeStatements.Append(variableReference + ";\n");
                            }
                        }
                        else
                        {
                            if (!IsBasicReturnType(returnType))
                            {
                                codeStatements.Append(
                                    variableReference + " = new " + packageName + returnType + "();\n");
                            }
                        }

                        if (string.IsNullOrEmpty(parent))
                        {
                            codeStatements.Append(this.tab);
                            codeStatements.Append(this.Build(element.Nodes(), element.Name.LocalName));
                        }
                        else
                        {
                            codeStatements.Append(this.tab);
                            codeStatements.Append(this.Build(element.Nodes(), parent + "." + element.Name.LocalName));
                        }
                    }
                    if (isAlistElement)
                    {
                        //recursive call to get the value
                        //codeStatements.Append(variableReference + ".Add(" + this.Build(element.Nodes(), parent) + ");\n");
                        codeStatements.Append(variableReference + ".Add(temp" + element.Name.LocalName + ");\n");
                    }

                }
                else
                {
                    if (element.Name.LocalName == "value-of")
                    {
                        codeStatements.Append(this.ReturnValue(element, parent));
                    }
                    else if (element.Name.LocalName == "copy-of")
                    {
                        codeStatements.Append(this.ReturnValue(element, parent));
                    }
                    else if (element.Name.LocalName == "attribute")
                    {
                        codeStatements.Append(this.BuildAttribute(element, parent));
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

        private string BuildAttribute(XElement element, string parent)
        {
            //return this.xpathBuilder.Build(element.Attribute("select").Value);
            string elementName = element.Attribute("name").Value;
            string lastElementName = this.GetLastElement(parent);
            var assignationString = parent + "." + lastElementName + VariableHelper.ToClassName(elementName);

            if (elementName != "xsi:nil")
            {
                return assignationString + this.Build(element.Nodes(), null);
            }

            return parent + " = " + "null;\n";
        }

        public string GetLastElement(string parent)
        {
            if (!parent.Contains("."))
            {
                return parent;
            }
            return parent.Substring(parent.LastIndexOf(".") +1 , parent.Length - (parent.LastIndexOf(".") + 1));

        }

        public StringBuilder ManageIterationTag(XElement element, string parent)
        {
            var codeStatements = new StringBuilder();
            var returnType = DefineReturnType(element);
            var variableReference = this.DefineVariableReference((XElement)element.FirstNode, null);
            var variableListReference = this.DefineVariableReference((XElement)element.FirstNode, parent) + "s";
            codeStatements.Append(this.tab + variableListReference + " = new List<" + returnType + ">();\n");
            codeStatements.Append(this.tab + "foreach (var item in " + this.ReturnForEachValue(element) + ")\n{\n");
            this.tab.Increment();
            codeStatements.Append(this.Build(element.Nodes(), null));
            codeStatements.Append(this.tab + variableListReference + ".Add(" + variableReference + ");\n");
            codeStatements.Append(this.tab.Decrement() + "}\n");
            return codeStatements;
        }

        public StringBuilder ManageConditionTag(XElement element, string parent, bool isIfCondition)
        {
            var codeStatements = new StringBuilder();
            var test = isIfCondition ? "if (" + this.ReturnCondition(element) + ")\n{\n" : "else\n{\n";
            codeStatements.Append(test);
            this.tab.Increment();
            codeStatements.Append(this.Build(element.Nodes(), parent));
            codeStatements.Append(this.tab.Decrement() + "}\n");
            return codeStatements;
        }

        public string ReturnValue(XElement element, string parent)
        {
            return parent + " = " + this.xpathBuilder.Build(element.Attribute("select").Value) + ";\n";
        }

        public string ReturnForEachValue(XElement element)
        {
            return this.xpathBuilder.Build(element.Attribute("select").Value);
        }

        public string ReturnCondition(XElement element)
        {
            return this.xpathBuilder.Build(element.Attribute("test").Value);
        }

        public static bool IsBasicReturnType(string returnType)
        {
            switch (returnType)
            {
                case CSharpTypeConstant.SystemString:
                    return true;
                case "Double":
                    return true;
                case "double":
                    return true;
                case "Int":
                    return true;
                case "Int32":
                    return true;
                case "bool":
                    return true;
                case "string":
                    return true;
                case "DateTime":
                    return true;
                default:
                    return false;
            }
        }

        public static string DefineReturnType(XElement inputedElement)
        {
            if (inputedElement.Attribute(XmlnsConstant.xsiNameSpace + "nil") != null && inputedElement.Attribute(XmlnsConstant.xsiNameSpace + "nil").Value == "true")
            {
                return null;
            }

            var elementTypes = new List<string>();
            var nodes = new List<XNode> { inputedElement };
            RetrieveAllTypeInTheElement(nodes, elementTypes);
            if (elementTypes.Count > 1 && IsBasicReturnType(elementTypes[1]))
            //if (elementTypes.Count == 2)
            {
                return ConvertToSafeType(elementTypes[1]);
            }

            return ConvertToSafeType(elementTypes[0]);
        }

        private static string ConvertToSafeType(string elementType)
        {
            if (elementType == "param")
            {
                return "@param";
            }

            return elementType;
        }

        public string DefineVariableReference(XElement inputedElement, string parent)
        {
            var elementTypes = new List<string>();
            var nodes = new List<XNode> { inputedElement };
            RetrieveAllTypeInTheElement(nodes, elementTypes);
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

        private static void RetrieveAllTypeInTheElement(IEnumerable<XNode> inputedElement, List<string> elementTypes)
        {
            int number = 0;
            foreach (XElement item in inputedElement)
            {
                if (!Regex.IsMatch(item.Name.NamespaceName, XmlnsConstant.xslNameSpace))
                {
                    elementTypes.Add(item.Name.LocalName);
                }
                else if (item.Name.LocalName == "value-of")
                {
                    if (item.Attribute("select").Value.Contains("tib:parse-date"))
                    {
                        elementTypes.Add("DateTime");
                    }
                    else if (item.Attribute("select").Value.StartsWith("number("))
                    {
                        elementTypes.Add("Double");
                    }
                    else if(Int32.TryParse(item.Attribute("select").Value,out number))
                    {
                        elementTypes.Add("Int32");
                    }    
                    else
                    {
                        elementTypes.Add(CSharpTypeConstant.SystemString);
                    }
                }
                else if (item.Name.LocalName == "variable")
                {
                    elementTypes.Add(item.Attribute("name").Value);
                }
                else if (item.Name.LocalName == "attribute")
                {
                    elementTypes.Add(item.Attribute("name").Value);
                }

                if (item.HasElements)
                {
                    RetrieveAllTypeInTheElement(item.Nodes(), elementTypes);
                }
            }
        }

        public static string FormatCorrectlyPackageName(string packageName)
        {
            if (string.IsNullOrEmpty(packageName) || IsBasicReturnType(packageName.Remove(packageName.Length-1, 1)) || IsBasicReturnType(packageName))
            {
                return string.Empty;
            }

            if (packageName.EndsWith("."))
            {
                return packageName;
            }

            return packageName + ".";
        }
    }
}

