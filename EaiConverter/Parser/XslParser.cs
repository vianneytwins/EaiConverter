using EaiConverter.Utils;
using EaiConverter.Builder;

namespace EaiConverter.Parser
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class XslParser
    {
         public List<ClassParameter> Parse(IEnumerable<XNode> inputNodes)
         {
            var paramaters = new List<ClassParameter>();
            if (inputNodes == null)
            {
                return paramaters;
            }

            foreach (var inputNode in inputNodes)
            {
                var element = (XElement) inputNode;
                if (!Regex.IsMatch(element.Name.NamespaceName, XmlnsConstant.xslNameSpace))
                {
                    string returnType = XslBuilder.DefineReturnType(element);
                    if (XslBuilder.IsBasicReturnType(returnType))
                    {
                        paramaters.Add(new ClassParameter{Type=returnType,Name = element.Name.LocalName});
                    }
                    else
                    {
                        paramaters.Add(new ClassParameter{Type=returnType,Name = element.Name.LocalName, ChildProperties=this.Parse(element.Nodes())});
                    }
                }
                else
                {
                    if (element.Name.LocalName =="value-of")
                    {
                    }
                    else if (element.Name.LocalName =="if")
                    {
                        return this.Parse(element.Nodes());
                    }
                    else if (element.Name.LocalName =="choose")
                    {
                        return this.Parse(element.Nodes());
                    }
                    else if (element.Name.LocalName =="when")
                    {
                        return this.Parse(element.Nodes());
                    }
                    else if (element.Name.LocalName =="otherwise")
                    {
                        return this.Parse(element.Nodes());
                    }
                    else if (element.Name.LocalName =="for-each")
                    {
                        return this.Parse(element.Nodes());
                    }
                }

            }
            return paramaters;

        }



          

    }
}

