using System;
using NUnit.Framework;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;
using EaiConverter.Mapper;
using System.Collections.Generic;
using System.Text;
using EaiConverter.Test.Utils;

namespace EaiConverterTest
{
	[TestFixture]
	public class XslBuilderTest
	{
        XslBuilder xslBuilder;
   

        [SetUp]
        public void SetUp ()
        {
            xslBuilder = new XslBuilder ();

        }

        [Test]
        public void Should_Return_1_Variable_assignement_with_String_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = xslBuilder.Build (doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual (@"string FundName = ""testvalue"";

", generateCode);
        }

        [Test]
        [Ignore]
        public void Should_Return_1_Variable_assignement_with_double_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundName>
            <xsl:value-of select=""number(testvalue)""/>
        </FundName>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = xslBuilder.Build (doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual (@"double FundName = double.Parse(""testvalue"");

", generateCode);
        }

        [Test]
        [Ignore]
        public void Should_Return_1_Variable_assignement_with_DateTime_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundName>
            <xsl:value-of select=""tib:parse-dateTime('MMM dd yyyy', $Mystuff)""/>
        </FundName>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = xslBuilder.Build (doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual (@"DateTime FundName = DateTime.ParseExact($Mystuff, ""MMM dd yyyy"", null);
", generateCode);

        }

        [Test]
        public void Should_Return_2_Variable_assignement_with_string_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build (doc.Nodes()));

            Assert.AreEqual (@"string FundName = ""testvalue"";
string AdminID = ""EVL"";

", generateCode);
        }


        [Test]
        public void Should_Return_1_Variable_assignement_with_complex_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<sqlParams>        
<FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>
</sqlParams>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build (doc.Nodes()));

            Assert.AreEqual (@"sqlParams sqlParams = new sqlParams();
sqlParams.FundName = ""testvalue"";
sqlParams.AdminID = ""EVL"";

", generateCode.ToString());
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

