using System;

using EaiConverter.CodeGenerator;
using EaiConverter.Processor;

namespace EaiConverter
{
	public class MainClass
	{
        public const string ProjectDirectory = "ProjectDirectory";

		public static void Main(string[] args)
		{
			ITibcoBWDirectoryProcessorService tibcoFileReaderService;
			IFileProcessorService tibcoFileProcessorService;
            IFileProcessorService xsdFileProcessorService;
            IFileProcessorService globalVariableProcessor;
			ISourceCodeGeneratorService sourceCodeGeneratorService;

			if (args.Length > 1){
				var sourceDirectory = args [0];
				var mode = args [1];
				Console.WriteLine ("You've inputed DIRECTORY: " + sourceDirectory);
				Console.WriteLine ("You've inputed MODE: " + mode);

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

				    sourceCodeGeneratorService.GenerateSolutionAndProjectFiles();
					tibcoFileReaderService.Process(sourceDirectory);
				} else {
                    Console.WriteLine ("Program is going to exit - sorry only MODE S_Csharp and G_Csharp is managed for the moment");
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
			Console.WriteLine ("Please specify a correct usage : EaiConverter.exe DIRECTORY MODE");
            Console.WriteLine ("exemple of usage : EaiConverter.exe ../../my_tibco_bw_project_directory S_Csharp");
			Console.WriteLine ("Possible MODE are : ");
			Console.WriteLine ("A - for Analysis");
			Console.WriteLine ("S_Csharp - for Simulation in C_Sharp");
			Console.WriteLine ("G_Csharp - for Generation of the target source file in C_Sharp");

            Console.ReadLine();
		}
	}
}
