namespace EaiConverter.Processor
{
    using System;
    using System.CodeDom;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;
    using EaiConverter.Parser;

    public class AdapterFileProcessorService : IFileProcessorService
    {
        private ISourceCodeGeneratorService sourceCodeGeneratorService;

        public AdapterFileProcessorService(ISourceCodeGeneratorService sourceCodeGeneratorService)
        {
            this.sourceCodeGeneratorService = sourceCodeGeneratorService;
        }

        public void Process(string fileName)
        {
            // TODO VC TO implement
            var schemaModel = new AdapterSchemaParser().Parse(fileName);
            var adapterSchemaBuilder = new AdapterSchemaBuilder();
            var targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(adapterSchemaBuilder.Build(schemaModel));

            this.sourceCodeGeneratorService.Generate(targetUnit);
        }
    }
}
