using System;
using System.IO;
using log4net;

namespace EaiConverter.Processor
{
	public class TibcoBWDirectoryProcessorService : ITibcoBWDirectoryProcessorService
	{
        private static readonly ILog log = LogManager.GetLogger(typeof(TibcoBWDirectoryProcessorService));

		IFileProcessorService tibcoFileProcessorService;
        IFileProcessorService xsdFileProcessorService;
        IFileProcessorService globalVariableProcessor;

        public TibcoBWDirectoryProcessorService (IFileProcessorService tibcoFileProcessorService, IFileProcessorService xsdFileProcessorService, IFileProcessorService globalVariableProcessor){
			this.tibcoFileProcessorService = tibcoFileProcessorService;
            this.xsdFileProcessorService = xsdFileProcessorService;
            this.globalVariableProcessor = globalVariableProcessor;
		}

		public void Process(string directory) { 
			string[] files; 
			string[] directories; 

			try 
			{ 
				files = System.IO.Directory.GetFiles(directory); 
				directories = System.IO.Directory.GetDirectories(directory); 
			} 
			catch(Exception e) 
			{ 
                log.Error("Unknow Error I'm going to quit because: ", e);
				return; 
			} 

			//Directory treatement
			foreach(string file in files) 
			{ 
				if (file.EndsWith(".process"))
				{
					//file treatement
					this.tibcoFileProcessorService.Process(file);
				}
                if (file.EndsWith(".xsd"))
                {
                    //file treatement
                    this.xsdFileProcessorService.Process(file);
                }
                if (file.EndsWith(".substvar"))
                {
                    //file treatement
                    this.globalVariableProcessor.Process(file);
                }
			} 
			foreach(string subDirectory in directories) 
			{ 
				Process(Path.Combine(directory, subDirectory)); 
			} 
		}
	}

}
