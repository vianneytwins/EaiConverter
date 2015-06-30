using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using EaiConverter.Test.Utils;
using System.Xml.Linq;
using System.Collections.Generic;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class GenerateErrorActivityBuilderTest
    {
        GenerateErrorActivityBuilder activityBuilder;
        GenerateErrorActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.activityBuilder = new GenerateErrorActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new GenerateErrorActivity( "My Activity Name",ActivityType.generateErrorActivity);
            this.activity.FaultName = "";
            var xml =
                @"
    <ns:ActivityInput xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns=""http://www.tibco.com/pe/GenerateErrorActivitySchema"">
        <message>
            <xsl:value-of select=""'testvalue'""/>
        </message>
        <messageCode>
            <xsl:value-of select=""'EVL'""/>
        </messageCode>
    </ns:ActivityInput>
";
            XElement doc = XElement.Parse(xml);
            this.activity.InputBindings = doc.Nodes();
            this.activity.Parameters = new List<ClassParameter>{
                new ClassParameter{
                    Name = "message",
                    Type= "String"},
                new ClassParameter{
                    Name = "messageCode",
                    Type= "String"}
            };
        }

        [Test]
        public void Should_Generate_invocation_method()
        {
            var expected = @"this.logger.Info(""Start Activity: My Activity Name of type: com.tibco.pe.core.GenerateErrorActivity"");
string message = ""testvalue"";
string messageCode = ""EVL"";

throw new System.Exception(String.Format(""Message : {0}\nMessage code : {1} "", message, messageCode));
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateCodeInvocation(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

