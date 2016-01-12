namespace EaiConverter.Test.Builder
{
    using System.Collections.Generic;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultActivityBuilderTest
    {
        private IActivityBuilder defaultBuilder;

        private Activity activity;

        [SetUp]
        public void SetUp()
        {
            this.defaultBuilder = new DefaultActivityBuilder();
            this.activity = new Activity("test_Activity", ActivityType.NotHandleYet);
        }

        [Test]
        public void Should_Log_Activity_Start()
        {
            var logCode = defaultBuilder.GenerateMethods(this.activity, new Dictionary<string, string>());
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(logCode[0].Statements);
            Assert.AreEqual("this.logger.Info(\"Start Activity: test_Activity of type: " + ActivityType.NotHandleYet + "\");\nthis.test_Activity.Execute();\n", generatedCode);
        }
    }
}

