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
    public class SleepActivityBuilderTest
    {
        SleepActivityBuilder activityBuilder;
        SleepActivity activity;

        [SetUp]
        public void SetUp()
        {
            this.activityBuilder = new SleepActivityBuilder(new XslBuilder(new XpathBuilder()));
            this.activity = new SleepActivity( "MyActivityName", ActivityType.sleepActivity);
            var xml =
                @"<ns1:SleepInputSchema xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns1=""www.tibco.com/plugin/Sleep"">
        <IntervalInMillisec>
            <xsl:value-of select=""3000""/>
        </IntervalInMillisec>
    </ns1:SleepInputSchema>
";
            XElement doc = XElement.Parse(xml);
            this.activity.InputBindings = doc.Nodes();
            this.activity.Parameters = new List<ClassParameter>{
                new ClassParameter{
                    Name = "IntervalInMillisec",
                    Type= "int"}
            };
        }


        [Test]
        public void Should_Generate_invocation_method()
        {
            var expected = @"this.logger.Info(""Start Activity: MyActivityName of type: com.tibco.plugin.timer.SleepActivity"");
Int32 IntervalInMillisec;
IntervalInMillisec = 3000;

new Timer(IntervalInMillisec);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateInvocationCode(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}

