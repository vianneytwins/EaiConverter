using System;

using EaiConverter.CodeGenerator;
using EaiConverter.Processor;
using log4net;
using log4net.Config;

namespace EaiConverter
{
	public class MainClass
	{
        private static readonly ILog log = LogManager.GetLogger(typeof(MainClass));

        public const string ProjectDirectory = "ProjectDirectory";

		public static void Main(string[] args)
		{
            BasicConfigurator.Configure();

			ITibcoBWDirectoryProcessorService tibcoFileReaderService;
			IFileProcessorService tibcoFileProcessorService;
            IFileProcessorService xsdFileProcessorService;
            IFileProcessorService globalVariableProcessor;
			ISourceCodeGeneratorService sourceCodeGeneratorService;

			if (args.Length > 1){
				var sourceDirectory = args [0];
				var mode = args [1];
                log.Info("You've inputed DIRECTORY: " + sourceDirectory);
                log.Info("You've inputed MODE: " + mode);

				if (mode == "S_Csharp") {
					sourceCodeGeneratorService = new CsharpSimulationSourceCodeGeneratorService ();
					tibcoFileProcessorService = new TibcoFileProcessorService(sourceCodeGeneratorService);
                    xsdFileProcessorService = new XsdFileProcessorService(sourceCodeGeneratorService);
                    globalVariableProcessor = new GlobalVariableProcessor(sourceCodeGeneratorService);
                    tibcoFileReaderService = new TibcoBWDirectoryProcessorService(tibcoFileProcessorService, xsdFileProcessorService, globalVariableProcessor);
                    ConfigurationApp.SaveProperty(ProjectDirectory, sourceDirectory);
					tibcoFileReaderService.Process(sourceDirectory);
				} else if (mode == "G_Csharp") {
					sourceCodeGeneratorService = new CsharpSourceCodeGeneratorService();
					tibcoFileProcessorService = new TibcoFileProcessorService(sourceCodeGeneratorService);
                    xsdFileProcessorService = new XsdFileProcessorService(sourceCodeGeneratorService);
                    globalVariableProcessor = new GlobalVariableProcessor(sourceCodeGeneratorService);
                    tibcoFileReaderService = new TibcoBWDirectoryProcessorService(tibcoFileProcessorService, xsdFileProcessorService, globalVariableProcessor);
                    ConfigurationApp.SaveProperty(ProjectDirectory, sourceDirectory);

				    sourceCodeGeneratorService.Init();
					tibcoFileReaderService.Process(sourceDirectory);
				} else {
                    log.Error("Program is going to exit - sorry only MODE S_Csharp and G_Csharp is managed for the moment");
				}

			}
			else {
				DisplayErrorMessage ();
				return;
			}
                
		    Console.ReadLine();
		}

		static void DisplayErrorMessage ()
		{
            log.Error("Please specify a correct usage : EaiConverter.exe DIRECTORY MODE");
            log.Error("exemple of usage : EaiConverter.exe ../../my_tibco_bw_project_directory S_Csharp");
            log.Error("Possible MODE are : ");
            log.Error("A - for Analysis");
            log.Error("S_Csharp - for Simulation in C_Sharp");
            log.Error("G_Csharp - for Generation of the target source file in C_Sharp");

            Console.ReadLine();
		}
	}
}
