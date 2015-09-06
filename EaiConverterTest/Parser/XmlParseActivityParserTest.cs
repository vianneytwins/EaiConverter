using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;
using EaiConverter.Utils;

namespace EaiConverter.Test.Parser
{
    [TestFixture]
    public class XmlParseActivityParserTest
    {
        XmlParseActivityParser xmlParseActivityParser;
        XElement doc;
		XsdParser xsdParser;

        [SetUp]
        public void SetUp ()
        {
			this.xsdParser = new XsdParser ();
			xmlParseActivityParser = new XmlParseActivityParser (xsdParser);
            var xml =
                @"<pd:activity name=""Parse Equity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.xml.XMLParseActivity</pd:type>
<config>
    <inputStyle>text</inputStyle>
    <term ref=""pfx4:EquityRecord""/>
</config>
<pd:inputBindings>
    <sqlParams>
        <xsl:value-of select=""testvalue""/>
    </sqlParams>
</pd:inputBindings>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_XmlParseActivity (){
            XmlParseActivity xmlParseActivity = (XmlParseActivity) xmlParseActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.xml.XMLParseActivity", xmlParseActivity.Type.ToString());
        }


        [Test]
        public void Should_Return_XsdReference_in_Term_config(){
            XmlParseActivity xmlParseActivity = (XmlParseActivity) xmlParseActivityParser.Parse (doc);

            Assert.AreEqual ("pfx4:EquityRecord", xmlParseActivity.XsdReference);
        }
    
		[Test]
		public void Should_Return_ObjectXNodes_in_Term_config(){
			var xml =
				@"<pd:activity name=""Parse Equity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<pd:type>com.tibco.plugin.xml.XMLParseActivity</pd:type>
<config>
    <inputStyle>text</inputStyle>
    <term>
<xsd:element name=""group"" ><xsd:complexType><xsd:sequence><xsd:element name=""adminID"" type=""xsd:string"" /></xsd:sequence></xsd:complexType></xsd:element>
</term>
</config>
<pd:inputBindings>
    <sqlParams>
        <xsl:value-of select=""testvalue""/>
    </sqlParams>
</pd:inputBindings>
</pd:activity>";
			var docz = XElement.Parse(xml);

			XmlParseActivity xmlParseActivity = (XmlParseActivity) xmlParseActivityParser.Parse (docz);

			Assert.IsTrue (xmlParseActivity.ObjectXNodes != null);
	}

        [Test]
        public void Should_Return_One_Parameter_Named_SqlParam(){
            XmlParseActivity xmlParseActivity = (XmlParseActivity) xmlParseActivityParser.Parse (doc);

            Assert.AreEqual ("sqlParams", xmlParseActivity.Parameters[0].Name);
        }

        [Test]
        public void Should_Return_One_Parameter_of_type_string(){
            XmlParseActivity xmlParseActivity = (XmlParseActivity) xmlParseActivityParser.Parse (doc);

			Assert.AreEqual (CSharpTypeConstant.SystemString, xmlParseActivity.Parameters[0].Type);
        }
    }
}

