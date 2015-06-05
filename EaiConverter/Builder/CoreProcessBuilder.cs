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
        public CodeStatementCollection GetActivityInvocationCodeStatement (TibcoBWProcess tibcoProcess, string activityName, Dictionary<string,CodeStatementCollection> activityToInvocation)
        {
            // ce que l'on veut generer :
            // Si c'est les activité Start ou End ou les AssignActivity... c'est des methodes...voir du code direct
            // Pour les autres activités
            // Si le methode est void :
            // this.monActivite.TheMethod(inputparams);
            // Sinon (var pas possible !!!)
            // monTypeDeRetour resultOfActivy = this.monActivite.TheMethod(inputparams);

            if (activityName == tibcoProcess.EndActivity.Name)
            {
                return null;
            }
            if (activityName == tibcoProcess.StartActivity.Name)
            {
                return null;
            }
            else
            {
                return activityToInvocation[activityName];
            }


        }


        /// <summary>
        /// Generates the code statement.
        /// </summary>
        /// <returns>The code statement.</returns>
        /// <param name="transitions">Transitions. They must be sorted. For one FromActivity the otherWise conditionnal must be the last one</param>
        /// <param name="activities">Activities.</param>
        /// <param name="activityName">Activity name.</param>
        /// <param name="exitBeforeActivityName">Exit before activity name.</param>
        public CodeStatementCollection  GenerateStartCodeStatement (TibcoBWProcess tibcoBwProcessToGenerate, CodeMemberMethod startMethod, string activityName, string exitBeforeActivityName, Dictionary<string,CodeStatementCollection> activityToServiceMapping){
            tibcoBwProcessToGenerate.Transitions.Sort ();
            var codeStatementCollection = new CodeStatementCollection ();
            if (activityName == exitBeforeActivityName) {
                return codeStatementCollection;
            }

            var invocationCode = this.GetActivityInvocationCodeStatement (tibcoBwProcessToGenerate, activityName, activityToServiceMapping);

            if (invocationCode != null)
            {
                codeStatementCollection.AddRange(invocationCode);
            }

            List<Transition> tranz = TransitionUtils.GetTransitionsFrom (tibcoBwProcessToGenerate.Transitions, activityName);
            if (tranz.Count == 0) {
                return codeStatementCollection;
            } else if (tranz.Count == 1) {
                string nextActivity = tranz [0].ToActivity;
                codeStatementCollection.AddRange ( this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod, nextActivity, exitBeforeActivityName,activityToServiceMapping));

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
                        var statementCollection = this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod ,nextActivity,nextCommonActivity,activityToServiceMapping);
                        trueCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo (trueCodeStatements,0);
                    }
                    else if (ConditionType.otherwise == transition.ConditionType) {
                        var statementCollection = this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod,nextActivity,nextCommonActivity,activityToServiceMapping);
                        falseCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo (falseCodeStatements,0);
                    }
                }
                codeStatementCollection.Add (new CodeConditionStatement(condition, trueCodeStatements, falseCodeStatements));
                //Call nextCommonActivtyCodeStatementGeneration
                codeStatementCollection.AddRange (this.GenerateStartCodeStatement (tibcoBwProcessToGenerate, startMethod ,nextCommonActivity, null, activityToServiceMapping));
            }
            return codeStatementCollection;
        }


	}
}

