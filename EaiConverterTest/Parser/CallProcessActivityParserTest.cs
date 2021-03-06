﻿namespace EaiConverter.Test.Parser
{
    using System.Linq;
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser;

    using NUnit.Framework;

    [TestFixture]
    public class CallProcessActivityParserTest
    {
        CallProcessActivityParser callProcessActivityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
            callProcessActivityParser = new CallProcessActivityParser ();
            var xml =
                @"<pd:activity name=""LookUp Strat"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.pe.core.CallProcessActivity</pd:type>
<config>
<processName>/Process/DAI/PrLookup.Fund.process</processName>

</config>
<pd:inputBindings>
    <sqlParams>
        <FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>
    </sqlParams>
</pd:inputBindings>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_CallProcessActivity ()
        {
            CallProcessActivity callProcessActivity = (CallProcessActivity) callProcessActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.pe.core.CallProcessActivity", callProcessActivity.Type.ToString());
        }


        [Test]
        public void Should_Return_ProcessName_in_config()
        {
            CallProcessActivity callProcessActivity = (CallProcessActivity) callProcessActivityParser.Parse (doc);

            Assert.AreEqual ("/Process/DAI/PrLookup.Fund.process", callProcessActivity.ProcessName);
        }
        
        [Test]
        public void Should_Return_Namespace_of_input_paramter_in_config()
        {
            var xml =
    @"<pd:activity name=""LookUp Strat"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx3=""http://pfx3Transform"">
<pd:type>com.tibco.pe.core.CallProcessActivity</pd:type>
<config>
    <processName>/Process/DAI/PrLookup.Fund.process</processName>
</config>
<pd:inputBindings>
    <pfx3:sqlParams>
        <FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>
    </pfx3:sqlParams>
</pd:inputBindings>
</pd:activity>";
            var doc2 = XElement.Parse(xml);
            
            CallProcessActivity callProcessActivity = (CallProcessActivity)this.callProcessActivityParser.Parse(doc2);

            Assert.AreEqual("http://pfx3Transform", ((XElement)callProcessActivity.InputBindings.First()).Name.Namespace.ToString());
        }


    
    }
}

