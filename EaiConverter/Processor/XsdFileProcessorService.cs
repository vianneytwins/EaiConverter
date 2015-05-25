using System;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;
using EaiConverter.CodeGenerator;
using EaiConverter.Processor;
using EaiConverter.Parser;
using EaiConverter.Mapper;

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
