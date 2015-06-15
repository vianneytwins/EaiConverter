using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using EaiConverter.Model;
using EaiConverter.Builder.Utils;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
	public class CoreProcessBuilder
	{
        public CodeStatementCollection GetActivityInvocationCodeStatement (string activityName, Dictionary<string,CodeStatementCollection> activityToInvocation)
        {
            if (activityToInvocation.ContainsKey(activityName))
            {
                return activityToInvocation[activityName];
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
        public CodeStatementCollection  GenerateStartCodeStatement (List<Transition> processTransitions, string activityName, string exitBeforeActivityName, Dictionary<string,CodeStatementCollection> activityToServiceMapping){
            processTransitions.Sort ();
            var codeStatementCollection = new CodeStatementCollection ();
            if (activityName == exitBeforeActivityName) {
                return codeStatementCollection;
            }

            var invocationCode = this.GetActivityInvocationCodeStatement ( activityName, activityToServiceMapping);

            if (invocationCode != null)
            {
                codeStatementCollection.AddRange(invocationCode);
            }

            List<Transition> tranz = TransitionUtils.GetTransitionsFrom (processTransitions, activityName);
            if (tranz.Count == 0) {
                return codeStatementCollection;
            } else if (tranz.Count == 1) {
                string nextActivity = tranz [0].ToActivity;
                codeStatementCollection.AddRange ( this.GenerateStartCodeStatement (processTransitions, nextActivity, exitBeforeActivityName,activityToServiceMapping));

            }
            else
            {
                var nextActivities =new List<string> ();
                foreach (var transition in tranz) {
                    nextActivities.Add (transition.ToActivity);
                }

                //CodeConditionStatement(CodeExpression, if true => CodeStatement[], else => CodeStatement[])
                // TODO c'est moche car cela marche que pour 1 seul If... S'il y en a plus il faut rajouter des ConditionsStatements sans else
                string nextCommonActivity = TransitionUtils.GetNextCommonActivity (nextActivities, processTransitions);

                CodeStatement[] trueCodeStatements = new CodeStatement[]{};
                CodeStatement[] falseCodeStatements = new CodeStatement[]{};
                CodeExpression condition=new CodeVariableReferenceExpression();
                foreach (var transition in tranz) {
                    //var conditionType = transition.ConditionType;
                    var nextActivity = transition.ToActivity;
                    if (ConditionType.xpath == transition.ConditionType) {
                        condition = new CodeVariableReferenceExpression(transition.ConditionPredicateName);
                        var statementCollection = this.GenerateStartCodeStatement (processTransitions, nextActivity,nextCommonActivity,activityToServiceMapping);
                        trueCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo (trueCodeStatements,0);
                    }
                    else if (ConditionType.otherwise == transition.ConditionType) {
                        var statementCollection = this.GenerateStartCodeStatement (processTransitions, nextActivity,nextCommonActivity,activityToServiceMapping);
                        falseCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo (falseCodeStatements,0);
                    }
                }
                codeStatementCollection.Add (new CodeConditionStatement(condition, trueCodeStatements, falseCodeStatements));
                //Call nextCommonActivtyCodeStatementGeneration
                codeStatementCollection.AddRange (this.GenerateStartCodeStatement (processTransitions, nextCommonActivity, null, activityToServiceMapping));
            }
            return codeStatementCollection;
        }


	}
}

