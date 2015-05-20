using System;
using NUnit.Framework;
using EaiConverter.Mapper;
using EaiConverter.Model;
using System.Xml.Linq;
using EaiConverter.Test.Utils;

namespace EaiConverterTest
{
    [TestFixture]
    public class AssignActivityBuilderTest
    {
        AssignActivityBuilder builder;
        AssignActivity assignActivity;

        [SetUp]
        public void SetUp(){
            this.builder = new AssignActivityBuilder (new XslBuilder(new XpathBuilder()));
            var inputBindings = @"
<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"">
    <var xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""'EVL'""/>
        </AdminID>
    </var>
</pd:inputBindings>            
";
            XElement doc = XElement.Parse(inputBindings);     

            this.assignActivity = new AssignActivity {
                Type = ActivityType.assignActivityType,
                Name = "TestAssignActivity",
                VariableName = "var",
                InputBindings = doc.Nodes()
            };  

        }

        [Test]
        public void Should_Return_Empty_Class_To_Generate(){
            var activityCodeDom = this.builder.Build(this.assignActivity);
            Assert.AreEqual(0, activityCodeDom.ClassesToGenerate.Count);
        }

        [Test]
        public void Should_Assign_Variable_To_its_new_value(){
            var activityCodeDom = this.builder.Build(this.assignActivity);
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityCodeDom.InvocationCode);

            var expectedCode = @"var var = new var();
var.FundName = ""testvalue"";
var.AdminID = ""EVL"";

this.var = var;
";
            Assert.AreEqual(expectedCode, generatedCode);
        }
    }
}

