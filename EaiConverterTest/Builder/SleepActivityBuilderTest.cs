﻿using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using EaiConverter.Test.Utils;

namespace EaiConverterTest.Builder
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
            this.activity = new SleepActivity( "My Activity Name", ActivityType.sleepActivity);
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
            var expected = @"this.logger.Info(""Start Activity: My Activity Name of type: com.tibco.plugin.timer.SleepActivity"");
int IntervalInMillisec = 3000;

new Timer(IntervalInMillisec);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(activityBuilder.GenerateCodeInvocation(this.activity));
            Assert.AreEqual(expected,generatedCode);
        }
    }
}
