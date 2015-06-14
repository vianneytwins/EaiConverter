using System;
using NUnit.Framework;
using System.Collections.Generic;
using EaiConverter.Builder.Utils;
using EaiConverter;
using EaiConverter.Model;

namespace EaiConverter.Test.Builder.Utils
{
	[TestFixture]
	public class TransitionUtilsTest
	{

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

		[Test]
		public void Should_Return_Step3_And_End_When_retrieving_of_All_next_Activities_From_Step1 () {
			List<string> expected = new List<string>{"step3","End" };
			var nextCommonActivity = TransitionUtils.GetAllNextActivities ( this.complexProcessTransitions, "step1");
			Assert.AreEqual (expected, nextCommonActivity);
		}

		[Test]
		public void Should_Return_All_Activities_When_retrieving_of_All_next_Activities_From_Start () {
			List<string> expected = new List<string>{"step1","step3","End","step2","End"};
			var nextCommonActivity = TransitionUtils.GetAllNextActivities ( this.complexProcessTransitions, "start");
			Assert.AreEqual (expected, nextCommonActivity);
		}

		[Test]
		public void Should_Return_End_When_retrieving_of_next_common_Activity_After_Start () {
			List<string> activitiesAfterStart = new List<string>{"step1","step2"};
			var nextCommonActivity = TransitionUtils.GetNextCommonActivity( activitiesAfterStart, this.complexProcessTransitions);
			Assert.AreEqual ("End", nextCommonActivity);
		}

		[Test]
		public void Should_Return_put_OtherWise_Condition_At_The_End_When_Sort_is_Applied_On_Transitions () {
			this.complexProcessTransitions.Sort ();
			Assert.AreEqual (ConditionType.otherwise, this.complexProcessTransitions[4].ConditionType);
		}

	}
}

