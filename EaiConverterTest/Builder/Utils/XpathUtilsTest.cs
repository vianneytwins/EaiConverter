namespace EaiConverterTest.Builder.Utils
{
    using System.Linq;
    using System.Xml.Linq;

    using EaiConverter.Builder.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class XpathUtilsTest
    {
        [Test]
        public void Should_Return_variableName_when_InputBinding_contains_a_xpath_formula_with_dollar_followed_by_the_varaible_name()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$myVar""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("myVar", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
            
        }
        
        [Test]
        public void Should_Return_variableName_when_InputBinding_contains_a_xpath_formula_with_dollar_followed_by_the_variable_name_and_a_parenthesis()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$params)""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("params", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
            
        }

        [Test]
        public void Should_Return_variableName_with_case_when_InputBinding_contains_a_xpath_formula_with_dollar_followed_by_the_variable_name()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$MyVar""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("MyVar", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());

        }

        [Test]
        public void Should_Return_variableName_when_InputBinding_contains_a_local_prefix_()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:ns=""http://mynamespace/schemas"">           
            <ns:logInfo>
<param>
                <message>
                    <xsl:value-of select=""$Start/sqlParams/FundName""/>
                </message>
</param>
</ns:logInfo>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start_sqlParams", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }
        
        [Test]
        public void Should_Return_variableName_when_InputBinding_contains_a_xpath_formula_with_dollar_followed_by_the_variable_name_slash()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$myVar/""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("myVar", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }

        [Test]
        public void Should_Return_variableName_when_InputBinding_contains_a_xpath_formula_with_dollar_followed_by_the_variable_name_and_add_operator()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$myVar+1""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("myVar", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }

        [Test]
        public void Should_Return_variableNames_when_InputBinding_contains_a_xpath_formula_with_several_dollars_and_variables()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$myVar/toto, contains($end/thisbidule)""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("myVar", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
            Assert.AreEqual("end", new XpathUtils().GetVariableNames(inputBindings)[1]);
        }

        [Test]
        public void Should_Return_variableNames_when_InputBinding_contains_a_xpath_formula_with_several_dollars_and_variables_in_different_xpath()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$myVar/toto""/>
        </xmlString>
        <xmlString>
            <xsl:value-of select=""contains($end/thisbidule)""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("myVar", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
            Assert.AreEqual("end", new XpathUtils().GetVariableNames(inputBindings)[1]);
        }
        
        [Test]
        public void Should_Return_variables_list_without_duplicates()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$myVar/toto""/>
        </xmlString>
        <xmlString>
            <xsl:value-of select=""contains($myVar/thisbidule)""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual(1, new XpathUtils().GetVariableNames(inputBindings).Count);
            Assert.AreEqual("myVar", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }

        [Test]
        public void Should_Return_variableName_composition_when_InputBinding_contains_a_xpath_formula_with_key_word_dollar_start()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start/Something""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start_Something", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }

        [Test]
        public void Should_Return_variableName_composition_when_InputBinding_contains_a_xpath_formula_with_key_word_dollar_start_and_nonSafeType()
        {
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start/param""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start_param", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }
        
        [Test]
        public void Should_Return_variableName_composition_and_remove_prefix_when_InputBinding_contains_a_xpath_formula_with_key_word_dollar_start()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
        <xmlString>
            <xsl:value-of select=""$start/pfx2:Something""/>
        </xmlString>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start_Something", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }
        
        [Test]
        public void Should_Return_variableName_when_InputBinding_contains_a_xpath_formula_with_key_word_dollar_finishing_by_a_coma()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
                            <Message>
                                <xsl:value-of select=""concat($Start/pfx3:logInfo, true(), ' ')""/>
                            </Message>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            Assert.AreEqual("start_logInfo", new XpathUtils().GetVariableNames(inputBindings).FirstOrDefault());
        }

        [Test]
        public void Should_Return_all_Local_Variable_from_List_when_InputBinding_contains_a_xpath_formula_contains_local_var()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
    <Log>      
        <xsl:variable name=""messageBody"" select=""tib:render-xml($Start/pfx3:logInfo, true(), true())""/>            
        <Message>
            <xsl:value-of select=""concat($Start/pfx3:logInfo, true(), $params,' ')""/>
        </Message>
    </Log>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            var variableNames = new XpathUtils().GetAllLocalVariables(inputBindings);
            Assert.AreEqual(1, variableNames.Count);
            Assert.AreEqual("messageBody", variableNames.FirstOrDefault());
        }

        [Test]
        public void Should_Return_remove_all_Local_Variable_from_List_when_InputBinding_contains_a_xpath_formula_contains_local_var()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
<Log>
                                
<xsl:variable name=""messageBody"" select=""tib:render-xml($Start/pfx3:logInfo, true(), true())""/>            
                <Message>
                    <xsl:value-of select=""concat($Start/pfx3:logInfo, true(), $messageBody,' ')""/>
                </Message>
</Log>              
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            var variableNames = new XpathUtils().GetVariableNames(inputBindings);
            Assert.AreEqual(1, variableNames.Count);
            Assert.AreEqual("start_logInfo", variableNames.FirstOrDefault());
        }

        [Test]
        public void Should_Return_complex_remove_all_Local_Variable_from_List_when_InputBinding_contains_a_xpath_formula_contains_local_var()
        {
            var xml =
    @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">           
 <inputs>
                <xsl:variable name=""params"">
                    <xsl:for-each select=""$Start/pfx3:logInfo/param"">
                        <xsl:if test=""name !='xml'"">
                            <xsl:value-of select=""concat(' [', name, '=', value, ']')""/>
                        </xsl:if>
                    </xsl:for-each>
                    <xsl:if test=""exists($Start/pfx3:logInfo/param[name='xml'])"">
                        <xsl:value-of select=""concat('&#xA;&#x9;', translate($Start/pfx3:logInfo/param[name='xml']/value, '&#xA;', ''))""/>
                    </xsl:if>
                </xsl:variable>
                
                     <Interface>
                        <xsl:value-of select=""Start/pfx3:logInfo/GetProcessInstanceInfo[1]/ProcessStarterName""/>
                    </Interface>
                     </inputs>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var inputBindings = doc.Nodes();
            var variableNames = new XpathUtils().GetVariableNames(inputBindings);
            Assert.AreEqual(1, variableNames.Count);
            Assert.AreEqual("start_logInfo", variableNames.FirstOrDefault());
        }
    }
}
