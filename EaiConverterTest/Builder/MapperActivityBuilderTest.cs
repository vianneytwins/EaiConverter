using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using EaiConverter.Test.Utils;
using System.Xml.Linq;
using System.Collections.Generic;
using EaiConverter;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class MapperActivityBuilderTest
    {
        MapperActivityBuilder xmlParseActivityBuilder;
        MapperActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.xmlParseActivityBuilder = new MapperActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new MapperActivity( "My Activity Name",ActivityType.mapperActivityType);
            this.activity.XsdReference = "pf4:EquityRecord";
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
    <EquityRecord>            
        <xmlString>
            <xsl:value-of select=""'TestString'""/>
        </xmlString>
    </EquityRecord>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            this.activity.InputBindings = doc.Nodes();
            this.activity.Parameters = new List<ClassParameter>{
                new ClassParameter{
                    Name = "EquityRecord",
                    Type= "EquityRecord"}
            };
        }

        [Test]
        public void Should_Generate_invocation_method()
        {
            var expected = @"this.logger.Info(""Start Activity: My Activity Name"");
EquityRecord EquityRecord = new EquityRecord();
EquityRecord.xmlString = ""TestString"";

EquityRecord myActivityName = EquityRecord;
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(xmlParseActivityBuilder.GenerateCodeInvocation(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

