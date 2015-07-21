namespace EaiConverter.Processor
{
    using System;
    using System.CodeDom;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;

    public class XsdFileProcessorService : IFileProcessorService
    {
        private ISourceCodeGeneratorService sourceCodeGeneratorService;

        public XsdFileProcessorService(ISourceCodeGeneratorService sourceCodeGeneratorService)
        {
            this.sourceCodeGeneratorService = sourceCodeGeneratorService;
        }

        public void Process(string fileName)
        {
            try
            {
                var xsdNameSpacetoGenerate = new XsdBuilder().Build(fileName);

                var targetUnit = new CodeCompileUnit();
                targetUnit.Namespaces.Add(xsdNameSpacetoGenerate);

                this.sourceCodeGeneratorService.Generate(targetUnit);
            }
            catch (Exception e)
            {
                Console.WriteLine("############### ERROR####### unable to generate class from XSD file:" + fileName);
                Console.WriteLine(e);
            }
        }
    }
}
