using System.CodeDom;
using EaiConverter.CodeGenerator;
using EaiConverter.Builder;

namespace EaiConverter.Processor
{
    public class XsdFileProcessorService : IFileProcessorService
	{
        ISourceCodeGeneratorService sourceCodeGeneratorService;

        public XsdFileProcessorService (ISourceCodeGeneratorService sourceCodeGeneratorService){
            this.sourceCodeGeneratorService = sourceCodeGeneratorService;
        }

        public void Process (string fileName)
        {
            var xsdNameSpacetoGenerate = new XsdBuilder ().Build (fileName);

            var targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(xsdNameSpacetoGenerate);

            this.sourceCodeGeneratorService.Generate (targetUnit);
        }
	}

}
