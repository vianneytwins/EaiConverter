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
    public class GroupActivityBuilderTest
    {
        GroupActivityBuilder groupActivityBuilder;
        GroupActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.groupActivityBuilder = new GroupActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new GroupActivity( "My Activity Name",ActivityType.groupActivityType);
            this.activity.GroupType = "simpleLoop";
            this.activity.Activities = new List<Activity>{ };
            this.activity.Transitions = new List<Transition>{ };
        }

        [Ignore]
        [Test]
        public void Should_Generate_invocation_method()
        {
            var expected = @"this.logger.Info(""Start Activity: My Activity Name of type: com.tibco.plugin.mapper.MapperActivity"");
EquityRecord EquityRecord = new EquityRecord();
EquityRecord.xmlString = ""TestString"";

EquityRecord myActivityName = EquityRecord;
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(groupActivityBuilder.Build(this.activity).InvocationCode);
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

