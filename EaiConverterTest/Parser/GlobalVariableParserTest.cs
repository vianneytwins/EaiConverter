﻿using System;
using NUnit.Framework;
using System.Xml.Linq;
using EaiConverter.Parser;
using EaiConverter.Model;

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

        [Test]
        public void Should_Parse_name()
        {
            var globalVariables  = parser.Parse(doc);
            Assert.AreEqual("Name test 1",globalVariables[0].Name);
        }

        [Test]
        public void Should_Parse_Value()
        {
            var globalVariables  = parser.Parse(doc);
            Assert.AreEqual("value test 1",globalVariables[0].Value);
        }

        [Test]
        public void Should_Parse_type()
        {
            var globalVariables  = parser.Parse(doc);
            Assert.AreEqual(GlobalVariableType.String, globalVariables[0].Type);
        }

        [Test]
        public void Should_Retrieve_PackageName_from_path()
        {
            Assert.AreEqual("myProject.Config", parser.ParsePackageName("myProject/Config/MarketConfig/default.substvar"));
        }

        [Test]
        public void Should_Retrieve_Filename_from_path()
        {
            Assert.AreEqual("MarketConfig", parser.ParseFileName("myProject/Config/MarketConfig/default.substvar"));
        }
    }
}

