using System;
using NUnit.Framework;
using EaiConverter.Mapper;

namespace EaiConverterTest
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
            var logCode = DefaultActivityBuilder.LogActivity("test Activity");

        }


    }
}

