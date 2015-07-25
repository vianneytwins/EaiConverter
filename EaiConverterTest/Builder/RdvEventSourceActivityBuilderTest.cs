using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using EaiConverter.Test.Utils;
using EaiConverter.Processor;

namespace EaiConverter.Test.Builder
{
	[TestFixture]
	public class RdvEventSourceActivityBuilderTest
	{
		RdvEventSourceActivityBuilder activityBuilder;

		RdvEventSourceActivity activity;

		[SetUp]
		public void SetUp ()
		{
			this.activityBuilder = new RdvEventSourceActivityBuilder();
			this.activity = new RdvEventSourceActivity( "My Activity Name", ActivityType.rdvEventSourceActivityType);

			this.activity.Parameters = new List<ClassParameter>{
				new ClassParameter{
					Name = "message",
					Type= "System.String"}
			};
		}

		[Test]
		public void Should_return_2_imports_For_calling_process ()
		{
			var imports = this.activityBuilder.GenerateImports (activity);
			Assert.AreEqual (2, imports.Count);
		}

		[Test]
		public void Should_return_One_ConstructorParameter_named_subscriber ()
		{
			var paramaters = this.activityBuilder.GenerateConstructorParameter (activity);
			Assert.AreEqual (1, paramaters.Count);
			Assert.AreEqual ("subscriber", paramaters[0].Name);
		}

		[Test]
		public void Should_return_One_field_named_subscriber ()
		{
			var fields = this.activityBuilder.GenerateFields(activity);
			Assert.AreEqual (1, fields.Count);
			Assert.AreEqual ("subscriber", fields[0].Name);
		}


		[Test]
		public void Should_return_construstor_statements ()
		{
			var expected = @"this.subscriber = subscriber;
this.subscriber.ResponseReceived += this.OnEvent;
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GenerateConstructorCodeStatement(activity));
			Assert.AreEqual(expected,generatedCode);
		}

		[Test]
		public void Should_return_2_Classes_To_Generate_if_they_are_not_already_generated ()
		{
			ConfigurationApp.SaveProperty("IsSubscriberInterfaceAlreadyGenerated", "false");
			ConfigurationApp.SaveProperty("IsTibcoSubscriberImplemAlreadyGenerated", "false");

			var namespaces = this.activityBuilder.GenerateClassesToGenerate(activity);
			Assert.AreEqual (3, namespaces.Count);
		}

		[Test]
		public void Should_return_1_Classe_To_Generate_if_they_are_not_already_generated ()
		{
			ConfigurationApp.SaveProperty("IsSubscriberInterfaceAlreadyGenerated", "true");
			ConfigurationApp.SaveProperty("IsTibcoSubscriberImplemAlreadyGenerated", "false");

			var namespaces = this.activityBuilder.GenerateClassesToGenerate(activity);
			Assert.AreEqual (1, namespaces.Count);
		}

		[Test]
		public void Should_generate_SubscriberInterface ()
		{
			var expected = @"namespace MyApp.Tools.TibcoRdv
{
    using System;
    
    
    public interface ISubscriber
    {
        
        int WaitingTimeLimit
        {
            get;
        }
        
        bool IsStarted
        {
            get;
        }
        
        private event ResponseReceivedEventHandler ResponseReceived;
        
        void Start();
        
        void Stop();
    }
}
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GenerateSubscriberInterface());
			Assert.AreEqual(expected, generatedCode);
		}


	}
}

