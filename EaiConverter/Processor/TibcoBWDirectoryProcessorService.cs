using System;
using System.IO;

namespace EaiConverter.Processor
{
	public class TibcoBWDirectoryProcessorService : ITibcoBWDirectoryProcessorService
	{
		IFileProcessorService tibcoFileProcessorService;
        IFileProcessorService xsdFileProcessorService;

        public TibcoBWDirectoryProcessorService (IFileProcessorService tibcoFileProcessorService, IFileProcessorService xsdFileProcessorService){
			this.tibcoFileProcessorService = tibcoFileProcessorService;
            this.xsdFileProcessorService = xsdFileProcessorService;
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
				Console.WriteLine ("Unknow Error I'm going to quit because: ");
				Console.WriteLine (e.Message);
				Console.WriteLine (e.StackTrace);
				return; 
			} 

			//Directory treatement
			foreach(string file in files) 
			{ 
				if (file.EndsWith (".process"))
				{
					//file treatement
					this.tibcoFileProcessorService.Process (file);
				}
                if (file.EndsWith (".xsd"))
                {
                    //file treatement
                    this.xsdFileProcessorService.Process (file);
                }
			} 
			foreach(string subDirectory in directories) 
			{ 
				Process(Path.Combine(directory, subDirectory)); 
			} 
		}
	}

}
