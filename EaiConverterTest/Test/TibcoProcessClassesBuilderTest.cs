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

namespace EaiConverter
{
	[TestFixture]
	public class TibcoProcessClassesBuilderTest
	{
		CodeDomProvider classGenerator;
		TibcoBWProcess tibcoBWProcess;
		string classesInString;
		CodeGeneratorOptions options;

		[SetUp]
		public void SetUp ()
		{
			classGenerator = CodeDomProvider.CreateProvider("CSharp");
			options = new CodeGeneratorOptions();
			options.BracingStyle = "C";

			tibcoBWProcess = new TibcoBWProcess ("MyNamespace/myProcessTest.process");
			tibcoBWProcess.Activities = new List<Activity> ();
			//stringBuilder = new StringBuilder ();
		}

		[Test]
		public void Should_Return_simple_Constructor_When_NoActivies_are_declared()
		{

			var expected ="this.logger = logger;\n";
			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);


			using (StringWriter writer = new StringWriter ()) {
				foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
					if (member is CodeConstructor) {
						classGenerator.GenerateCodeFromStatement 
						(((CodeConstructor)member).Statements [0], writer, new CodeGeneratorOptions ());
						classesInString = writer.GetStringBuilder ().ToString ();
					}
				}
			}

			//classGenerator.GenerateMethod(classToGenerate.Constructors[0], stringBuilder, new TibcoBWConverter.CodeGenerator.utils.Tab(),false);
            Assert.AreEqual (expected, classesInString.RemoveWindowsReturnLineChar());
		}

		[Test]
		public void Should_Return_Constructor_with_setter_of_Activity_When_1_Activy_is_declared()
		{
			tibcoBWProcess.Activities.Add(new Activity ("MySqlRequestActivity","JbdcQuery"));
			var expected = "this.mySqlRequestActivityService = mySqlRequestActivityService;\n";
			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);


			using (StringWriter writer = new StringWriter ()) {
				foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
					if (member is CodeConstructor) {
						classGenerator.GenerateCodeFromStatement 
						(((CodeConstructor)member).Statements [1], writer, new CodeGeneratorOptions ());
						classesInString = writer.GetStringBuilder ().ToString ();
					}
				}
			}

			//classGenerator.GenerateMethod(classToGenerate.Constructors[0], stringBuilder, new TibcoBWConverter.CodeGenerator.utils.Tab(),false);
            Assert.AreEqual (expected, classesInString.RemoveWindowsReturnLineChar());
		}


		[Test]
		public void Should_Return_1_privateField_When_NoActivies_are_declared()
		{

			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);
			int fieldCount = 0;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberField) {
					fieldCount++;
				}
			}

			Assert.AreEqual (1, fieldCount);
		}

		[Test]
		public void Should_Return_4_import_For_Empty_Process()
		{
		
			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);


			//classGenerator.GenerateImport(classToGenerate.Imports, stringBuilder, new TibcoBWConverter.CodeGenerator.utils.Tab());
			Assert.AreEqual (4, classToGenerate.Namespaces[0].Imports.Count);
		}

		[Test]
		public void Should_return_void_Start_Method_with_no_input_param_When_no_Start_and_Return_type_are_specified()
		{
			tibcoBWProcess.StartActivity = new Activity ("Start", "Start");
			tibcoBWProcess.EndActivity = new Activity ("End", "End");

			var expected ="void";			
			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);
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
			tibcoBWProcess.StartActivity = new Activity ("Start", "Start");
			tibcoBWProcess.StartActivity.Parameters = new List <ClassParameter> {
				new ClassParameter
				{
					Type = "string",
					Name = "inputName"
				}
			};
			tibcoBWProcess.EndActivity = new Activity ("End", "End");

			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);

			string actual = string.Empty;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
					actual = ((CodeMemberMethod)member).Parameters[0].Name;
				}
			}

			Assert.AreEqual ("inputName", actual);
		}

		[Test]
		public void Should_return_Start_Method_with_return_type_string_When_Return_is_defined()
		{
			tibcoBWProcess.StartActivity = new Activity ("Start", "Start");
			tibcoBWProcess.StartActivity.Parameters = new List <ClassParameter> {
				new ClassParameter
				{
					Type = "string",
					Name = "inputName"
				}
			};
			tibcoBWProcess.EndActivity = new Activity ("End", "End");
			tibcoBWProcess.EndActivity.Parameters = new List <ClassParameter> {
				new ClassParameter
				{
					Type = "string",
					Name = "endResult"
				}
			};
		
			var tibcoBWProcessBuilder = new TibcoProcessClassesBuilder ();
			var classToGenerate = tibcoBWProcessBuilder.Build (tibcoBWProcess);


			string actual = string.Empty;
			foreach (var member in classToGenerate.Namespaces [0].Types [0].Members) {
				if (member is CodeMemberMethod &&  ((CodeMemberMethod)member).Name == "Start") {
					actual = ((CodeMemberMethod)member).ReturnType.BaseType;
				}
			}
			//classGenerator.GenerateMethod(classToGenerate.Methods[0], stringBuilder, new TibcoBWConverter.CodeGenerator.utils.Tab(),false);
			Assert.AreEqual ("string", actual);
		}
	}
}

