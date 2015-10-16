using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using EaiConverter.Test.Utils;

using System.Collections.Generic;

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
            this.activity = new GroupActivity( "My Activity Name",ActivityType.loopGroupActivityType);

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
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.LoopGroup"");
this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
";
            this.activity.GroupType = GroupType.SIMPLEGROUP;
			groupActivityBuilder.GenerateClassesToGenerate (this.activity);
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(groupActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_For_inputLoop()
        {
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.LoopGroup"");
for (int index = 0; (index < paramsets.elements.Length); index = (index + 1))
{
    var current = paramsets.elements[index];
    this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
}
";
            this.activity.GroupType = GroupType.INPUTLOOP;
            this.activity.Over = "$Paramsets/elements";
            this.activity.IndexSlot = "index";
            this.activity.IterationElementSlot = "current";
			groupActivityBuilder.GenerateClassesToGenerate (this.activity);
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(groupActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_For_repeatLoop()
        {
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.LoopGroup"");
for (
; !(true); 
)
{
    this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
}
";
            this.activity.GroupType = GroupType.REPEAT;
            this.activity.RepeatCondition = "true";
			groupActivityBuilder.GenerateClassesToGenerate (this.activity);
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(groupActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_For_criticalSection()
        {
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.CriticalSectionGroup"");
lock (my_Activity_NameLock){
this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
}
";
            this.activity.Type = ActivityType.criticalSectionGroupActivityType;
            this.activity.GroupType = GroupType.CRITICALSECTION;
			groupActivityBuilder.GenerateClassesToGenerate (this.activity);
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(groupActivityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

