using System;
using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter
{
    [TestFixture]
    public class XmlParseActivityParserTest
    {
        XmlParseActivityParser xmlParseActivityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            xmlParseActivityParser = new XmlParseActivityParser ();
            var xml =
                @"<pd:activity name=""Parse Equity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"">
<pd:type>com.tibco.plugin.xml.XMLParseActivity</pd:type>
<config>
<inputStyle>text</inputStyle>
<term ref=""pfx4:EquityRecord""/>

</config>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_XmlParseActivity (){
            XmlParseActivity xmlParseActivity = xmlParseActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.xml.XMLParseActivity", xmlParseActivity.Type.ToString());
        }


        [Test]
        public void Should_Return_XsdReference_in_Term_config(){
            XmlParseActivity xmlParseActivity = xmlParseActivityParser.Parse (doc);

            Assert.AreEqual ("pfx4:EquityRecord", xmlParseActivity.XsdReference);
        }
    
    }
}

