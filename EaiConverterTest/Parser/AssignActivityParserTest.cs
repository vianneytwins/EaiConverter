using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter.Test.Parser
{
    [TestFixture]
    public class AssignActivityParserTest
    {
        AssignActivityParser assignActivityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            assignActivityParser = new AssignActivityParser ();
            var xml =
                @"<pd:activity name=""Assign Strat"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.pe.core.AssignActivity</pd:type>
<config>
    <variableName>var</variableName>
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
        public void Should_Return_Activity_Type_Is_AssignActivity (){
            var assignActivity = assignActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.pe.core.AssignActivity", assignActivity.Type.ToString());
        }


        [Test]
        public void Should_Return_VariableName_in_config(){
            var assignActivity = assignActivityParser.Parse (doc);

            Assert.AreEqual ("var", ((AssignActivity) assignActivity).VariableName);
        }
    
    }
}

