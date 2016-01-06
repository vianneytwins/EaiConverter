namespace EaiConverter.Test.Builder
{
    using System.Collections.Generic;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
	public class ConfirmActivityBuilderTest
    {
		IActivityBuilder activityBuilder;
		ConfirmActivity activity;

        [SetUp]
        public void SetUp()
        {
			this.activityBuilder = new ConfirmActivityBuilder();
			this.activity = new ConfirmActivity( "MyActivityName", ActivityType.ConfirmActivityType);
        }


        [Test]
        public void Should_Generate_invocation_method()
        {
			var expected = @"this.logger.Info(""Start Activity: MyActivityName of type: com.tibco.pe.core.ConfirmActivity"");
// TODO: Should be this.subscriber.Confirm(message);
this.subscriber.Confirm();
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateMethod(this.activity, new Dictionary<string, string>()).Statements);
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

