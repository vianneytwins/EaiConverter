using EaiConverter.Parser;

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
            this.CallProcessActivityBuilder = new CallProcessActivityBuilder(new XslBuilder(new XpathBuilder()), new OverideTibcoBWProcessLinqParser());
            this.activity = new CallProcessActivity("My Call Process Activity", ActivityType.callProcessActivityType);
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
            this.activity.Parameters = new List<ClassParameter>
            {
                new ClassParameter
                {
                    Name = "xmlString",
                    Type = "System.String"
                }
            };
        }

        [Test]
        public void Should_Return_Method_body()
        {
            var expected = @"System.String xmlString;
xmlString = ""TestString"";

return this.processToCall.start(xmlString);
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

        [Test]
        public void Should_Return_2_input_parameter_in_the_method()
        {
            var activity = new CallProcessActivity("My Call Process Activity", ActivityType.callProcessActivityType);
            activity.ProcessName = "/Process/DAI/PNO/process.To.Call.process";
            var xml =
                @"<pd:inputBindings  xmlns:ns=""http://mynamespace/schemas"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
            <ns:logInfo>
                <message>
                    <xsl:value-of select=""'Lookup Strategy'""/>
                </message>
                <msgCode>
                    <xsl:value-of select=""'LOOKUP'""/>
                </msgCode>
                <param>
                    <name>
                        <xsl:value-of select=""'in:FundName'""/>
                    </name>
                    <value>
                        <xsl:value-of select=""$Start/sqlParams/FundName""/>
                    </value>
                </param>
                <param>
                    <name>
                        <xsl:value-of select=""'out:Strategy'""/>
                    </name>
                    <value>
                        <xsl:value-of select=""$Lookup-Strategy/resultSet/Resultsets/ResultSet1[1]/Record1[1]/Strategy""/>
                    </value>
                </param>
                <param>
                    <name>
                        <xsl:value-of select=""'out:SubStrategy'""/>
                    </name>
                    <value>
                        <xsl:value-of select=""$Lookup-Strategy/resultSet/Resultsets/ResultSet1[1]/Record1[1]/SubStrategy""/>
                    </value>
                </param>
            </ns:logInfo>
        </pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            activity.InputBindings = doc.Nodes();
            activity.Parameters = new List<ClassParameter>
            {
                new ClassParameter
                {
                    Name = "logInfo",
                    Type = "logInfo"
                }
            };

            var executeQueryMethod = this.CallProcessActivityBuilder.GenerateMethods(activity, new Dictionary<string, string> { { "start_sqlParams", "start_sqlParams" }, { "Lookup_Strategy", "Lookup_Strategy" } })[0];
            
            Assert.AreEqual(2, executeQueryMethod.Parameters.Count);
        }

        [Test]
        public void Should_Return_Called_Process_return_type()
        {
            var expected = "MyType";
            Assert.AreEqual(expected, this.CallProcessActivityBuilder.GetReturnType(this.activity));
        }
    }

    public class OverideTibcoBWProcessLinqParser : TibcoBWProcessLinqParser 
    {
        public override TibcoBWProcess Parse(string filePath)
        {
            return new TibcoBWProcess("tata")
                       {
                           StartActivity = new Activity("start",ActivityType.startType),
                           EndActivity = new Activity("End",ActivityType.endType){Parameters = new List<ClassParameter>{new ClassParameter(){Type = "MyType", Name = "myEndVar"}}}
                       };
        } 
    }
}

