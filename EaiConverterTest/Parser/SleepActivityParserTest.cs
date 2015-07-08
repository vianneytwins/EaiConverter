using System;
using NUnit.Framework;
using System.Xml.Linq;
using EaiConverter.Parser;
using EaiConverter.Model;

namespace EaiConverter.Test.Parser
{

    [TestFixture]
    public class SleepActivityParserTest
    {

        SleepActivityParser activityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            activityParser = new SleepActivityParser ();
            var xml =
                @"<pd:activity name=""sleep activity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns1=""www.tibco.com/plugin/Sleep"">
<pd:type>com.tibco.plugin.timer.SleepActivity</pd:type>
<pd:inputBindings>
    <ns1:SleepInputSchema>
        <IntervalInMillisec>
            <xsl:value-of select=""3000""/>
        </IntervalInMillisec>
    </ns1:SleepInputSchema>
</pd:inputBindings>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_NullActivity (){
            var activity = activityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.timer.SleepActivity", activity.Type.ToString());
        }

        [Test]
        public void Should_Return_Parameter_Timer_interval(){
            var activity = (SleepActivity) activityParser.Parse (doc);

            Assert.AreEqual ("IntervalInMillisec", activity.Parameters[0].Name);
        }

    }
}

