using System;
using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter
{
    [TestFixture]
    public class WriteToLogActivityParserTest
    {
        WriteToLogActivityParser writetoLogActivityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            writetoLogActivityParser = new WriteToLogActivityParser ();
            var xml =
                @"<pd:activity name=""write to log"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns=""http://www.tibco.com/pe/WriteToLogActivitySchema"">
<pd:type>com.tibco.pe.core.WriteToLogActivity</pd:type>
<config>
    <role>Info</role>
</config>
<pd:inputBindings>
    <ns:ActivityInput>
        <message>
            <xsl:value-of select=""testvalue""/>
        </message>
        <msgCode>
            <xsl:value-of select=""EVL""/>
        </msgCode>
    </ns:ActivityInput>
</pd:inputBindings>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_writeToLogActivity (){
            var writeToLogActivity = writetoLogActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.pe.core.WriteToLogActivity", writeToLogActivity.Type.ToString());
        }


        [Test]
        public void Should_Return_Log_level_in_Role_from_config(){
            var writeToLogActivity = (WriteToLogActivity) writetoLogActivityParser.Parse (doc);

            Assert.AreEqual ("Info", writeToLogActivity.Role);
        }

        [Test]
        public void Should_Return_Paramter(){
            var writeToLogActivity = (WriteToLogActivity) writetoLogActivityParser.Parse (doc);

            Assert.AreEqual ("message", writeToLogActivity.Parameters[0].Name);
        }
    
    }
}

