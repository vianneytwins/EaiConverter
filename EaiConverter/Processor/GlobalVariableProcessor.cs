namespace EaiConverter
{
    using System.CodeDom;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;
    using EaiConverter.Parser;
    using EaiConverter.Processor;

    public class GlobalVariableProcessor : IFileProcessorService
    {
        ISourceCodeGeneratorService sourceCodeGeneratorService;

        public GlobalVariableProcessor(ISourceCodeGeneratorService sourceCodeGeneratorService)
        {
            this.sourceCodeGeneratorService = sourceCodeGeneratorService;
        }

        public void Process(string fileName)
        {
            //TODO : manage the input directory name to have the relative path = package name

            var globalVariableProcess = new GlobalVariableParser().ParseVariable(fileName);
            var globalVariableBuilder = new GlobalVariableBuilder();
            var targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(globalVariableBuilder.Build(globalVariableProcess));

            this.sourceCodeGeneratorService.Generate(targetUnit);
        }
    }
}
