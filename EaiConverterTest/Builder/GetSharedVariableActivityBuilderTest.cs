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
			var expected = @"
var configName = ""myconfigPath"";
var myActivityName = this.sharedVariableService.Get(configName);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateInvocationCode(this.activity));
            Assert.IsTrue(generatedCode.EndsWith(expected));
        }
    }
}

