using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using EaiConverter.Test.Utils;
using System.Xml.Linq;
using System.Collections.Generic;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class XmlParserActivityBuilderTest
    {

        XmlParseActivityBuilder xmlParseActivityBuilder;
        XmlParseActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.xmlParseActivityBuilder = new XmlParseActivityBuilder(new XslBuilder(new XpathBuilder()), new XmlParserHelperBuilder());
            this.activity = new XmlParseActivity( "My Activity Name",ActivityType.xmlParseActivityType);
            this.activity.XsdReference = "pf4:EquityRecord";
            var xml =
                @"
        <pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
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
        public void Should_Generate_invocation_method()
        {
            var expected = @"this.logger.Info(""Start Activity: My Activity Name of type: com.tibco.plugin.xml.XMLParseActivity"");
string xmlString = ""TestString"";

EquityRecord myActivityName = this.xmlParserHelperService.FromXml(xmlString);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(xmlParseActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

