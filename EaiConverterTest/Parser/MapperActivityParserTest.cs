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
		XsdParser xsdParser;

        [SetUp]
        public void SetUp ()
        {
			this.xsdParser = new XsdParser ();
			mapperActivityParser = new MapperActivityParser (xsdParser);
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

		[Test]
		public void Should_Return_ObjectXNodes_in_Element_config(){
			var xml =
				@"<pd:activity name=""Mappe Equity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<pd:type>com.tibco.plugin.mapper.MapperActivity</pd:type>
<config>
<element>
 <xsd:element name=""adminID"" type=""xsd:string"" />
</element>
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
			var docz = XElement.Parse(xml);

			MapperActivity mapperActivity = (MapperActivity) mapperActivityParser.Parse (docz);

			Assert.IsTrue( mapperActivity.ObjectXNodes != null);
		}
    
    }
}

