namespace EaiConverter.Processor
{
    using System;
    using System.IO;

    using log4net;

    public class TibcoBWDirectoryProcessorService : IDirectoryProcessorService
	{
        private static readonly ILog Log = LogManager.GetLogger(typeof(TibcoBWDirectoryProcessorService));

        private readonly IFileProcessorService tibcoFileProcessorService;
        private readonly IFileProcessorService xsdFileProcessorService;
        private readonly IFileProcessorService globalVariableProcessor;

        private readonly IFileProcessorService adapterSchemaProcessor;

        private readonly IFileFilter filter;

        public TibcoBWDirectoryProcessorService(IFileProcessorService tibcoFileProcessorService, IFileProcessorService xsdFileProcessorService, IFileProcessorService globalVariableProcessor, IFileProcessorService adapterSchemaProcessor, IFileFilter fileFilter)
        {
			this.tibcoFileProcessorService = tibcoFileProcessorService;
            this.xsdFileProcessorService = xsdFileProcessorService;
            this.globalVariableProcessor = globalVariableProcessor;
            this.adapterSchemaProcessor = adapterSchemaProcessor;
            this.filter = fileFilter;
        }

		public void Process(string directory)
        { 
			string[] files; 
			string[] directories; 

			try 
			{ 
				files = Directory.GetFiles(directory); 
				directories = Directory.GetDirectories(directory); 
			} 
			catch(Exception e) 
			{ 
                Log.Error("Unknow Error I'm going to quit because: ", e);
				return; 
			} 

			//Directory treatement
			foreach(string file in files) 
			{
			    if (file.EndsWith(".process"))
			    {
			        if (this.filter.IsFileAuthorized(file))
			        {
			            //file treatement
			            this.tibcoFileProcessorService.Process(file);
			        }
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
                if (file.EndsWith(".aeschema"))
                {
                    //file treatement
                    this.adapterSchemaProcessor.Process(file);
                }
			} 

			foreach(string subDirectory in directories) 
			{ 
				this.Process(Path.Combine(directory, subDirectory)); 
			} 
		}
	}

}
