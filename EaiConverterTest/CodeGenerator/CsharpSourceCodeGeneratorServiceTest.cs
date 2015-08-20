using NUnit.Framework;
using EaiConverter.CodeGenerator;
using System.CodeDom;
using System.IO;

namespace EaiConverter.Test.CodeGenerator
{
    [TestFixture]
    public class CsharpSourceCodeGeneratorServiceTest
    {
        
        private ISourceCodeGeneratorService csharpSourceCodeGenetatorService;

        [SetUp]
        public void SetUp(){
            this.csharpSourceCodeGenetatorService = new CsharpSourceCodeGeneratorService();
        }

        [TearDown]
        public void TearDown(){
            if (Directory.Exists(CsharpSourceCodeGeneratorService.SolutionDestinationPath))
            {
                Directory.Delete(CsharpSourceCodeGeneratorService.SolutionDestinationPath, true);
            }
        }

        [Test]
        public void Should_Create_file_When_final_generation(){

            var targetUnit = new CodeCompileUnit();
            var testNamespace = new CodeNamespace("testnamespace.subnamespace");
            var testClassToGenerate = new CodeTypeDeclaration("TestClass");
            testNamespace.Types.Add(testClassToGenerate);
            targetUnit.Namespaces.Add(testNamespace);
            this.csharpSourceCodeGenetatorService.Generate(targetUnit);
            Assert.IsTrue(File.Exists(CsharpSourceCodeGeneratorService.ProjectDestinationPath + "/testnamespace/subnamespace" + "/TestClass.cs"));
        }


    }

}

