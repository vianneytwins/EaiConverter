namespace EaiConverter.Test.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Parser;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
	public class CoreProcessBuilderTest
	{
        Dictionary <string, CodeStatementCollection> activitiesToServiceMapping = new Dictionary <string, CodeStatementCollection> 
        {
            {"step1", new CodeStatementCollection{ DefaultInvocationMethod("step1Service")}},
            {"step2", new CodeStatementCollection{ DefaultInvocationMethod("step2Service")}},
            {"step3", new CodeStatementCollection{ DefaultInvocationMethod("step3Service")}},
            {"error", new CodeStatementCollection{ DefaultInvocationMethod("errorService")}},
            {"sendmail", new CodeStatementCollection{ DefaultInvocationMethod("sendmailService")}},
            {"getDBDate", new CodeStatementCollection{ DefaultInvocationMethod("getDBDateService")}},
            {"lastValidDate", new CodeStatementCollection{ DefaultInvocationMethod("lastValidDateService")}},
            {"tradeinvalid", new CodeStatementCollection{ DefaultInvocationMethod("tradeinvalidService")}},
            {"step5", new CodeStatementCollection{ DefaultInvocationMethod("step5Service")}},
            {"End",new CodeStatementCollection{ new CodeMethodReturnStatement() }}
        };

		CoreProcessBuilder builder;

		List<Transition> complexProcessTransitions = new List <Transition> {
			new Transition {
				FromActivity = "start",
				ToActivity = "step1",
				ConditionType = ConditionType.xpath,
				ConditionPredicateName = "condition1",
				ConditionPredicate = "isCondition1"
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

        List <Transition> errorProcessTransitions = new List <Transition> {
            new Transition {
                FromActivity = "start",
                ToActivity = "step1",
                ConditionType = ConditionType.always
            },
            new Transition {
                FromActivity = "step1",
                ToActivity = "End",
                ConditionType = ConditionType.xpath,
                ConditionPredicateName = "Condition1"
            },
            new Transition {
                FromActivity = "step1",
                ToActivity = "step2",
                ConditionType = ConditionType.error
            }
        };


		[SetUp]
		public void SetUp(){
			this.builder = new CoreProcessBuilder();
		}

        [Test]
        public void Should_Return_Simple_Start_Method_Body(){
            var expected = @"this.step1Service.ExecuteQuery();
return;
";
            var tibcoBWProcess = new TibcoBWProcess("MyTestProcess");
            tibcoBWProcess.StartActivity = new Activity("start", ActivityType.startType);
            tibcoBWProcess.EndActivity = new Activity("End", ActivityType.endType);
            tibcoBWProcess.Transitions = this.simpleProcessTransitions;

            var codeStatementCollection = this.builder.GenerateMainCodeStatement (tibcoBWProcess.Transitions, tibcoBWProcess.StartActivity.Name, null, activitiesToServiceMapping);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (codeStatementCollection);

            Assert.AreEqual (expected, classesInString);
        }

        [Test]
        public void Should_Return_ERROR_Start_Method_Body(){
            var expected = @"try
{
    this.step1Service.ExecuteQuery();
}
catch (System.Exception ex)
{
    this.step2Service.ExecuteQuery();
}
return;
";
            var tibcoBWProcess = new TibcoBWProcess("MyTestProcess");
            tibcoBWProcess.StartActivity = new Activity("start", ActivityType.startType);
            tibcoBWProcess.EndActivity = new Activity("End", ActivityType.endType);
            tibcoBWProcess.Transitions = this.errorProcessTransitions;

            var codeStatementCollection = this.builder.GenerateMainCodeStatement (tibcoBWProcess.Transitions, tibcoBWProcess.StartActivity.Name, null, activitiesToServiceMapping);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (codeStatementCollection);

            Assert.AreEqual(expected, classesInString);
        }

        [Test]
        public void Should_Return_Complex_Start_Method_Body(){
            var expected = @"if (isCondition1)
{
    this.step1Service.ExecuteQuery();
    this.step3Service.ExecuteQuery();
}
else
{
    this.step2Service.ExecuteQuery();
}
return;
";
            var tibcoBWProcess = new TibcoBWProcess("MyTestProcess");
            tibcoBWProcess.StartActivity = new Activity("start", ActivityType.startType);
            tibcoBWProcess.EndActivity = new Activity("End", ActivityType.endType);
            tibcoBWProcess.Transitions = this.complexProcessTransitions;

            var codeStatementCollection = this.builder.GenerateMainCodeStatement (tibcoBWProcess.Transitions, tibcoBWProcess.StartActivity.Name, null, this.activitiesToServiceMapping);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (codeStatementCollection);

            Assert.AreEqual (expected, classesInString);
        }

	    [Test]
	    public void Should_be_managed_by_kevin()
	    {
            var expected = @"this.getDBDateService.ExecuteQuery();
if (condition_2)
{
	return;
}
else if (condition_1)
{
    this.lastValidDateService.ExecuteQuery();
    if (condition_2)
    {
		return;        
    }
}
this.tradeinvalidService.ExecuteQuery();
if (!GOTO_ERROR)
{
	this.sendmailService.ExecuteQuery();
}
this.errorService.ExecuteQuery();
";
	        var tibcoParser = new TibcoBWProcessLinqParser();
	        var docXml = XElement.Load("../../ressources/complex_transition.xml");

	        var transitions = tibcoParser.ParseTransitions(docXml);

            var codeStatementCollection = this.builder.GenerateMainCodeStatement(transitions, "start", null, this.activitiesToServiceMapping);

            var classesInString = TestCodeGeneratorUtils.GenerateCode(codeStatementCollection);

            Assert.AreEqual(expected, classesInString);

	    }

	    public static CodeMethodInvokeExpression DefaultInvocationMethod (string activityName){
            var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(activityName));
            return new CodeMethodInvokeExpression (activityServiceReference, "ExecuteQuery", new CodeExpression[] {});
        }
	}
}

