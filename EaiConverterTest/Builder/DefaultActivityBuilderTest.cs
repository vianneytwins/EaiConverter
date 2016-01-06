using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Test.Utils;
using EaiConverter.Model;

namespace EaiConverter.Test.Builder
{
    using System.Collections.Generic;

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
            var logCode = defaultBuilder.GenerateMethod(activity, new Dictionary<string, string>());
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(logCode.Statements);
            Assert.AreEqual("this.logger.Info(\"Start Activity: test_Activity of type: "+ActivityType.NotHandleYet+"\");\n", generatedCode);
        }
    }
}

