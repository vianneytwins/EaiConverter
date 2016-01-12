namespace EaiConverterTest.Builder
{
    using System;

    using EaiConverter.Builder;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class ModuleBuilderTest
    {
        ModuleBuilder moduleBuilder;

        [SetUp]
        public void SetUp()
        {
            this.moduleBuilder = new ModuleBuilder();
            ModuleBuilder.EmptyServiceToRegisterList();
            ModuleBuilder.AddServiceToRegister("ITruc", "Truc");
        }

        [Test]
        public void Should_Generate_Module()
        {
            string expected = string.Empty;
            if (Environment.OSVersion.ToString().Contains("indows"))
            {
                expected = @"using System;
using MyApp.Mydomain.Service.Contract;
using MyApp.Mydomain.DataAccess;
using MyApp.Mydomain.Service;
using MyApp.Tools.EngineCommand;
using MyApp.Tools.EventSourcing;
using MyApp.Tools.Logging;
using MyApp.Tools.Xml;


public class MyAppModule
{
    
    private IServiceManager serviceManager;
    
    public MyAppModule(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }
    
    public void RegisterServices()
    {
        this.serviceManager.RegisterApplicationService<ITruc, Truc>();
    }
}
";
            }
            else
            {
                expected = @"using System;
using MyApp.Mydomain.Service.Contract;
using MyApp.Mydomain.DataAccess;
using MyApp.Mydomain.Service;
using MyApp.Tools.EngineCommand;
using MyApp.Tools.EventSourcing;
using MyApp.Tools.Logging;
using MyApp.Tools.Xml;


public class MyAppModule
{
    
    private IServiceManager serviceManager;
    
    public MyAppModule(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }
    
    public void RegisterServices()
    {
        this.serviceManager.RegisterApplicationService <ITruc, Truc>();
    }
}
";
            }
            var actual = TestCodeGeneratorUtils.GenerateCode(this.moduleBuilder.Build()).RemoveWindowsReturnLineChar();
            Assert.AreEqual(expected.RemoveWindowsReturnLineChar(), actual);
        }
    }
}

