using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Test.Utils;
using EaiConverter.Model;
using System.Collections.Generic;

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

		[Test]
		public void Should_Generate_Classes_from_Parameters()
		{
			var expected = @"namespace My.Namespace
{
    
    
    public class exception
    {
        
        public processData processData { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
";
			var parameters = new List<ClassParameter>();
			var parameter = new ClassParameter
			{
				Name = "exception",
				Type = "exception",
				ChildProperties = new List<ClassParameter>
				{
					new ClassParameter
					{
						Name = "processData",
						Type = "processData"
					},

					new ClassParameter
					{
						Name = "ExceptionMessage",
						Type = "string"
					}
				}
			};
			parameters.Add (parameter);
			var codenamespace = this.xsdBuilder.Build(parameters, "My.Namespace");
			var code = TestCodeGeneratorUtils.GenerateCode (codenamespace);
			Assert.AreEqual (expected, code);
		}
    }
}

