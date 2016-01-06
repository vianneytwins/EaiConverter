namespace EaiConverterTest.Builder.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using EaiConverter.Builder.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class XpathUtilsTest
    {
        [Test]
        public void Should_Return_start_when_InputBinding_contains_a_xpath_formula_with_dollar_start()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
            
        }

        [Test]
        public void Should_Return_Start_when_InputBinding_contains_a_xpath_formula_with_dollar_start()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$Start""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("Start", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());

        }

        [Test]
        public void Should_Return_Start_when_InputBinding_contains_a_xpath_formula_with_dollar_start_slash()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start/""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }

        [Test]
        public void Should_Return_Start_and_end_when_InputBinding_contains_a_xpath_formula_with_dollar_start_slash_and_dollaes_end()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start/toto, contains($end/thisbidule)""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
            Assert.AreEqual("end", new XpathUtils().GetVariableNames(inputBindings)[1]);
        }

        [Test]
        public void Should_Return_Start_and_end_when_InputBinding_contains_a_xpath_formula_with_dollar_start_slash_and_dollars_end_in_different_xpath()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start/toto""/>
        </xmlString>
        <xmlString>
            <xsl:value-of select=""contains($end/thisbidule)""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
            Assert.AreEqual("end", new XpathUtils().GetVariableNames(inputBindings)[1]);
        }
        
        [Test]
        public void Should_Return_variables_list_without_duplicates()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start/toto""/>
        </xmlString>
        <xmlString>
            <xsl:value-of select=""contains($start/thisbidule)""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual(1, new XpathUtils().GetVariableNames(inputBindings).Count);
            Assert.AreEqual("start", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }
    }
}
