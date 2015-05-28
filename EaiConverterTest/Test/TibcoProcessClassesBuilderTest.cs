using System;
using NUnit.Framework;
using EaiConverter.CodeGenerator;
using System.Collections.Generic;
using System.Text;
using EaiConverter.Mapper;
using System.CodeDom.Compiler;
using System.IO;
using System.CodeDom;
using EaiConverter.Model;
using EaiConverter.Test.Utils;
using System.Xml.Linq;

namespace EaiConverter
{
	[TestFixture]
	public class TibcoProcessClassesBuilderTest
	{
	
		TibcoBWProcess tibcoBWProcess;
        TibcoProcessClassesBuilder tibcoBWProcessBuilder;


		[SetUp]
		public void SetUp ()
		{
            tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();

			tibcoBWProcess = new TibcoBWProcess ("MyNamespace/myProcessTest.process");
			tibcoBWProcess.Activities = new List<Activity> ();
		}

		[Test]
		public void Should_Return_simple_Constructor_When_NoActivies_are_declared()
		{

			var expected ="this.logger = logger;\n";
			var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);

            string classesInString = string.Empty;

			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeConstructor) {
                    classesInString = TestCodeGeneratorUtils.GenerateCode(((CodeConstructor)member).Statements);
				}
			}

            Assert.AreEqual (expected, classesInString);
		}

		[Test]
		public void Should_Return_Constructor_with_setter_of_Activity_When_1_Activy_is_declared()
		{
            tibcoBWProcess.Activities.Add(new Activity ("MySqlRequestActivity", ActivityType.NotHandleYet));
            var expected = "this.logger = logger;\nthis.mySqlRequestActivityService = mySqlRequestActivityService;\n";
			
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);

            string classesInString = string.Empty;

            foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
                if (member is CodeConstructor) {
                    classesInString = TestCodeGeneratorUtils.GenerateCode(((CodeConstructor)member).Statements);
                }
            }

			//classGenerator.GenerateMethod(classToGenerate.Constructors[0], stringBuilder, new TibcoBWConverter.CodeGenerator.utils.Tab(),false);
            Assert.AreEqual (expected, classesInString);
		}


		[Test]
		public void Should_Return_logger_as_a_privateField()
		{

			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);
            var fieldName = ((CodeMemberField)classToGenerate.Namespaces[0].Types[0].Members[0]).Name;
            Assert.AreEqual ("logger", fieldName);
		}

        [Test]
        public void Should_Return_privateField_When_a_Process_Variable_is_declared()
        {
            tibcoBWProcess.ProcessVariables = new List<ProcessVariable>{
                new ProcessVariable{
                    Parameter = new ClassParameter{Name = "var",Type = "string"}
                }
            };
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);

            var fieldName = ((CodeMemberField)classToGenerate.Namespaces[0].Types[0].Members[1]).Name;

            Assert.AreEqual ("var", fieldName);
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

            tibcoBWProcess.ProcessVariables = new List<ProcessVariable>{
                new ProcessVariable{
                    Parameter = new ClassParameter{Name = "var",Type = "group"},
                    ObjectXNodes = doc.Nodes()
                }
            };
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);

            var className = classToGenerate.Namespaces[1].Types[0].Name;

            Assert.AreEqual ("group", className);
            Assert.AreEqual ("MyNamespace.myProcessTest", classToGenerate.Namespaces[1].Name);
        }



		[Test]
		public void Should_Return_4_import_For_Empty_Process()
		{
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);

			Assert.AreEqual (4, classToGenerate.Namespaces[0].Imports.Count);
		}

		[Test]
		public void Should_return_void_Start_Method_with_no_input_param_When_no_Start_and_Return_type_are_specified()
		{
            tibcoBWProcess.StartActivity = new Activity ("Start", ActivityType.startType);
            tibcoBWProcess.EndActivity = new Activity ("End", ActivityType.endType);

			var expected ="void";
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);
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
            tibcoBWProcess.StartActivity = new Activity ("Start", ActivityType.startType);
			tibcoBWProcess.StartActivity.Parameters = new List <ClassParameter> {
				new ClassParameter
				{
					Type = "string",
					Name = "inputName"
				}
			};
            tibcoBWProcess.EndActivity = new Activity ("End", ActivityType.endType);

			
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);

			string actual = string.Empty;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
					actual = ((CodeMemberMethod)member).Parameters[0].Name;
				}
			}

			Assert.AreEqual ("inputName", actual);
		}

        [Test]
        public void Should_have_Start_Method_with_one_Complex_input_param()
        {
            tibcoBWProcess.StartActivity = new Activity ("Start", ActivityType.startType);
            tibcoBWProcess.StartActivity.Parameters = new List <ClassParameter> {
                new ClassParameter
                {
                    Type = "NotSimpleType",
                    Name = "inputName"
                }
            };
            tibcoBWProcess.EndActivity = new Activity ("End", ActivityType.endType);


            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);

            string actual = string.Empty;
            foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
                if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
                    actual = ((CodeMemberMethod)member).Parameters[0].Type.BaseType;
                }
            }

            Assert.AreEqual (tibcoBWProcess.inputAndOutputNameSpace + ".NotSimpleType", actual);
        }

		[Test]
		public void Should_return_Start_Method_with_return_type_string_When_Return_is_defined()
		{
            // prepare
            tibcoBWProcess.StartActivity = new Activity ("Start", ActivityType.startType);

            tibcoBWProcess.EndActivity = new Activity ("End", ActivityType.endType);
			tibcoBWProcess.EndActivity.Parameters = new List <ClassParameter> {
				new ClassParameter
				{
					Type = "string",
					Name = "endResult"
				}
			};
		
			
            // Act
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);


			string actual = string.Empty;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
					actual = ((CodeMemberMethod)member).ReturnType.BaseType;
				}
			}

			Assert.AreEqual ("string", actual);
		}

        [Test]
        public void Should_return_Start_Method_with_complex_return_type()
        {
            // prepare
            tibcoBWProcess.StartActivity = new Activity ("Start", ActivityType.startType);

            tibcoBWProcess.EndActivity = new Activity ("End", ActivityType.endType);
            tibcoBWProcess.EndActivity.Parameters = new List <ClassParameter> {
                new ClassParameter
                {
                    Type = "NotSimpleType",
                    Name = "endResult"
                }
            };


            // Act
            var classToGenerate = this.tibcoBWProcessBuilder.Build (tibcoBWProcess);


            string actual = string.Empty;
            foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
                if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
                    actual = ((CodeMemberMethod)member).ReturnType.BaseType;
                }
            }

            Assert.AreEqual (tibcoBWProcess.inputAndOutputNameSpace + ".NotSimpleType", actual);
        }

        [Test]
        public void Should_Convert_XsdImport_in_Code_namespace_to_import(){
            // Arrange
            var expected = "DAI.PNO.XSD";

            // Act
            var import = TibcoProcessClassesBuilder.ConvertXsdImportToNameSpace ("/XmlSchemas/DAI/PNO/XSD/RM3D.xsd");

            // Assert
            Assert.AreEqual(expected,import);
        }
	}
}

