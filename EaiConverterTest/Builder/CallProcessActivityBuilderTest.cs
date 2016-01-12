namespace EaiConverter.Test.Builder
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class CallProcessActivityBuilderTest
    {

        CallProcessActivityBuilder CallProcessActivityBuilder;
        CallProcessActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.CallProcessActivityBuilder = new CallProcessActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new CallProcessActivity ( "My Call Process Activity", ActivityType.callProcessActivityType);
            this.activity.ProcessName = "/Process/DAI/PNO/process.To.Call.process";
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
                    Type= "System.String"}
            };
        }

        [Test]
        public void Should_Return_InvocationCode()
        {
            var expected = @"System.String xmlString;
xmlString = ""TestString"";

return this.processToCall.Start(xmlString);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.CallProcessActivityBuilder.GenerateMethods(this.activity, new Dictionary<string, string>())[0]);
            Assert.IsTrue(generatedCode.EndsWith(expected));
        }

        [Test]
        public void Should_Return_constructor_parameter()
        {
            var expected = "MyApp.Process.DAI.PNO.IProcessToCall";
            var constructorFields = this.CallProcessActivityBuilder.GenerateConstructorParameter(this.activity);
            Assert.AreEqual(expected, constructorFields[0].Type.BaseType);
        }
    }
}

