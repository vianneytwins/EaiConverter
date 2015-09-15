using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using EaiConverter.Test.Utils;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
	public class SetSharedVariableActivityBuilderTest
    {
		IActivityBuilder activityBuilder;
		SharedVariableActivity activity;

        [SetUp]
        public void SetUp()
        {
			this.activityBuilder = new SetSharedVariableActivityBuilder(new XslBuilder(new XpathBuilder()));
			this.activity = new SharedVariableActivity( "MyActivityName", ActivityType.setSharedVariableActivityType);
			this.activity.VariableConfig = "myconfigPath";
			var xml =
				@"
    <ns:ActivityInput xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns=""http://www.tibco.com/pe/GenerateErrorActivitySchema"">
        <message>
            <xsl:value-of select=""'testvalue'""/>
        </message>
    </ns:ActivityInput>
";
			XElement doc = XElement.Parse(xml);
			this.activity.InputBindings = doc.Nodes();
			this.activity.Parameters = new List<ClassParameter>{
				new ClassParameter{
					Name = "message",
					Type= "System.String"}
			};
        }

	
        [Test]
        public void Should_Generate_invocation_method()
        {
			var expected = @"System.String message = ""testvalue"";

var configName = ""myconfigPath"";
this.sharedVariableService.Set(configName, message);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateInvocationCode(this.activity));
            Assert.IsTrue(generatedCode.EndsWith(expected));
        }
    }
}

