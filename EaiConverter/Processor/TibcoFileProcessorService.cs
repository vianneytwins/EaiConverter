namespace EaiConverter.Processor
{
    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;
    using EaiConverter.Parser;

    public class TibcoFileProcessorService : IFileProcessorService
	{
        private readonly ISourceCodeGeneratorService sourceCodeGeneratorService;

		public TibcoFileProcessorService (ISourceCodeGeneratorService sourceCodeGeneratorService)
        {
			this.sourceCodeGeneratorService = sourceCodeGeneratorService;
		}

		public void Process (string fileName)
		{
			var tibcoBwProcess = new TibcoBWProcessLinqParser().Parse (fileName);
			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var targetUnit = tibcoBWProcessBuilder.Build (tibcoBwProcess);
	
			// TODO if exist don't add it ? Ugly but no Config manager on Mono/Xamarin
			if (ConfigurationApp.GetProperty ("IsLoggerAlreadyGenerated") != "true")
            {
				targetUnit.Namespaces.Add(new LoggerInterfaceBuilder ().Build ());
				ConfigurationApp.SaveProperty("IsLoggerAlreadyGenerated", "true");
			}
			
			this.sourceCodeGeneratorService.Generate (targetUnit);
		}
	}

}
