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
	public class GetSharedVariableActivityBuilderTest
    {
		IActivityBuilder activityBuilder;
		SharedVariableActivity activity;

        [SetUp]
        public void SetUp()
        {
			this.activityBuilder = new GetSharedVariableActivityBuilder(new XslBuilder(new XpathBuilder()));
			this.activity = new SharedVariableActivity( "MyActivityName", ActivityType.getSharedVariableActivityType);
			this.activity.VariableConfig = "myconfigPath";

        }

	
        [Test]
        public void Should_Generate_invocation_method()
        {
            var expected = @"var configName = ""myconfigPath"";
return this.sharedVariableService.Get(configName);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateMethod(this.activity, new Dictionary<string, string>()).Statements);
            Assert.IsTrue(generatedCode.EndsWith(expected));
        }
    }
}

