namespace EaiConverter.Test.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser;

    using NUnit.Framework;

    [TestFixture]
    public class EngineCommandActivityParserTest
    {
        EngineCommandActivityParser activityParser;
        XElement doc;

        [SetUp]
        public void SetUp()
        {
            this.activityParser = new EngineCommandActivityParser();
            var xml =
                @"<pd:activity name=""null activity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx4=""com/tibco/pe/commands"">
        <pd:type>com.tibco.pe.core.EngineCommandActivity</pd:type>
        <pd:resourceType>ae.activities.enginecommand</pd:resourceType>
        <config>
            <command>GetProcessInstanceInfo</command>
        </config>
        <pd:inputBindings>
            <pfx4:input/>
        </pd:inputBindings>

</pd:activity>";
            this.doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_NullActivity ()
        {
            var activity = this.activityParser.Parse(this.doc);

            Assert.AreEqual("com.tibco.pe.core.EngineCommandActivity", activity.Type.ToString());
        }

        [Test]
        public void Should_Return_Command()
        {
            var activity = (EngineCommandActivity)this.activityParser.Parse(this.doc);

            Assert.AreEqual("GetProcessInstanceInfo", activity.Command);
        }

    }
}

