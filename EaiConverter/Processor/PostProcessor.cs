namespace EaiConverter.Processor
{
    using System.CodeDom;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;

    public class PostProcessor
    {
        private ISourceCodeGeneratorService sourceCodeGeneratorService;

        private ModuleBuilder moduleBuilder;

        private ServiceManagerInterfaceBuilder interfaceBuilder;

        public PostProcessor(ISourceCodeGeneratorService sourceCodeGeneratorService)
        {
            this.sourceCodeGeneratorService = sourceCodeGeneratorService;
            this.moduleBuilder = new ModuleBuilder();
            this.interfaceBuilder = new ServiceManagerInterfaceBuilder();
        }

        public void Process()
        {
            var targetUnit = new CodeCompileUnit();
            targetUnit.Namespaces.Add(this.moduleBuilder.Build());
            targetUnit.Namespaces.Add(this.interfaceBuilder.Build());
            
            this.sourceCodeGeneratorService.Generate(targetUnit);
        }
    }
}
