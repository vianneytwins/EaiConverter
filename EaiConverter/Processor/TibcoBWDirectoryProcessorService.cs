using System;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace EaiConverter.Processor
{
	public class TibcoBWDirectoryProcessorService : ITibcoBWDirectoryProcessorService
	{
		ITibcoFileProcessorService tibcoFileProcessorService;

		public TibcoBWDirectoryProcessorService (ITibcoFileProcessorService tibcoFileProcessorService){
			this.tibcoFileProcessorService = tibcoFileProcessorService;
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
			} 
			foreach(string subDirectory in directories) 
			{ 
				Process(Path.Combine(directory, subDirectory)); 
			} 
		}
	}

}
