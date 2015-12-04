namespace EaiConverter.Test.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
	public class TibcoProcessClassesBuilderTest
	{
	
		private TibcoBWProcess tibcoBwProcess;
        private TibcoProcessClassesBuilder tibcoBwProcessBuilder;


		[SetUp]
		public void SetUp()
		{
            this.tibcoBwProcessBuilder = new TibcoProcessClassesBuilder();

			this.tibcoBwProcess = new TibcoBWProcess("MyNamespace/myProcessTest.process");
			this.tibcoBwProcess.Activities = new List<Activity>();
		}

		[Test]
		public void Should_Return_simple_Constructor_When_NoActivies_are_declared()
		{
			var expected ="this.logger = logger;\n";
			var classToGenerate = this.tibcoBwProcessBuilder.Build(this.tibcoBwProcess);

            string classesInString = string.Empty;

			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members)
            {
				if (member is CodeConstructor)
                {
                    classesInString = TestCodeGeneratorUtils.GenerateCode(((CodeConstructor)member).Statements);
				}
			}

            Assert.AreEqual (expected, classesInString);
		}

		[Test]
		public void Should_Return_Constructor_with_setter_of_Activity_When_1_Activy_is_declared()
		{
            this.tibcoBwProcess.Activities.Add(new Activity ("MySqlRequestActivity", ActivityType.NotHandleYet));
            var expected = "this.logger = logger;\nthis.mySqlRequestActivityService = mySqlRequestActivityService;\n";
			
            var classToGenerate = this.tibcoBwProcessBuilder.Build (this.tibcoBwProcess);

            string classesInString = string.Empty;

            foreach (var member in classToGenerate.Namespaces [0].Types [0].Members)
            {
                if (member is CodeConstructor)
                {
                    classesInString = TestCodeGeneratorUtils.GenerateCode(((CodeConstructor)member).Statements);
                }
            }

			//classGenerator.GenerateMethod(classToGenerate.Constructors[0], stringBuilder, new TibcoBWConverter.CodeGenerator.utils.Tab(),false);
            Assert.AreEqual (expected, classesInString);
		}


		[Test]
		public void Should_Return_logger_as_a_privateField()
		{
			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder();
			var classToGenerate = tibcoBWProcessBuilder.Build(this.tibcoBwProcess);
            var fieldName = ((CodeMemberField)classToGenerate.Namespaces[0].Types[0].Members[0]).Name;
            Assert.AreEqual("logger", fieldName);
		}

        [Test]
        public void Should_Return_privateField_When_a_Process_Variable_is_declared()
        {
            this.tibcoBwProcess.ProcessVariables = new List<ProcessVariable>
            {
                new ProcessVariable
                {
                    Parameter = new ClassParameter { Name = "var", Type = "System.String" }
                }
            };
            var classToGenerate = this.tibcoBwProcessBuilder.Build(this.tibcoBwProcess);

            Assert.AreEqual("var", ((CodeMemberField)classToGenerate.Namespaces[0].Types[0].Members[1]).Name);
            Assert.AreEqual("System.String", ((CodeMemberField)classToGenerate.Namespaces[0].Types[0].Members[1]).Type.BaseType);
        }

        [Test]
        public void Should_Return_Generate_Class_For_Process_Variable_When_type_is_not_basic()
        {
            var xml =
                @"<var xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                <xsd:element name=""group"">
    <xsd:complexType>
        <xsd:sequence>
            <xsd:element name=""UndlCcy"" type=""xsd:string""/>
            <xsd:element name=""Ccy"" type=""xsd:string"" minOccurs=""0""/>
        </xsd:sequence>
    </xsd:complexType>
</xsd:element>
</var>";
            var doc = XElement.Parse(xml);

            this.tibcoBwProcess.ProcessVariables = new List<ProcessVariable>
            {
                new ProcessVariable
                {
                    Parameter = new ClassParameter { Name = "var", Type = "group" },
                    ObjectXNodes = doc.Nodes()
                }
            };
            var classToGenerate = this.tibcoBwProcessBuilder.Build(this.tibcoBwProcess);

            var className = classToGenerate.Namespaces[3].Types[0].Name;

            Assert.AreEqual("group", className);
            Assert.AreEqual("MyNamespace.myProcessTestVariables", classToGenerate.Namespaces[3].Name);
        }



		[Test]
		public void Should_Return_4_import_For_Empty_Process()
		{
            var classToGenerate = this.tibcoBwProcessBuilder.Build (this.tibcoBwProcess);

			Assert.AreEqual(3, classToGenerate.Namespaces[0].Imports.Count);
		}

		[Test]
		public void Should_return_void_Start_Method_with_no_input_param_When_no_Start_and_Return_type_are_specified()
		{
            this.tibcoBwProcess.StartActivity = new Activity ("Start", ActivityType.startType);
            this.tibcoBwProcess.EndActivity = new Activity ("End", ActivityType.endType);

			var expected ="System.Void";
            var classToGenerate = this.tibcoBwProcessBuilder.Build (this.tibcoBwProcess);
			string actual = string.Empty;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
					actual = ((CodeMemberMethod)member).ReturnType.BaseType;
				}
			}
		
			Assert.AreEqual (expected, actual);
		}


		[Test]
		public void Should_return_Start_Method_with_one_input_param_When_Starttype_is_defined()
		{
            this.tibcoBwProcess.StartActivity = new Activity ("Start", ActivityType.startType);
			this.tibcoBwProcess.StartActivity.Parameters = new List <ClassParameter> {
				new ClassParameter
				{
					Type = "string",
					Name = "inputName"
				}
			};
            this.tibcoBwProcess.EndActivity = new Activity ("End", ActivityType.endType);

			
            var classToGenerate = this.tibcoBwProcessBuilder.Build (this.tibcoBwProcess);

			string actual = string.Empty;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start")
                {
					actual = ((CodeMemberMethod)member).Parameters[0].Name;
				}
			}

			Assert.AreEqual ("start_inputName", actual);
		}

		[Test]
		public void Should_return_Start_Method_with_return_type_string_When_Return_is_defined()
		{
            // prepare
            this.tibcoBwProcess.StartActivity = new Activity ("Start", ActivityType.startType);

            this.tibcoBwProcess.EndActivity = new Activity ("End", ActivityType.endType);
			this.tibcoBwProcess.EndActivity.Parameters = new List <ClassParameter> {
				new ClassParameter
				{
					Type = "string",
					Name = "endResult"
				}
			};
		    // Act
            var classToGenerate = this.tibcoBwProcessBuilder.Build (this.tibcoBwProcess);

            string actual = string.Empty;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
					actual = ((CodeMemberMethod)member).ReturnType.BaseType;
				}
			}

			Assert.AreEqual ("string", actual);
		}

        [Test]
        public void Should_Convert_Unix_XsdImport_in_Code_namespace_to_import(){
            // Arrange
            var expected = "DAI.PNO.XSD";

            // Act
            var import = TargetAppNameSpaceService.ConvertXsdImportToNameSpace ("/XmlSchemas/DAI/PNO/XSD/RM3D.xsd");

            // Assert
            Assert.AreEqual(expected,import);
        }

        [Test]
        public void Should_Convert_Windows_pathname_in_Code_namespace_to_import()
        {
            // Arrange
            var expected = "DAI.PNO.XSD";

            ConfigurationApp.SaveProperty(MainClass.ProjectDirectory, "c:\\test");

            // Act
            var import = TargetAppNameSpaceService.ConvertXsdImportToNameSpace("c:\\test\\XmlSchemas\\DAI\\PNO\\XSD\\RM3D.xsd");

            // Assert
            Assert.AreEqual(expected, import);
        }

        [Test]
        public void Should_Convert_XsdImport_in_Code_namespace_to_import_when_no_slash()
        {
            // Arrange
            var expected = ".Process.DAI.PNO.Common";

            // Act
			var import = TargetAppNameSpaceService.ConvertXsdImportToNameSpace(".Process.DAI.PNO.Common");

            // Assert
            Assert.AreEqual(expected, import);
        }

        [Test]
        public void Should_RemoveDuplicate_fields()
        {

            var tibcoBwProcessClassModel = new CodeTypeDeclaration("MyTestprocess")
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };
            var fields = new List<CodeMemberField>
                             {
                                 new CodeMemberField
                                     {
                                         Type = new CodeTypeReference("ILogger"),
                                         Name = "logger",
                                         Attributes = MemberAttributes.Private
                                     },
                                     new CodeMemberField
                                     {
                                         Type = new CodeTypeReference("ILogger"),
                                         Name = "logger",
                                         Attributes = MemberAttributes.Private
                                     }
                             };
            tibcoBwProcessClassModel.Members.AddRange(fields.ToArray());
            
            this.tibcoBwProcessBuilder.RemoveDuplicateFields(tibcoBwProcessClassModel);
            Assert.AreEqual(1, tibcoBwProcessClassModel.Members.Count);
        }

        [Test]
        public void Should_Generate_End_InvocationCode()
        {

            this.tibcoBwProcess.StartActivity = new Activity ("Start", ActivityType.startType);
            this.tibcoBwProcess.EndActivity = new Activity ("End", ActivityType.endType);
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
    <EquityRecord>            
        <xmlString>
            <xsl:value-of select=""'TestString'""/>
        </xmlString>
    </EquityRecord>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            this.tibcoBwProcess.EndActivity.InputBindings = doc.Nodes();
            this.tibcoBwProcess.EndActivity.Parameters = new List<ClassParameter>{
                new ClassParameter{
                    Name = "EquityRecord", 
                    Type= "EquityRecord"}
            };

            var expected = @"this.logger.Info(""Start Activity: End of type: endType"");
EquityRecord EquityRecord = new EquityRecord();
EquityRecord.xmlString = ""TestString"";

return EquityRecord;
";

            var codeCollection = this.tibcoBwProcessBuilder.GenerateEndActivityInvocationCode (this.tibcoBwProcess);
            string actual = TestCodeGeneratorUtils.GenerateCode(codeCollection);

            Assert.AreEqual(expected,actual);
        }
	}
}

