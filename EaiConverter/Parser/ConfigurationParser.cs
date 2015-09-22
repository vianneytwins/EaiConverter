namespace EaiConverter.Parser
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser.Utils;

    public class ConfigurationParser 
    {
        public TbwConfiguration Parse(string filePathName)
        {
            XElement allFileElement = XElement.Load(filePathName);
            return this.Parse(allFileElement);
        }

        public TbwConfiguration Parse(XElement inputElement)
        {
            var configuration = new TbwConfiguration();
            var applicationElement = inputElement.Element(XmlnsConstant.ConfigNamespace + "application");
            configuration.Name = inputElement.Attribute("name").Value;
            configuration.RepoInstanceName = inputElement.Element(XmlnsConstant.ConfigNamespace + "repoInstanceName").Value;

            var servicesElement = inputElement.Element(XmlnsConstant.ConfigNamespace + "services");

            configuration.ServicesConfig = new TbwServiceConfig();
            if (servicesElement.Elements(XmlnsConstant.ConfigNamespace + "adapter") != null)
            {
                configuration.ServicesConfig.TbwAdapters = this.GetTbwAdapters(servicesElement.Elements(XmlnsConstant.ConfigNamespace + "adapter"));
            }

            if (servicesElement.Elements(XmlnsConstant.ConfigNamespace + "bw") != null)
            {
                configuration.ServicesConfig.TbwProcessContainers = this.TbwProcessContainers(servicesElement.Elements(XmlnsConstant.ConfigNamespace + "bw"));
            }

            return configuration;
        }

        private List<TbwProcessContainerConfig> TbwProcessContainers(IEnumerable<XElement> elements)
        {
            var tbwProcessContainerConfigs = new List<TbwProcessContainerConfig>();
            foreach (var bwElement in elements)
            {
                tbwProcessContainerConfigs.Add(this.GetTbxProcessConfig(bwElement));
            }
            return tbwProcessContainerConfigs;
        }

        private TbwProcessContainerConfig GetTbxProcessConfig(XElement xElement)
        {
            var container = new TbwProcessContainerConfig();
            container.Name = xElement.Attribute("name").Value;
            container.IsEnabled = XElementParserUtils.GetBoolValue(xElement.Element(XmlnsConstant.ConfigNamespace + "enabled"));
            var processesElements = xElement.Element(XmlnsConstant.ConfigNamespace + "bwprocesses").Elements(XmlnsConstant.ConfigNamespace + "bwprocess");
            container.TbwProcessConfigs = new List<TbwProcessConfig>();
            foreach (var processesElement in processesElements)
            {
                container.TbwProcessConfigs.Add(new TbwProcessConfig
                                                    {
                                                        Name = processesElement.Attribute("name").Value,
                                                        IsEnabled = (bool)XElementParserUtils.GetBoolValue(processesElement.Element(XmlnsConstant.ConfigNamespace + "enabled")),
                                                        Activation = (bool)XElementParserUtils.GetBoolValue(processesElement.Element(XmlnsConstant.ConfigNamespace + "activation")),
                                                        MaxJob = (int)XElementParserUtils.GetIntValue(processesElement.Element(XmlnsConstant.ConfigNamespace + "maxJob")),
                                                        FlowLimit = (int)XElementParserUtils.GetIntValue(processesElement.Element(XmlnsConstant.ConfigNamespace + "flowLimit"))
                                                    });
            }
            
            return container;
        }

        private List<TbwAdapterConfig> GetTbwAdapters(IEnumerable<XElement> elements)
        {
            var tbwAdapterConfigs = new List<TbwAdapterConfig>();
            return tbwAdapterConfigs;
        }
    }
}