using System;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace EaiConverter.CodeGenerator
{
    public class CsharpSimulationSourceCodeGeneratorService : ISourceCodeGeneratorService
    {
        public void Generate(CodeCompileUnit targetUnit)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            string classesInString;
            using (StringWriter writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, writer, new CodeGeneratorOptions());
                classesInString = writer.GetStringBuilder().ToString();
            }
            Console.WriteLine(classesInString);
        }

        public void GenerateSolutionAndProjectFiles()
        {
            // Do nothing
        }
    }
}
