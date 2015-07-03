using System;
using NUnit.Framework;
using System.Xml.Linq;
using EaiConverter.Parser;

namespace EaiConverter.Test.Parser
{
    [TestFixture]
    public class GlobalVariableParserTest
    {
        GlobalVariableParser parser;
        XElement doc;

        [SetUp]
        public void SetUp()
        {
            parser = new GlobalVariableParser();
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
<repository xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://www.tibco.com/xmlns/repo/types/2002"">
    <globalVariables>
        <globalVariable>
            <name>Name test 1</name>
            <value>value test 1</value>
            <type>String</type>
            <deploymentSettable>true</deploymentSettable>
            <serviceSettable>false</serviceSettable>
            <modTime>13304412311412</modTime>
        </globalVariable>
        <globalVariable>
            <name>Name test 2</name>
            <value>value test 2</value>
            <type>String</type>
            <deploymentSettable>true</deploymentSettable>
            <serviceSettable>false</serviceSettable>
            <modTime>13304412311411</modTime>
        </globalVariable>
    </globalVariables>
</repository>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_2_Global_Variables()
        {
            var globalVariables  = parser.Parse(doc);
            Assert.AreEqual(2,globalVariables.Count);

        }
    }
}

