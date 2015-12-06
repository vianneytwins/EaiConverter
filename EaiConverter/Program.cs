namespace EaiConverter
{
    using System;

    using EaiConverter.CodeGenerator;
    using EaiConverter.Processor;

    using log4net;
    using log4net.Config;

    public class MainClass
	{
        public const string ProjectDirectory = "ProjectDirectory";

        private static readonly ILog Log = LogManager.GetLogger(typeof(MainClass));

        public static void Main(string[] args)
		{
            BasicConfigurator.Configure();

			IDirectoryProcessorService tibcoFileReaderService;
			IFileProcessorService tibcoFileProcessorService;
            IFileProcessorService xsdFileProcessorService;
            IFileProcessorService globalVariableProcessor;
            IFileProcessorService adapterFileProcessorService;
			ISourceCodeGeneratorService sourceCodeGeneratorService;
			IFileFilter fileFilter;

			if (args.Length > 1)
            {
				var sourceDirectory = args[0];
				var mode = args[1];
                string initFilePath = string.Empty;

                Log.Info("You've inputed DIRECTORY: " + sourceDirectory);
                Log.Info("You've inputed MODE: " + mode);
                
                if (args.Length > 2)
                {
                    initFilePath = args[2];
                    Log.Info("You've inputed FILTERING FILE: " + initFilePath);
                }
                
                fileFilter = new FileFilter(initFilePath);
                ConfigurationApp.SaveProperty(ProjectDirectory, sourceDirectory);

				if (mode == "S_Csharp")
                {
					sourceCodeGeneratorService = new CsharpSimulationSourceCodeGeneratorService ();
					tibcoFileProcessorService = new TibcoFileProcessorService(sourceCodeGeneratorService);
                    xsdFileProcessorService = new XsdFileProcessorService(sourceCodeGeneratorService);
                    globalVariableProcessor = new GlobalVariableProcessor(sourceCodeGeneratorService);
                    adapterFileProcessorService = new AdapterFileProcessorService(sourceCodeGeneratorService);
                    tibcoFileReaderService = new TibcoBWDirectoryProcessorService(tibcoFileProcessorService, xsdFileProcessorService, globalVariableProcessor, adapterFileProcessorService, fileFilter);

					tibcoFileReaderService.Process(sourceDirectory);
				}
                else if (mode == "G_Csharp")
                {
					sourceCodeGeneratorService = new CsharpSourceCodeGeneratorService();
					tibcoFileProcessorService = new TibcoFileProcessorService(sourceCodeGeneratorService);
                    xsdFileProcessorService = new XsdFileProcessorService(sourceCodeGeneratorService);
                    globalVariableProcessor = new GlobalVariableProcessor(sourceCodeGeneratorService);
                    adapterFileProcessorService = new AdapterFileProcessorService(sourceCodeGeneratorService);
                    tibcoFileReaderService = new TibcoBWDirectoryProcessorService(tibcoFileProcessorService, xsdFileProcessorService, globalVariableProcessor, adapterFileProcessorService, fileFilter);
               
					tibcoFileReaderService.Process(sourceDirectory);
				}
                else if (mode == "A")
                {
                    var tibcoDependencyAnalyserProcessorService = new TibcoDependencyAnalyserProcessorService(new AnalyserFileProcessorService());
                    ConfigurationApp.SaveProperty(ProjectDirectory, sourceDirectory);

                    var processToAnalyseFileName = args[2];

                    tibcoDependencyAnalyserProcessorService.Process(processToAnalyseFileName);
                }
                else
                {
                    Log.Error("Program is going to exit - sorry only MODE S_Csharp, G_Csharp and A are managed for the moment");
				}

			}
			else
            {
				DisplayErrorMessage ();
				return;
			}
                
		    Console.ReadLine();
		}

		static void DisplayErrorMessage ()
		{
            Log.Error("Please specify a correct usage : EaiConverter.exe DIRECTORY MODE");
            Log.Error("exemple of usage : EaiConverter.exe ../../my_tibco_bw_project_directory S_Csharp");
            Log.Error("Possible MODE are : ");
            Log.Error("A - for Analysis");
            Log.Error("S_Csharp - for Simulation in C_Sharp");
            Log.Error("G_Csharp - for Generation of the target source file in C_Sharp");

            Console.ReadLine();
		}
	}
}
