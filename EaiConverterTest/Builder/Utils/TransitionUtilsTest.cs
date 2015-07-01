﻿using NUnit.Framework;
using System.Collections.Generic;
using EaiConverter.Builder.Utils;
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

		[Test]
		public void Should_Return_Step3_And_End_When_retrieving_of_All_next_Activities_From_Step1 () {
			List<string> expected = new List<string>{"step3","End" };
            var nextCommonActivity = TransitionUtils.GetAllNextActivities ( "step1", this.complexProcessTransitions);
			Assert.AreEqual (expected, nextCommonActivity);
		}

		[Test]
		public void Should_Return_All_Activities_When_retrieving_of_All_next_Activities_From_Start () {
			List<string> expected = new List<string>{"step1","step3","End","step2","End"};
            var nextCommonActivity = TransitionUtils.GetAllNextActivities ( "start", this.complexProcessTransitions);
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
            
        [Test]
        public void Should_Return_true_When_an_Error_transition_exist_from_the_given_activity () {
            Assert.AreEqual ("step2", TransitionUtils.ToActivityOfErrorTransitionFrom( "step1", this.errorProcessTransitions));
        }

	}
}

