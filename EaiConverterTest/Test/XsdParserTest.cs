using System;
using NUnit.Framework;
using EaiConverter;
using EaiConverter.Parser;
using System.Xml.Linq;
using System.Text;
using System.Collections.Generic;

namespace EaiConverter
{
	[TestFixture]
	public class XsdParserTest
	{
		[Test]
		public void Should_return_One_element_with_Type_String ()
		{
			var xsdParser = new XsdParser();
			string xml = @"<xsd:element name=""adminID"" type=""xsd:string"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>";

			var doc = XElement.Parse(xml);
			var actual = xsdParser.Parse (new List<XNode>() {doc});

			Assert.AreEqual (1,actual.Count);
			Assert.AreEqual ("adminID",actual[0].Name);
			Assert.AreEqual ("string",actual[0].Type);
		}

		[Test]
		public void Should_return_One_element_with_One_Child ()
		{
			var xsdParser = new XsdParser();
			string xml = @"<xsd:element name=""group"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><xsd:complexType><xsd:sequence><xsd:element name=""adminID"" type=""xsd:string"" /></xsd:sequence></xsd:complexType></xsd:element>";

			var doc = XElement.Parse(xml);
			var actual = xsdParser.Parse (new List<XNode>() {doc});

			Assert.AreEqual (1, actual.Count);
			Assert.AreEqual ("group", actual[0].Name);
			Assert.AreEqual ("group", actual[0].Type);
			Assert.AreEqual ("adminID", actual[0].ChildProperties[0].Name);
			Assert.AreEqual ("string", actual[0].ChildProperties[0].Type);
		}

		[Test]
		public void Should_return_One_With_3_child_when_parsing_the_exemple ()
		{
			var xsdParser = new XsdParser();
			var doc = XElement.Load ("../../ressources/xsdtest.xsd");
			var actual = xsdParser.Parse (new List<XNode>() {doc});

			Assert.AreEqual (1, actual.Count);

			Assert.AreEqual ("group", actual[0].Name);
			Assert.AreEqual ("group", actual[0].Type);
			Assert.AreEqual (3, actual[0].ChildProperties.Count);
			Assert.AreEqual ("adminID", actual[0].ChildProperties[1].Name);
			Assert.AreEqual ("string", actual[0].ChildProperties[1].Type);
			Assert.AreEqual ("param", actual[0].ChildProperties[2].Name);
		}

	}

}

