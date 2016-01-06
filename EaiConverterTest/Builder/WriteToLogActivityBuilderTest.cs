using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using EaiConverter.Test.Utils;
using System.Xml.Linq;
using System.Collections.Generic;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class WriteToLogActivityBuilderTest
    {
        WriteToLogActivityBuilder activityBuilder;
        WriteToLogActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.activityBuilder = new WriteToLogActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new WriteToLogActivity( "My Activity Name",ActivityType.writeToLogActivityType);
            this.activity.Role = "Error";
            var xml =
                @"
    <ns:ActivityInput xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns=""http://www.tibco.com/pe/WriteToLogActivitySchema"">
        <message>
            <xsl:value-of select=""'testvalue'""/>
        </message>
        <msgCode>
            <xsl:value-of select=""'EVL'""/>
        </msgCode>
    </ns:ActivityInput>
";
            XElement doc = XElement.Parse(xml);
            this.activity.InputBindings = doc.Nodes();
            this.activity.Parameters = new List<ClassParameter>{
                new ClassParameter{
                    Name = "message",
					Type= "System.String"},
                new ClassParameter{
                    Name = "msgCode",
					Type= "System.String"}
            };
        }

        [Test]
        public void Should_Generate_invocation_method()
        {
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.WriteToLogActivity"");
System.String message;
message = ""testvalue"";
System.String msgCode;
msgCode = ""EVL"";

this.logger.Error(String.Format(""Message : {0}\nMessage code : {1} "", message, msgCode));
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateMethod(this.activity,null).Statements);
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

