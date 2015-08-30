namespace EaiConverter.Test.Builder
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Parser;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class MapperActivityBuilderTest
    {
		MapperActivityBuilder mapperActivityBuilder;
        MapperActivity activity;

        [SetUp]
        public void SetUp()
        {
			this.mapperActivityBuilder = new MapperActivityBuilder(new XslBuilder(new XpathBuilder()), new XsdBuilder(), new XsdParser());
            this.activity = new MapperActivity("My Activity Name", ActivityType.mapperActivityType);
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
        public void Should_Generate_invocation_method_When_XsdReference_is_present()
        {
            var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.mapper.MapperActivity"");
EquityRecord EquityRecord = new EquityRecord();
EquityRecord.xmlString = ""TestString"";

EquityRecord my_Activity_Name = EquityRecord;
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.mapperActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected, generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_When_XsdReference_is_present_with_no_prefix()
        {
            var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.mapper.MapperActivity"");
EquityRecord EquityRecord = new EquityRecord();
EquityRecord.xmlString = ""TestString"";

EquityRecord my_Activity_Name = EquityRecord;
";

            this.activity.XsdReference = "EquityRecord"; 

            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.mapperActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected, generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_When_XsdReference_is_not_present()
		{
			this.activity.XsdReference = null;
			var xsdElement = "<element xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n<xsd:element name=\"EquityRecord\" ><xsd:complexType><xsd:sequence><xsd:element name=\"adminID\" type=\"xsd:string\" /></xsd:sequence></xsd:complexType></xsd:element>\n</element>";
		    XElement doc = XElement.Parse(xsdElement);
            this.activity.ObjectXNodes = doc.Nodes();

			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.mapper.MapperActivity"");
EquityRecord EquityRecord = new EquityRecord();
EquityRecord.xmlString = ""TestString"";

MyApp.Mydomain.Service.Contract.My_Activity_Name.EquityRecord my_Activity_Name = EquityRecord;
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.mapperActivityBuilder.GenerateInvocationCode(this.activity));
			Assert.AreEqual(expected, generatedCode);
		}

		[Test]
		public void Should_Generate_Classes_method_When_XsdReference_is_not_present()
		{
			this.activity.XsdReference = null;
			var xsdElement = "<element xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n<xsd:element name=\"EquityRecord\" ><xsd:complexType><xsd:sequence><xsd:element name=\"adminID\" type=\"xsd:string\" /></xsd:sequence></xsd:complexType></xsd:element>\n</element>";
			XElement doc = XElement.Parse(xsdElement);
			this.activity.ObjectXNodes = doc.Nodes();

			var generatedClasses = this.mapperActivityBuilder.GenerateClassesToGenerate(this.activity);
			Assert.AreEqual(1, generatedClasses.Count);
			Assert.AreEqual("EquityRecord", generatedClasses[0].Types[0].Name);
		}
    }
}

