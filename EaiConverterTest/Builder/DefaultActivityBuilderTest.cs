using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Test.Utils;
using EaiConverter.Model;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class DefaultActivityBuilderTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Should_Log_Activity_Start()
        {
            var logCode = DefaultActivityBuilder.LogActivity(new Activity ("test Activity", ActivityType.assignActivityType));
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(logCode);
            Assert.AreEqual("this.logger.Info(\"Start Activity: test Activity of type: "+ActivityType.assignActivityType+"\");\n", generatedCode);
        }
    }
}

