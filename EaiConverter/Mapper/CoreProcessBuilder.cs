using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;

namespace EaiConverter.Mapper
{
	public class CoreProcessBuilder
	{
				
		public CodeMethodInvokeExpression GetActivityInvocationCodeStatement (Dictionary <string,string>  activities, string activityName)
		{
			var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), activities [activityName]);
			return new CodeMethodInvokeExpression (activityServiceReference, "ExecuteMethod", new CodeExpression[] {});
		}

        public CodeMethodInvokeExpression GetActivityInvocationCodeStatement (TibcoBWProcess tibcoProcess, string activityName)
        {
            // ce que l'on veut generer :
            // Si c'est les activité Start ou End ou les AssignActivity... c'est des methodes...voir du code direct
            // Pour les autres activités
            // Si le methode est void :
            // this.monActivite.TheMethod(inputparams);
            // Sinon (var pas possible !!!)
            // monTypeDeRetour resultOfActivy = this.monActivite.TheMethod(inputparams);
            if (activityName != tibcoProcess.StartActivity.Name) {
                var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), "Do"+activityName);
                return new CodeMethodInvokeExpression (activityServiceReference, "ExecuteMethod", new CodeExpression[] {});
            }
            return null;

        }

		/// <summary>
		/// Generates the code statement.
		/// </summary>
		/// <returns>The code statement.</returns>
		/// <param name="transitions">Transitions. They must be sorted. For one FromActivity the otherWise conditionnal must be the last one</param>
		/// <param name="activities">Activities.</param>
		/// <param name="activityName">Activity name.</param>
		/// <param name="exitBeforeActivityName">Exit before activity name.</param>
		public CodeStatementCollection  GenerateCodeStatement (List<Transition> transitions, Dictionary <string,string>  activities ,string activityName, string exitBeforeActivityName){
			transitions.Sort ();
			var codeStatementCollection = new CodeStatementCollection ();
			if (activityName == exitBeforeActivityName) {
				return codeStatementCollection;
			}

			var methodCallExpression = this.GetActivityInvocationCodeStatement (activities, activityName);

			codeStatementCollection.Add (methodCallExpression);

			List<Transition> tranz = TransitionUtils.GetTransitionsFrom (transitions, activityName);
			if (tranz.Count == 0) {
				return codeStatementCollection;
			} else if (tranz.Count == 1) {
				string nextActivity = tranz [0].ToActivity;
				codeStatementCollection.AddRange ( this.GenerateCodeStatement (transitions, activities, nextActivity, exitBeforeActivityName));

			}
			else
			{
				var nextActivities =new List<string> ();
				foreach (var transition in tranz) {
					nextActivities.Add (transition.ToActivity);
				}

				//CodeConditionStatement(CodeExpression, if true => CodeStatement[], else => CodeStatement[])
				// TODO c'est moche car cela marche que pour 1 seul If... S'il y en a plus il faut rajouter des ConditionsStatements sans else
				string nextCommonActivity = TransitionUtils.GetNextCommonActivity (nextActivities, transitions);

				CodeStatement[] trueCodeStatements = new CodeStatement[]{};
				CodeStatement[] falseCodeStatements = new CodeStatement[]{};
				CodeExpression condition=new CodeVariableReferenceExpression();
				foreach (var transition in tranz) {
					//var conditionType = transition.ConditionType;
					var nextActivity = transition.ToActivity;
					if (ConditionType.xpath == transition.ConditionType) {
						condition = new CodeVariableReferenceExpression(transition.ConditionPredicateName);
						var statementCollection = this.GenerateCodeStatement (transitions, activities ,nextActivity,nextCommonActivity);
						trueCodeStatements = new CodeStatement[statementCollection.Count];
						statementCollection.CopyTo (trueCodeStatements,0);
					}
					else if (ConditionType.otherwise == transition.ConditionType) {
						var statementCollection = this.GenerateCodeStatement (transitions, activities ,nextActivity,nextCommonActivity);
						falseCodeStatements = new CodeStatement[statementCollection.Count];
						statementCollection.CopyTo (falseCodeStatements,0);
					}
				}
				codeStatementCollection.Add (new CodeConditionStatement(condition, trueCodeStatements, falseCodeStatements));
				//Call nextCommonActivtyCodeStatementGeneration
				codeStatementCollection.AddRange (this.GenerateCodeStatement (transitions, activities ,nextCommonActivity, null));
			}
			return codeStatementCollection;
		}


        /// <summary>
        /// Generates the code statement.
        /// </summary>
        /// <returns>The code statement.</returns>
        /// <param name="transitions">Transitions. They must be sorted. For one FromActivity the otherWise conditionnal must be the last one</param>
        /// <param name="activities">Activities.</param>
        /// <param name="activityName">Activity name.</param>
        /// <param name="exitBeforeActivityName">Exit before activity name.</param>
        public CodeStatementCollection  GenerateStartCodeStatement (TibcoBWProcess tibcoBwProcessToGenerate, CodeMemberMethod startMethod, string activityName, string exitBeforeActivityName){
            tibcoBwProcessToGenerate.Transitions.Sort ();
            var codeStatementCollection = new CodeStatementCollection ();
            if (activityName == exitBeforeActivityName) {
                return codeStatementCollection;
            }

            var methodCallExpression = this.GetActivityInvocationCodeStatement (tibcoBwProcessToGenerate, activityName);

            if (methodCallExpression != null)
            {
                codeStatementCollection.Add(methodCallExpression);
            }

            List<Transition> tranz = TransitionUtils.GetTransitionsFrom (tibcoBwProcessToGenerate.Transitions, activityName);
            if (tranz.Count == 0) {
                return codeStatementCollection;
            } else if (tranz.Count == 1) {
                string nextActivity = tranz [0].ToActivity;
                codeStatementCollection.AddRange ( this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod, nextActivity, exitBeforeActivityName));

            }
            else
            {
                var nextActivities =new List<string> ();
                foreach (var transition in tranz) {
                    nextActivities.Add (transition.ToActivity);
                }

                //CodeConditionStatement(CodeExpression, if true => CodeStatement[], else => CodeStatement[])
                // TODO c'est moche car cela marche que pour 1 seul If... S'il y en a plus il faut rajouter des ConditionsStatements sans else
                string nextCommonActivity = TransitionUtils.GetNextCommonActivity (nextActivities, tibcoBwProcessToGenerate.Transitions);

                CodeStatement[] trueCodeStatements = new CodeStatement[]{};
                CodeStatement[] falseCodeStatements = new CodeStatement[]{};
                CodeExpression condition=new CodeVariableReferenceExpression();
                foreach (var transition in tranz) {
                    //var conditionType = transition.ConditionType;
                    var nextActivity = transition.ToActivity;
                    if (ConditionType.xpath == transition.ConditionType) {
                        condition = new CodeVariableReferenceExpression(transition.ConditionPredicateName);
                        var statementCollection = this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod ,nextActivity,nextCommonActivity);
                        trueCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo (trueCodeStatements,0);
                    }
                    else if (ConditionType.otherwise == transition.ConditionType) {
                        var statementCollection = this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod,nextActivity,nextCommonActivity);
                        falseCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo (falseCodeStatements,0);
                    }
                }
                codeStatementCollection.Add (new CodeConditionStatement(condition, trueCodeStatements, falseCodeStatements));
                //Call nextCommonActivtyCodeStatementGeneration
                codeStatementCollection.AddRange (this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod ,nextCommonActivity, null));
            }
            return codeStatementCollection;
        }


	}
}

