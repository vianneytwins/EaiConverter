namespace EaiConverter.Test.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
	public class RdvPublishActivityBuilderTest
	{
		RdvPublishActivityBuilder activityBuilder;

		RdvPublishActivity activity;

		[SetUp]
		public void SetUp()
		{
			this.activityBuilder = new RdvPublishActivityBuilder(new XslBuilder(new XpathBuilder()));
			this.activity = new RdvPublishActivity("My Activity Name", ActivityType.rdvPubActivityType);
			this.activity.XsdString = "pfx:FileStatisticalData";
			this.activity.Subject = "MY.Tibco.subject";
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
					Type = "ActivityInput"
                }
			};
		}


		[Test]
		public void Should_return_1_imports_For_calling_process ()
		{
			var imports = this.activityBuilder.GenerateImports (activity);
			Assert.AreEqual (1, imports.Count);
		}

		[Test]
		public void Should_return_One_ConstructorParameter_named_myActivityNameRdvPublisher()
		{
			var paramaters = this.activityBuilder.GenerateConstructorParameter (activity);
			Assert.AreEqual (1, paramaters.Count);
			Assert.AreEqual ("my_Activity_NameRdvPublisher", paramaters[0].Name);
			Assert.AreEqual ("IPublisher", paramaters[0].Type.BaseType);
		}

		[Test]
		public void Should_return_One_field_named_myActivityNameRdvPublisher ()
		{
			var fields = this.activityBuilder.GenerateFields(activity);
			Assert.AreEqual (1, fields.Count);
			Assert.AreEqual ("my_Activity_NameRdvPublisher", fields[0].Name);
			Assert.AreEqual ("IPublisher", fields[0].Type.BaseType);
		}


		[Test]
		public void Should_return_construstor_statements ()
		{
			var expected = @"this.my_Activity_NameRdvPublisher = my_Activity_NameRdvPublisher;
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GenerateConstructorCodeStatement(activity));
			Assert.AreEqual(expected,generatedCode);
		}

		[Test]
		public void Should_return_2_Classes_To_Generate_if_they_are_not_already_generated ()
		{
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.IsPublisherInterfaceAlreadyGenerated, "false");
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.IsTibcoPublisherImplemAlreadyGenerated, "false");

            var namespaces = this.activityBuilder.GenerateClassesToGenerate(activity, null);
			Assert.AreEqual (2, namespaces.Count);
		}

		[Test]
		public void Should_return_1_Classe_To_Generate_if_they_are_not_already_generated ()
		{
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.IsPublisherInterfaceAlreadyGenerated, "true");
			ConfigurationApp.SaveProperty(RdvPublishActivityBuilder.IsTibcoPublisherImplemAlreadyGenerated, "false");

            var namespaces = this.activityBuilder.GenerateClassesToGenerate(activity, null);
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
        
        void Send(string subject, string message);
    }
}
";
			var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GeneratePublisherInterface());
			Assert.AreEqual(expected, generatedCode);
		}

		[Test]
		public void Should_return_invocation_Code()
		{
			var expected = @"this.logger.Info(""Start Activity: My_Activity_Name of type: com.tibco.plugin.tibrv.RVPubActivity"");
ActivityInput ActivityInput = new ActivityInput();
ActivityInput.body = ""TestString"";

string subject = ""MY.Tibco.subject"";
this.my_Activity_NameRdvPublisher.Send(subject, ActivityInput);
";
            var generatedCode = TestCodeGeneratorUtils.GenerateCode(this.activityBuilder.GenerateMethods(this.activity, null)[0].Statements);
			Assert.AreEqual(expected, generatedCode);
		}

	}
}

