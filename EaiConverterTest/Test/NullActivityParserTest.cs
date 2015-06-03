using System;
using NUnit.Framework;
using EaiConverter.Model;
using EaiConverter.Parser;
using System.Xml.Linq;

namespace EaiConverterTest.Test
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
<pd:type>com.tibco.plugin.java.JavaActivity</pd:type>

</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_JavaActivity (){
            var activity = activityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.java.JavaActivity", activity.Type.ToString());
        }

    

    }
}

