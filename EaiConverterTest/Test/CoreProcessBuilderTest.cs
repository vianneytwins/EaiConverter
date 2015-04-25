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
        Dictionary <string, string> activitiesToServiceMapping = new Dictionary <string, string> 
        {
            {"step1","step1Service"},
            {"step2","step2Service"},
            {"step3","step3Service"}
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
        public void Should_Return_Simple_Start_Method_Body(){
            var expected = @"this.step1Service.ExecuteQuery();
";
            var tibcoBWProcess = new TibcoBWProcess("MyTestProcess");
            tibcoBWProcess.StartActivity = new Activity("start", ActivityType.startType);
            tibcoBWProcess.EndActivity = new Activity("End", ActivityType.endType);
            tibcoBWProcess.Transitions = this.simpleProcessTransitions;

            CodeMemberMethod startMethod = new CodeMemberMethod();
            var codeStatementCollection = this.builder.GenerateStartCodeStatement (tibcoBWProcess, startMethod, tibcoBWProcess.StartActivity.Name, null, activitiesToServiceMapping);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (codeStatementCollection);

            Assert.AreEqual (expected, classesInString);
        }

        [Test]
        public void Should_Return_Complex_Start_Method_Body(){
            var expected = @"if (condition1)
{
    this.step1Service.ExecuteQuery();
    this.step3Service.ExecuteQuery();
}
else
{
    this.step2Service.ExecuteQuery();
}
";
            var tibcoBWProcess = new TibcoBWProcess("MyTestProcess");
            tibcoBWProcess.StartActivity = new Activity("start", ActivityType.startType);
            tibcoBWProcess.EndActivity = new Activity("End", ActivityType.endType);
            tibcoBWProcess.Transitions = this.complexProcessTransitions;

            CodeMemberMethod startMethod = new CodeMemberMethod();
            var codeStatementCollection = this.builder.GenerateStartCodeStatement (tibcoBWProcess, startMethod, tibcoBWProcess.StartActivity.Name, null, activitiesToServiceMapping);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (codeStatementCollection);

            Assert.AreEqual (expected, classesInString);
        }


	}
}

