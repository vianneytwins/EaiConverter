using System;
using NUnit.Framework;
using System.Xml.Linq;
using EaiConverter.Parser.Utils;

namespace EaiConverter
{
	[TestFixture]
	public class XElementParserUtilsTest
	{

		[Test]
		public void Should_Return_a_string_Value_When_Element_is_not_null_And_Has_a_Value()
		{
			var element = new XElement ("Test");
			element.Value = "TestValue";
			var actual = XElementParserUtils.GetStringValue (element);

			Assert.AreEqual ("TestValue", actual);
		} 

		[Test]
		public void Should_Return_null_If_Element_is_null()
		{
			var actual = XElementParserUtils.GetStringValue (null);

			Assert.IsNull (actual);
		} 

		[Test]
		public void Should_Return_null_When_GetStringValue_is_Called_With_a_Null_Element()
		{
			var element = new XElement ("Test");
			var actual = XElementParserUtils.GetStringValue (element);

			Assert.AreEqual (null, actual);
		}

		[Test]
		public void Should_Return_an_int_Value_When_Element_is_not_null_And_Has_a_Value()
		{
			var element = new XElement ("Test");
			element.Value = "1";
			var actual = XElementParserUtils.GetIntValue (element);

			Assert.AreEqual (1, actual);
		} 

		[Test]
		public void Should_Return_null_When_GetIntValue_is_Called_With_a_Null_Element()
		{
			var actual = XElementParserUtils.GetIntValue (null);

			Assert.AreEqual (null, actual);
		} 

		[Test]
		public void Should_Return_null_If_Element_is_not_Null_And_Value_Null()
		{
			var element = new XElement ("Test");
			var actual = XElementParserUtils.GetIntValue (element);

			Assert.AreEqual (null, actual);
		}

		[Test]
		public void Should_Return_true_Value_When_Element_is_not_null_And_Has_a_Value_true()
		{
			var element = new XElement ("Test");
			element.Value = "True";
			var actual = XElementParserUtils.GetBoolValue (element);

			Assert.AreEqual(true, actual);
		} 

		[Test]
		public void Should_Return_null_If_bool_Element_is_null()
		{
			var actual = XElementParserUtils.GetBoolValue (null);

			Assert.IsNull(actual);
		} 

		[Test]
		public void Should_Return_null_When_GetBooleanValue_is_Called_With_a_Element_Not_Null_But_With_No_Value()
		{
			var element = new XElement ("Test");
			var actual = XElementParserUtils.GetBoolValue (element);

			Assert.AreEqual(null, actual);
		}

	}
}

