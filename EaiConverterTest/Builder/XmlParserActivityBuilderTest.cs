namespace EaiConverter.Test.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Parser;
    using EaiConverter.Processor;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class XmlParserActivityBuilderTest
    {
        private XmlParseActivityBuilder xmlParseActivityBuilder;
        private XmlParseActivity activity;

        [SetUp]
        public void SetUp()
        {
			this.xmlParseActivityBuilder = new XmlParseActivityBuilder(new XslBuilder(new XpathBuilder()), new XmlParserHelperBuilder(), new XsdBuilder(), new XsdParser());
            this.activity = new XmlParseActivity("My Activity Name", ActivityType.xmlParseActivityType);
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
            this.activity.Parameters = new List<ClassParameter>
            {
                new ClassParameter
                {
                    Name = "xmlString",
                    Type = "string"
                }
            };
        }

        [Test]
        public void Should_Generate_invocation_method_When_XsdReference_is_present()
        {
            string expected;

            if (Environment.OSVersion.ToString().Contains("indows"))
            {
                expected =
                    @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.xml.XMLParseActivity"");
System.String xmlString;
xmlString = ""TestString"";

EquityRecord my_Activity_Name = this.xmlParserHelperService.FromXml<EquityRecord>(xmlString);
";
            }
            else
            {
                expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.xml.XMLParseActivity"");
System.String xmlString;
xmlString = ""TestString"";

EquityRecord my_Activity_Name = this.xmlParserHelperService.FromXml <EquityRecord>(xmlString);
";
            }

            var generatedCode = TestCodeGeneratorUtils.GenerateCode(xmlParseActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected, generatedCode);
        }

		[Test]
		public void Should_Generate_invocation_method_When_XsdReference_is_not_present()
		{
			this.activity.XsdReference = null;
			var xsdElement = "<term xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n<xsd:element name=\"EquityRecord\" ><xsd:complexType><xsd:sequence><xsd:element name=\"adminID\" type=\"xsd:string\" /></xsd:sequence></xsd:complexType></xsd:element>\n</term>";
			XElement doc = XElement.Parse(xsdElement);
			this.activity.ObjectXNodes =doc.Nodes();

            string expected;

		    if (Environment.OSVersion.ToString().Contains("indows"))
		    {
		        expected =
		            @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.xml.XMLParseActivity"");
System.String xmlString;
xmlString = ""TestString"";

MyApp.Mydomain.Service.Contract.My_Activity_Name.EquityRecord my_Activity_Name = this.xmlParserHelperService.FromXml<MyApp.Mydomain.Service.Contract.My_Activity_Name.EquityRecord>(xmlString);
";
		    }
		    else
		    {
                expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.xml.XMLParseActivity"");
System.String xmlString;
xmlString = ""TestString"";

MyApp.Mydomain.Service.Contract.My_Activity_Name.EquityRecord my_Activity_Name = this.xmlParserHelperService.FromXml <MyApp.Mydomain.Service.Contract.My_Activity_Name.EquityRecord>(xmlString);
";
		    }

		    var generatedCode = TestCodeGeneratorUtils.GenerateCode(xmlParseActivityBuilder.GenerateInvocationCode(this.activity));
			Assert.AreEqual(expected,generatedCode);
		}

		[Test]
		public void Should_Generate_Classes_method_When_XsdReference_is_present()
		{
			ConfigurationApp.SaveProperty("IsXmlParserHelperAlreadyGenerated", "false");
			var generatedClasses = xmlParseActivityBuilder.GenerateClassesToGenerate (this.activity);
			Assert.AreEqual(2, generatedClasses.Count);
		}

		[Test]
		public void Should_Generate_Classes_method_When_XsdReference_is_not_present()
		{
			ConfigurationApp.SaveProperty("IsXmlParserHelperAlreadyGenerated", "false");
			this.activity.XsdReference = null;
			var xsdElement = "<element xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n<xsd:element name=\"EquityRecord\" ><xsd:complexType><xsd:sequence><xsd:element name=\"adminID\" type=\"xsd:string\" /></xsd:sequence></xsd:complexType></xsd:element>\n</element>";
			XElement doc = XElement.Parse(xsdElement);
			this.activity.ObjectXNodes = doc.Nodes();

			var generatedClasses = xmlParseActivityBuilder.GenerateClassesToGenerate (this.activity);
			Assert.AreEqual(3, generatedClasses.Count);
			Assert.AreEqual("EquityRecord", generatedClasses[2].Types[0].Name);
		}

    }
}

