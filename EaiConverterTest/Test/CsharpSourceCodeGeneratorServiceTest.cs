using System;
using NUnit.Framework;
using EaiConverter.CodeGenerator;
using System.CodeDom;

namespace EaiConverterTest
{
    [TestFixture]
    public class CsharpSourceCodeGeneratorServiceTest
    {
        string directory ="./";

        private ISourceCodeGeneratorService csharpSourceCodeGenetatorService;

        [Test]
        public void Should_Create_file_When_final_generation(){

            var targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(new CodeNamespace("testnamespace"));
            //this.csharpSourceCodeGenetatorService.Generate(targetUnit);
            //Assert.AreEqual("testnamespace.cs",System.IO.Directory.GetFiles("./"));
        }

        [SetUp]
        public void SetUp(){
            this.csharpSourceCodeGenetatorService = new CsharpSourceCodeGeneratorService();
        }
        [TearDown]
        public void TearDown(){
            System.IO.File.Delete("./testnamespace.cs");

        }

    }

}

