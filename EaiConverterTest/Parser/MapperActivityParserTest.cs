using System;
using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter.Test.Parser
{
    [TestFixture]
    public class MapperActivityParserTest
    {
        MapperActivityParser mapperActivityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            mapperActivityParser = new MapperActivityParser ();
            var xml =
                @"<pd:activity name=""Mappe Equity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.mapper.MapperActivity</pd:type>
<config>
<element ref=""pfx2:NTMMessage""/>

</config>
<pd:inputBindings>
    <sqlParams>
        <FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>
    </sqlParams>
</pd:inputBindings>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_MapperActivity (){
            MapperActivity mapperActivity = (MapperActivity) mapperActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.mapper.MapperActivity", mapperActivity.Type.ToString());
        }


        [Test]
        public void Should_Return_XsdReference_in_Element_config(){
            MapperActivity mapperActivity = (MapperActivity) mapperActivityParser.Parse (doc);

            Assert.AreEqual ("pfx2:NTMMessage", mapperActivity.XsdReference);
        }
    
    }
}

