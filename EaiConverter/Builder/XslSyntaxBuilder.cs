using System.Xml.Linq;
using EaiConverter.Builder;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
	public class XslSyntaxBuilder
	{
		private IXpathBuilder xpathBuilder;
		//xsi:nil="true"
		private Tab tab = new Tab();

		public XslSyntaxBuilder(IXpathBuilder xpathBuilder)
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
			var newPackageName = XslBuilder.FormatCorrectlyPackageName(packageName);
			var xslElementInList = this.BuildSyntax(newPackageName, inputNodes);
			var codeStatements = new CodeStatementCollection();
			foreach (var element in xslElementInList)
			{
				codeStatements.Add(new CodeSnippetStatement(element.ToString()));
			}
			return codeStatements;
		}

		public List<XslSyntaxElement> BuildSyntax(string packageName, IEnumerable<XNode> inputNodes)
		{
			var xslSyntaxElements = new List<XslSyntaxElement> ();
            if (inputNodes == null)
            {
                return xslSyntaxElements;
            }

            foreach (var inputNode in inputNodes)
            {
                var element = (XElement)inputNode;
                var xslElement = new XslSyntaxElement
                {
                    Name = element.Name.LocalName,
                    ReturnType = XslBuilder.DefineReturnType(element),
                    PackageName = packageName !=null ? packageName : string.Empty,
                    Value = GetValue(element.Nodes())
                };
                
                xslSyntaxElements.Add(xslElement);
            }

            return xslSyntaxElements;
		}

        public string GetValue(IEnumerable<XNode> nodes)
        {
            throw new System.NotImplementedException();
        }
	}

}

