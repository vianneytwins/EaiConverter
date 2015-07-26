using System;
using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Collections.Generic;
using EaiConverter.Test.Utils;
using EaiConverter.Processor;
using System.Xml.Linq;

namespace EaiConverter.Test.Builder
{
	[TestFixture]
	public class RdvPublishActivityBuilderTest
	{
		RdvPublishActivityBuilder activityBuilder;

		RdvPublishActivity activity;

		[SetUp]
		public void SetUp ()
		{
			this.activityBuilder = new RdvPublishActivityBuilder(new XslBuilder(new XpathBuilder()));
			this.activity = new RdvPublishActivity( "My Activity Name", ActivityType.rdvPubActivityType);
			this.activity.XsdString = "pfx:FileStatisticalData";
			var xml =
				@"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
    <ActivityInput>            
        <body>
            <xsl:value-of select=""'TestString'""/>
        </body>
    </ActivityInput>
</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);
			this.activity.InputBindings = doc.Nodes();
			this.activity.Parameters = new List<ClassParameter>{
				new ClassParameter{
					Name = "ActivityInput",
					Type= "ActivityInput"}
			};
		}


		[Test]
		public void Should_return_1_imports_For_calling_process ()
		{
			var imports = this.activityBuilder.GenerateImports (activity);
			Assert.AreEqual (1, imports.Count);
		}

		[Test]
		public void Should_return_One_ConstructorParameter_named_myActivityNameRdvPublisher ()
		{
			var paramaters = this.activityBuilder.GenerateConstructorParameter (activity);
			Assert.AreEqual (1, paramaters.Count);
			Assert.AreEqual ("myActivityNameRdvPublisher", paramaters[0].Name);
			Assert.AreEqual ("IPublisher", paramaters[0].Type.BaseType);
		}

		[Test]
		public void Should_return_One_field_named_myActivityNameRdvPublisher ()
		{
			var fields = this.activityBuilder.GenerateFields(activity);
			Assert.AreEqual (1, fields.Count);
			Assert.AreEqual ("myActivityNameRdvPublisher", fields[0].Name);
			Assert.AreEqual ("IPublisher", fields[0].Type.BaseType);
		}


		[Test]
		public void Should_return_construstor_statements ()
		{
			var expected = @"this.myActivityNameRdvPublisher = myActivityNameRdvPublisher;
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GenerateConstructorCodeStatement(activity));
			Assert.AreEqual(expected,generatedCode);
		}

		[Test]
		public void Should_return_2_Classes_To_Generate_if_they_are_not_already_generated ()
		{
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.isPublisherInterfaceAlreadyGenerated, "false");
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.isTibcoPublisherImplemAlreadyGenerated, "false");

			var namespaces = this.activityBuilder.GenerateClassesToGenerate(activity);
			Assert.AreEqual (2, namespaces.Count);
		}

		[Test]
		public void Should_return_1_Classe_To_Generate_if_they_are_not_already_generated ()
		{
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.isPublisherInterfaceAlreadyGenerated, "true");
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.isTibcoPublisherImplemAlreadyGenerated, "false");

			var namespaces = this.activityBuilder.GenerateClassesToGenerate(activity);
			Assert.AreEqual (1, namespaces.Count);
		}

		[Test]
		public void Should_generate_PublisherInterface ()
		{
			var expected = @"namespace MyApp.Tools.EventSourcing
{
    using System;
    
    
    public interface IPublisher
    {
        
        void Send(string message);
    }
}
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GeneratePublisherInterface());
			Assert.AreEqual(expected, generatedCode);
		}

		[Test]
		public void Should_return_invocation_Code()
		{
			var expected = @"this.logger.Info(""Start Activity: My Activity Name of type: com.plugin.tibrv.RVPubActivity"");
ActivityInput ActivityInput = new ActivityInput();
ActivityInput.body = ""TestString"";

this.myActivityNameRdvPublisher.Send(ActivityInput);
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GenerateInvocationCode(this.activity));
			Assert.AreEqual(expected, generatedCode);
		}

	}
}

