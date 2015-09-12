using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Test.Utils;

namespace EaiConverter.Test.Builder
{
    [TestFixture]
    public class XsdBuilderTest
    {
		XsdBuilder xsdBuilder = new XsdBuilder();

		[Ignore]
		[Test]
		public void Should_return_enum_value()
		{
			var codenamespace = this.xsdBuilder.Build("./../../Ressources/Account.xsd");
			var code = TestCodeGeneratorUtils.GenerateCode (codenamespace);
			Assert.AreEqual ("", code);
		}
    }
}

