namespace EaiConverter.Processor
{
    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;
    using EaiConverter.Parser;

    public class TibcoDependencyAnalyserProcessorService 
	{
        private readonly AnalyserFileProcessorService analyserFileProcessorService;

        public TibcoDependencyAnalyserProcessorService(AnalyserFileProcessorService analyserFileProcessorService)
        {
            this.analyserFileProcessorService = analyserFileProcessorService;
		}

        public void Process(string processToAnalyseFileName)
        {
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(processToAnalyseFileName);
            while((line = file.ReadLine()) != null)
            {
                this.analyserFileProcessorService.Process(line);
            }

            file.Close();
        }


	}

}
