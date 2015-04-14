using System;
using NUnit.Framework;
using System.Collections.Generic;
using EaiConverter;
using System.Text;
using EaiConverter.Mapper;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;
using EaiConverter.Model;
using EaiConverter.Test.Utils;

namespace EaiConverter
{
	[TestFixture]
	public class CoreProcessBuilderTest
	{
		Dictionary <string, string> activities = new Dictionary <string, string> 
		{
			{"start","DoStart"},
			{"step1","DoStep1"},
			{"End","DoEnd"},
			{"step2","DoStep2"},
			{"step3","DoStep3"}
		};

		CoreProcessBuilder builder;

		List<Transition> complexProcessTransitions = new List <Transition> {
			new Transition {
				FromActivity = "start",
				ToActivity = "step1",
				ConditionType = ConditionType.xpath,
				ConditionPredicateName = "condition1"
			},
			new Transition {
				FromActivity = "start",
				ToActivity = "step2",
				ConditionType = ConditionType.otherwise,
				ConditionPredicateName = "else"
			},
			new Transition {
				FromActivity = "step1",
				ToActivity = "step3",
				ConditionType = ConditionType.always
			},
			new Transition {
				FromActivity = "step3",
				ToActivity = "End",
				ConditionType = ConditionType.always
			},
			new Transition {
				FromActivity = "step2",
				ToActivity = "End",
				ConditionType = ConditionType.always
			}
		};

        List <Transition> simpleProcessTransitions = new List <Transition> {
            new Transition {
                FromActivity = "start",
                ToActivity = "step1",
                ConditionType = ConditionType.always
            },
            new Transition {
                FromActivity = "step1",
                ToActivity = "End",
                ConditionType = ConditionType.always
            }
        };

		[SetUp]
		public void SetUp(){
			this.builder = new CoreProcessBuilder ();

		}
		[Test]
		public void Should_Return_Simple_Process(){

			var activityStart = "start";
            var expected = @"this.DoStart.ExecuteMethod();
this.DoStep1.ExecuteMethod();
this.DoEnd.ExecuteMethod();
";
			var codeStatementCollection = this.builder.GenerateCodeStatement (simpleProcessTransitions, this.activities ,activityStart, null);

			var classesInString = GenerateCode (codeStatementCollection);

            Assert.AreEqual (expected, classesInString.RemoveWindowsReturnLineChar());
		}
			

		[Test]
		public void Should_Return_Complex_if_Process(){

			var activityStart = "start";
            var expected = @"this.DoStart.ExecuteMethod();
if (condition1)
{
    this.DoStep1.ExecuteMethod();
    this.DoStep3.ExecuteMethod();
}
else
{
    this.DoStep2.ExecuteMethod();
}
this.DoEnd.ExecuteMethod();
";
			this.complexProcessTransitions.Sort ();
			var codeStatementCollection = this.builder.GenerateCodeStatement (this.complexProcessTransitions, this.activities ,activityStart, null);

			var classesInString = GenerateCode (codeStatementCollection);

            Assert.AreEqual (expected, classesInString.RemoveWindowsReturnLineChar());
		}

        [Test]
        public void Should_Return_Simple_Start_Method_Body(){
            var expected = @"this.Dostep1.ExecuteMethod();
this.DoEnd.ExecuteMethod();
";
            var tibcoBWProcess = new TibcoBWProcess("MyTestProcess");
            tibcoBWProcess.StartActivity = new Activity("start", ActivityType.startType);
            tibcoBWProcess.EndActivity = new Activity("End", ActivityType.endType);
            tibcoBWProcess.Transitions = simpleProcessTransitions;

            CodeMemberMethod startMethod = new CodeMemberMethod();
            var codeStatementCollection = this.builder.GenerateStartCodeStatement (tibcoBWProcess, startMethod, tibcoBWProcess.StartActivity.Name, null);

            var classesInString = GenerateCode (codeStatementCollection);

            Assert.AreEqual (expected, classesInString.RemoveWindowsReturnLineChar());
        }



		static string GenerateCode (CodeStatementCollection codeStatementCollection)
		{
			var classGenerator = CodeDomProvider.CreateProvider ("CSharp");
			var options = new CodeGeneratorOptions ();
			options.BracingStyle = "C";
			string classesInString;
			using (StringWriter writer = new StringWriter ()) {
				foreach (CodeStatement codeStatement in codeStatementCollection) {
					classGenerator.GenerateCodeFromStatement (codeStatement, writer, options);
				}
				classesInString = writer.GetStringBuilder ().ToString ();
			}
			return classesInString;
		}
	}
}

