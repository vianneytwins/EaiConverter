namespace EaiConverter.Test.Builder
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class GenerateErrorActivityBuilderTest
    {
        private GenerateErrorActivityBuilder activityBuilder;
        private GenerateErrorActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.activityBuilder = new GenerateErrorActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new GenerateErrorActivity( "My", ActivityType.generateErrorActivity);
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
					Type= "System.String"},
                new ClassParameter{
                    Name = "messageCode",
					Type= "System.String"}
            };
        }

        [Test]
        public void Should_Generate_invocation_method()
        {
            var expected = @"this.logger.Info(""Start Activity: My of type: com.tibco.pe.core.GenerateErrorActivity"");
System.String message;
message = ""testvalue"";
System.String messageCode;
messageCode = ""EVL"";

throw new System.Exception(String.Format(""Message : {0}\nMessage code : {1} "", message, messageCode));
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateInvocationCode(this.activity, new Dictionary<string, string>()));
            Assert.AreEqual(expected, generatedCode);
        }
    }
}

