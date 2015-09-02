namespace EaiConverter.Processor
{
    using System;
    using System.CodeDom;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;

    using log4net;

    public class XsdFileProcessorService : IFileProcessorService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(XsdFileProcessorService));

        private readonly ISourceCodeGeneratorService sourceCodeGeneratorService;

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
                Log.Error("unable to generate class from XSD file:" + fileName, e);
            }
        }
    }
}
