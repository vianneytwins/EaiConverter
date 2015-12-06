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
            Assert.IsTrue(File.Exists(CsharpSourceCodeGeneratorService.ProjectDestinationPath + "/testnamespace/subnamespace/TestClass.cs"));
        }

        [Test]
        public void Should_Create_2files_When_Namspace_Contains_2classes(){

            var targetUnit = new CodeCompileUnit();
            var testNamespace = new CodeNamespace("testnamespace.subnamespace");
            testNamespace.Types.Add(new CodeTypeDeclaration("Test1Class"));
            testNamespace.Types.Add(new CodeTypeDeclaration("Test2Class"));
            targetUnit.Namespaces.Add(testNamespace);
            this.csharpSourceCodeGenetatorService.Generate(targetUnit);
            Assert.IsTrue(File.Exists(CsharpSourceCodeGeneratorService.ProjectDestinationPath + "/testnamespace/subnamespace/Test1Class.cs"));
            Assert.IsTrue(File.Exists(CsharpSourceCodeGeneratorService.ProjectDestinationPath + "/testnamespace/subnamespace/Test2Class.cs"));
        }

    }

}

