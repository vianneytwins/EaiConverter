namespace EaiConverter.Test.Parser
{
    using System.Xml;
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser;

    using NUnit.Framework;

    [TestFixture]
    public class GenerateErrorActivityParserTest
    {
        private GenerateErrorActivityParser activityParser;
        private XElement doc;

        private GenerateErrorActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.activityParser = new GenerateErrorActivityParser();
            var xml =
                @"<pd:activity name=""ERROR activity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns=""http://www.tibco.com/pe/GenerateErrorActivity/InputSchema"">
        <pd:type>com.tibco.pe.core.GenerateErrorActivity</pd:type>
        <pd:resourceType>ae.activities.throw</pd:resourceType>
        <pd:x>624</pd:x>
        <pd:y>253</pd:y>
        <config>
            <faultName>Info</faultName>
        </config>
        <pd:inputBindings>
            <ns:ActivityInput>
              <message>
                 <xsl:value-of select=""'ReadState'""/>
              </message>
              <messageCode>
                    <xsl:value-of select=""'Not last tuesday : trade filtered'""/>
                </messageCode>
            </ns:ActivityInput>
        </pd:inputBindings>
    </pd:activity>";
            doc = XElement.Parse(xml);
            this.activity = (GenerateErrorActivity)this.activityParser.Parse(this.doc);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_GenerateLogActivity (){
            

            Assert.AreEqual (ActivityType.generateErrorActivity.ToString(), this.activity.Type.ToString());
        }


        [Test]
        public void Should_Return_FaultName()
        {    
            Assert.AreEqual("Info", this.activity.FaultName);
        }

        [Test]
        public void Should_Return_Paramter()
        {
            Assert.AreEqual("message", this.activity.Parameters[0].Name);
        }
    
    }
}

