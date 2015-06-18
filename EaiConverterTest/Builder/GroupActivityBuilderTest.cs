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

            this.activity.Activities = new List<Activity>{
                new Activity("myNullActivity",ActivityType.nullActivityType),
            };
            this.activity.Transitions = new List<Transition>{
                new Transition{
                    FromActivity="start",
                    ToActivity="myNullActivity",
                    ConditionType=ConditionType.always
                },
                new Transition{
                    FromActivity="myNullActivity",
                    ToActivity="end",
                    ConditionType=ConditionType.always
                }
            };
        }

        [Test]
        public void Should_Generate_invocation_method_For_simpleGroup()
        {
            var expected = @"this.logger.Info(""Start Activity: My Activity Name of type: com.tibco.pe.core.LoopGroup"");
this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
";
            this.activity.GroupType = GroupType.simpleGroup;

            var generatedCode = TestCodeGeneratorUtils.GenerateCode(groupActivityBuilder.Build(this.activity).InvocationCode);
            Assert.AreEqual(expected,generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_For_inputLoop()
        {
            var expected = @"this.logger.Info(""Start Activity: My Activity Name of type: com.tibco.pe.core.LoopGroup"");
for (int index = 0; (index < $Paramsets.elements.Lenght); index = (index + 1))
{
    var current = $Paramsets.elements[index];
    this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
}
";
            this.activity.GroupType = GroupType.inputLoop;
            this.activity.Over = "$Paramsets.elements";
            this.activity.IndexSlot = "index";
            this.activity.IterationElementSlot = "current";

            var generatedCode = TestCodeGeneratorUtils.GenerateCode(groupActivityBuilder.Build(this.activity).InvocationCode);
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

