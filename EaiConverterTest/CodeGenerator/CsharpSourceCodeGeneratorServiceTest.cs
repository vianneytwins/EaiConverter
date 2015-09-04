namespace EaiConverter.Test.CodeGenerator
{
    using System.CodeDom;
    using System.IO;

    using EaiConverter.CodeGenerator;

    using NUnit.Framework;

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

