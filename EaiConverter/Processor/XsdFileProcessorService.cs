namespace EaiConverter.Processor
{
    using System;
    using System.CodeDom;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;
    using EaiConverter.Parser;

    using log4net;

    public class XsdFileProcessorService : IFileProcessorService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(XsdFileProcessorService));

        private readonly ISourceCodeGeneratorService sourceCodeGeneratorService;

        private XsdParser xsdParser;

        public XsdFileProcessorService(ISourceCodeGeneratorService sourceCodeGeneratorService)
        {
            this.sourceCodeGeneratorService = sourceCodeGeneratorService;
            this.xsdParser = new XsdParser();
        }

        public void Process(string fileName)
        {
            CodeNamespace xsdNameSpacetoGenerate = new CodeNamespace();
            try
            {
                xsdNameSpacetoGenerate = new XsdBuilder().Build(fileName);
            }
            catch (Exception e)
            {
                Log.Error("unable to generate class from XSD file:" + fileName, e);
            }

            var targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(xsdNameSpacetoGenerate);

            this.sourceCodeGeneratorService.Generate(targetUnit);
        }
    }
}
