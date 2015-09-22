namespace EaiConverterTest.Parser
{
    using System.ComponentModel.Design;
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser;

    using NUnit.Framework;

    [TestFixture]
    public class ConfigurationParserTest
    {
        private ConfigurationParser configurationParser;

        private string xml;

        private TbwConfiguration config;

        [SetUp]
        public void SetUp()
        {
            this.configurationParser = new ConfigurationParser();

            this.xml = @"<?xml version=""1.0"" encoding=""UTF-8"" ?> 
<application xmlns=""http://www.tibco.com/xmlns/ApplicationManagement"" name=""PNOEXP/tibcoae-pno_monthly"">
	<repoInstanceName>%%DOMAIN%%-tibcoae-pno_monthly</repoInstanceName> 
	<contact>Marius STAN</contact> 
    <services>
		<adapter name=""IFSAdapter.aar"">
			<enabled>false</enabled> 
        </adapter>
        <bw name=""Panorama.par"">
			<enabled>true</enabled>
            <bwprocesses>
				<bwprocess name=""Process/DAI/PNO/Monitoring/PrMonitoring.IFS.FileInput.process"">
					<starter>PubInstrument Subscriber</starter> 
					<enabled>true</enabled> 
					<maxJob>1</maxJob> 
					<activation>true</activation> 
					<flowLimit>1</flowLimit> 
				</bwprocess>
				<bwprocess name=""Process/DAI/PNO/Routing/EVLOTC/ProcessTRS.process"">
					<starter>Timer</starter> 
					<enabled>false</enabled> 
					<maxJob>0</maxJob> 
					<activation>true</activation> 
					<flowLimit>0</flowLimit> 
				</bwprocess>
            </bwprocesses>
        </bw>
	</services>
</application>
";
            config = this.configurationParser.Parse(XElement.Parse(this.xml));
        }

        [Test]
        public void Should_return_2_bw_process_in_the_first_container()
        {
            Assert.AreEqual(2, this.config.ServicesConfig.TbwProcessContainers[0].TbwProcessConfigs.Count);
        }

        [Test]
        public void Should_return_bw_process()
        {
            
            Assert.AreEqual("Process/DAI/PNO/Monitoring/PrMonitoring.IFS.FileInput.process", this.config.ServicesConfig.TbwProcessContainers[0].TbwProcessConfigs[0].Name);
            Assert.AreEqual(1, this.config.ServicesConfig.TbwProcessContainers[0].TbwProcessConfigs[0].MaxJob);
            Assert.AreEqual(1, this.config.ServicesConfig.TbwProcessContainers[0].TbwProcessConfigs[0].FlowLimit);
            Assert.IsTrue(this.config.ServicesConfig.TbwProcessContainers[0].TbwProcessConfigs[0].IsEnabled == true);
            Assert.IsTrue(this.config.ServicesConfig.TbwProcessContainers[0].TbwProcessConfigs[0].Activation == true);
        }
    }

}
