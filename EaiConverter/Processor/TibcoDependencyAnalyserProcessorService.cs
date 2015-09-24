namespace EaiConverter.Processor
{
    using System;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator;
    using EaiConverter.Parser;

    public class TibcoDependencyAnalyserProcessorService 
	{
        private readonly AnalyserFileProcessorService analyserFileProcessorService;

        private ConfigurationParser configurationParser;

        public TibcoDependencyAnalyserProcessorService(AnalyserFileProcessorService analyserFileProcessorService)
        {
            this.analyserFileProcessorService = analyserFileProcessorService;
            this.configurationParser = new ConfigurationParser();
		}
        /// <summary>
        /// analyse the dependancy of your project via the XML application config file
        /// It's usefull when you don't know which process are used or not.
        /// What out !! TibcoBW aloow you to have dynamic process loading, in that case the analyser doesn't work
        /// </summary>
        /// <param name="processToAnalyseFileName">the path of the XML application configuration file</param>
        public void Process(string processToAnalyseFileName)
        {
            this.AnalyseViaTheTbwXmlConfigFile(processToAnalyseFileName);
            //this.AnalyseViaGivenListOfProcess(processToAnalyseFileName);
        }

        private void AnalyseViaTheTbwXmlConfigFile(string processToAnalyseFileName)
        {
            var config = this.configurationParser.Parse(processToAnalyseFileName);
            var projectDir = ConfigurationApp.GetProperty(MainClass.ProjectDirectory);
            foreach (var tbwProcessContainerConfig in config.ServicesConfig.TbwProcessContainers)
            {
                foreach (var processConfig in tbwProcessContainerConfig.TbwProcessConfigs)
                {
                    if (processConfig.IsEnabled)
                    {
                        this.analyserFileProcessorService.Process(projectDir + "/" + processConfig.Name);
                    }
                }
            }
        }

        private void AnalyseViaGivenListOfProcess(string processToAnalyseFileName)
        {
            string line;

            var file = new System.IO.StreamReader(processToAnalyseFileName);
            while ((line = file.ReadLine()) != null)
            {
                this.analyserFileProcessorService.Process(line);
            }

            file.Close();
        }
	}

}
