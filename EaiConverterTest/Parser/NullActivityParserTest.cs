using NUnit.Framework;

using EaiConverter.Parser;
using System.Xml.Linq;

namespace EaiConverter.Test.Parser
{
    [TestFixture]
    public class NullActivityParserTest
    {
        NullActivityParser activityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            activityParser = new NullActivityParser ();
            var xml =
                @"<pd:activity name=""java call activity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.timer.NullActivity</pd:type>

</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_NullActivity (){
            var activity = activityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.timer.NullActivity", activity.Type.ToString());
        }

    

    }
}

