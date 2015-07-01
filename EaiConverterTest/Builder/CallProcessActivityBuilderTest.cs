using NUnit.Framework;
using EaiConverter.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using EaiConverter.Builder;
using EaiConverter.Test.Utils;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class CallProcessActivityBuilderTest
    {

        CallProcessActivityBuilder xmlParseActivityBuilder;
        CallProcessActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.xmlParseActivityBuilder = new CallProcessActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new CallProcessActivity ( "My Call Process Activity",ActivityType.callProcessActivityType);
            this.activity.ProcessName = "Process/DAI/PNO/process.To.Call.process";
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""'TestString'""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            this.activity.InputBindings = doc.Nodes();
            this.activity.Parameters = new List<ClassParameter>{
                new ClassParameter{
                    Name = "xmlString",
                    Type= "string"}
            };
        }

        [Test]
        public void Should_Return_InvocationCode()
        {
            var expected = @"this.logger.Info(""Start Activity: My Call Process Activity of type: com.tibco.pe.core.CallProcessActivity"");
string xmlString = ""TestString"";

var myCallProcessActivity = this.processToCall.Start(xmlString);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(xmlParseActivityBuilder.GenerateCodeInvocation(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

