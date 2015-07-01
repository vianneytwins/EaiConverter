﻿using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter.Test.Parser
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
        public void Should_Return_One_Parameter_Named_SqlParam(){
            XmlParseActivity xmlParseActivity = (XmlParseActivity) xmlParseActivityParser.Parse (doc);

            Assert.AreEqual ("sqlParams", xmlParseActivity.Parameters[0].Name);
        }

        [Test]
        public void Should_Return_One_Parameter_of_type_string(){
            XmlParseActivity xmlParseActivity = (XmlParseActivity) xmlParseActivityParser.Parse (doc);

            Assert.AreEqual ("string", xmlParseActivity.Parameters[0].Type);
        }
    }
}

