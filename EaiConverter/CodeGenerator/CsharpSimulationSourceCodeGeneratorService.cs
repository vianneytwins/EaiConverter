namespace EaiConverter.CodeGenerator
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.IO;

    public class CsharpSimulationSourceCodeGeneratorService : ISourceCodeGeneratorService
    {
        public void Generate(CodeCompileUnit targetUnit)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            string classesInString;
            using (var writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(targetUnit, writer, new CodeGeneratorOptions());
                classesInString = writer.GetStringBuilder().ToString();
            }

            Console.WriteLine(classesInString);
        }

        public void Init()
        {
            // Do nothing
        }
    }
}
