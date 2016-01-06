namespace EaiConverter.Test.Builder
{
    using System.Collections.Generic;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class GroupActivityBuilderTest
    {
        private GroupActivityBuilder groupActivityBuilder;
        private GroupActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.groupActivityBuilder = new GroupActivityBuilder();
            this.activity = new GroupActivity( "My Activity Name", ActivityType.loopGroupActivityType);

            this.activity.Activities = new List<Activity>
            {
                new Activity("myNullActivity",ActivityType.nullActivityType),
            };
            this.activity.Transitions = new List<Transition>
            {
                new Transition
                {
                    FromActivity = "start",
                    ToActivity = "myNullActivity",
                    ConditionType = ConditionType.always
                },
                new Transition
                {
                    FromActivity = "myNullActivity",
                    ToActivity = "end",
                    ConditionType = ConditionType.always
                }
            };
        }

        [Test]
        public void Should_Generate_invocation_method_For_simpleGroup()
        {
            /**
            var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.LoopGroup"");
this.MyNullActivityCall();
";*/
            var expected = "this.MyNullActivityCall();\n";
            this.activity.GroupType = GroupType.SIMPLEGROUP;
            this.groupActivityBuilder.GenerateClassesToGenerate(this.activity, null);
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.groupActivityBuilder.GenerateInvocationCode(this.activity, null));
            Assert.AreEqual(expected, generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_For_inputLoop()
        {
            /**
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.LoopGroup"");
for (int index = 0; (index < paramsets.elements.Length); index = (index + 1))
{
    var current = paramsets.elements[index];
    this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
}
";*/
            var expected = @"for (int index = 0; (index < paramsets.elements.Length); index = (index + 1))
{
    var current = paramsets.elements[index];
    this.MyNullActivityCall();
}
";
            this.activity.GroupType = GroupType.INPUTLOOP;
            this.activity.Over = "$Paramsets/elements";
            this.activity.IndexSlot = "index";
            this.activity.IterationElementSlot = "current";
			this.groupActivityBuilder.GenerateClassesToGenerate(this.activity, null);
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.groupActivityBuilder.GenerateInvocationCode(this.activity, null));
            Assert.AreEqual(expected, generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_For_repeatLoop()
        {
            /**
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.LoopGroup"");
for (
; !(true); 
)
{
    this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
}
";*/
            var expected = @"for (
; !(true); 
)
{
    this.MyNullActivityCall();
}
";
            this.activity.GroupType = GroupType.REPEAT;
            this.activity.RepeatCondition = "true";
            this.groupActivityBuilder.GenerateClassesToGenerate(this.activity, null);
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.groupActivityBuilder.GenerateInvocationCode(this.activity, null));
            Assert.AreEqual(expected, generatedCode);
        }

        [Test]
        public void Should_Generate_invocation_method_For_criticalSection()
        {
            /**
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.pe.core.CriticalSectionGroup"");
lock (my_Activity_NameLock){
this.logger.Info(""Start Activity: myNullActivity of type: com.tibco.plugin.timer.NullActivity"");
}
";*/
            var expected = @"lock (my_Activity_NameLock){
this.MyNullActivityCall();
}
";
            this.activity.Type = ActivityType.criticalSectionGroupActivityType;
            this.activity.GroupType = GroupType.CRITICALSECTION;
            this.groupActivityBuilder.GenerateClassesToGenerate(this.activity, null);
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.groupActivityBuilder.GenerateInvocationCode(this.activity, null));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

