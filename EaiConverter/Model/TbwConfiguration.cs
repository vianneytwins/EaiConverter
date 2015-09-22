namespace EaiConverter.Model
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.Design;

    public class TbwConfiguration
    {
        public string RepoInstanceName { get; set; }

        public TbwServiceConfig ServicesConfig { get; set; }

        public string Name { get; set; }
    }

    public class TbwServiceConfig
    {
        public List<TbwAdapterConfig> TbwAdapters { get; set; }

        public List<TbwProcessContainerConfig> TbwProcessContainers { get; set; }

    }

    public class TbwAdapterConfig
    {
        public string Name { get; set; }

        public bool? IsEnabled { get; set; }
    }

    public class TbwProcessContainerConfig
    {
        public string Name { get; set; }

        public bool? IsEnabled { get; set; }

        public List<TbwProcessConfig> TbwProcessConfigs { get; set; }
    }

    public class TbwProcessConfig
    {
        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        public bool Activation { get; set; }

        public int MaxJob { get; set; }

        public int FlowLimit { get; set; }


    }
}
