using System;
using NUnit.Framework;
using EaiConverter.Parser;
using System.Xml.Linq;
using EaiConverter.Utils;

namespace EaiConverter.Test.Parser
{
	[TestFixture]
	public class XslParserTest
	{
		private XslParser xslParser;

		[SetUp]
		public void SetUp()
		{
			this.xslParser = new XslParser();
		}

		[Test]
		public void Should_Return_1_Class_Parameter_of_type_string_and_named_param()
		{
			var xml =
				@"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
        
        <param>
            <xsl:value-of select=""'testvalue1'""/>
        </param>

</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

			var classParameters = this.xslParser.Parse(doc.Nodes());
			Assert.AreEqual ("param", classParameters [0].Name);
			Assert.AreEqual (CSharpTypeConstant.SystemString, classParameters [0].Type);

		}

		[Test]
		public void Should_Return_Class_Parameter_with_full_type_and_named_param()
		{
			var xml =
				@"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx=""http://www.mytest"">
        <pfx:MyType>
	        <param>
	            <xsl:value-of select=""'testvalue1'""/>
	        </param>
		</pfx:MyType>
</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

			var classParameters = this.xslParser.Parse(doc.Nodes());
			Assert.AreEqual ("MyType", classParameters [0].Name);
			Assert.AreEqual ("MyType", classParameters [0].Type);

		}
	}
}

