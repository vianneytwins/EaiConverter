namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;

    public class CoreProcessBuilder
    {
        private IXpathBuilder xpathBuilder;

        public CoreProcessBuilder()
        {
            this.xpathBuilder = new XpathBuilder();
        }

        /// <summary>
        /// Generates the code statement.
        /// </summary>
        /// <returns>The code statement.</returns>
        /// <param name="processTransitions">list of the transition in the process</param>
        /// <param name="activityName">Activity name.</param>
        /// <param name="exitBeforeActivityName">Exit before activity name.</param>
        /// <param name="activityToServiceMapping">invocation code for each activity</param>
        public CodeStatementCollection GenerateMainCodeStatement(List<Transition> processTransitions, string activityName, string exitBeforeActivityName, Dictionary<string, CodeStatementCollection> activityToServiceMapping)
        {
            processTransitions.Sort();
            var codeStatementCollection = new CodeStatementCollection();
            if (activityName == exitBeforeActivityName)
            {
                return codeStatementCollection;
            }

            var invocationCode = this.GetActivityInvocationCodeStatement(activityName, activityToServiceMapping);

            string startPointOfTryCatch = TransitionUtils.ToActivityOfErrorTransitionFrom(activityName, processTransitions);

            if (invocationCode != null && startPointOfTryCatch == null)
            {
                codeStatementCollection.AddRange(invocationCode);
            }
            else if (startPointOfTryCatch != null)
            {
                // Manage the case where the start point of the process has a try catch around it (can append with Starter only)
                if (invocationCode == null)
                {
                    var validTransition = TransitionUtils.GetValidTransitionsFrom(activityName, processTransitions);
                    if (validTransition.Count > 1)
                    {
                        invocationCode = new CodeStatementCollection()
                                             {
                                                 new CodeSnippetStatement(
                                                     "// Your are in a try catch with several conditionnal transitions on a starter activity please check your worflow generation")
                                             };
                    }
                    else
                    {
                        var newStartPointOfTryCatch = validTransition[0].ToActivity;
                        var nextCommonActivity =
                            TransitionUtils.GetNextCommonActivity(
                                new List<string> { activityName, startPointOfTryCatch },
                                processTransitions);
                        invocationCode = this.GenerateMainCodeStatement(
                            processTransitions,
                            newStartPointOfTryCatch,
                            nextCommonActivity,
                            activityToServiceMapping);
                    }
                }

                // Defines a try statement that calls the ThrowApplicationException method.
                var try1 = new CodeTryCatchFinallyStatement();
                try1.TryStatements.AddRange(invocationCode);
                codeStatementCollection.Add(try1);

                // Defines a catch clause for any remaining unhandled exception types.
                var catch1 = new CodeCatchClause("ex");
                catch1.Statements.AddRange(this.GenerateMainCodeStatement(processTransitions, startPointOfTryCatch, null, activityToServiceMapping));
                try1.CatchClauses.Add(catch1);
            }

            List<Transition> tranz = TransitionUtils.GetValidTransitionsFrom(activityName, processTransitions);
            if (tranz.Count == 0)
            {
                return codeStatementCollection;
            }
            else if (tranz.Count == 1)
            {
                string nextActivity = tranz[0].ToActivity;
                codeStatementCollection.AddRange(this.GenerateMainCodeStatement(processTransitions, nextActivity, exitBeforeActivityName, activityToServiceMapping));
            }
            else
            {
                var nextActivities = new List<string>();
                foreach (var transition in tranz)
                {
                    nextActivities.Add(transition.ToActivity);
                }

                // CodeConditionStatement(CodeExpression, if true => CodeStatement[], else => CodeStatement[])
                // TODO c'est moche car cela marche que pour 1 seul If... S'il y en a plus il faut rajouter des ConditionsStatements sans else
                string nextCommonActivity = TransitionUtils.GetNextCommonActivity(nextActivities, processTransitions);

                var trueCodeStatements = new CodeStatement[] { };
                var falseCodeStatements = new CodeStatement[] { };
                CodeExpression condition = new CodeVariableReferenceExpression();
                foreach (var transition in tranz)
                {
                    var nextActivity = transition.ToActivity;
                    if (ConditionType.xpath == transition.ConditionType)
                    {
                        condition = new CodeVariableReferenceExpression(this.xpathBuilder.Build(transition.ConditionPredicate));
                        var statementCollection = this.GenerateMainCodeStatement(processTransitions, nextActivity, nextCommonActivity, activityToServiceMapping);
                        trueCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo(trueCodeStatements, 0);
                    }
                    else if (ConditionType.otherwise == transition.ConditionType)
                    {
                        var statementCollection = this.GenerateMainCodeStatement(processTransitions, nextActivity, nextCommonActivity, activityToServiceMapping);
                        falseCodeStatements = new CodeStatement[statementCollection.Count];
                        statementCollection.CopyTo(falseCodeStatements, 0);
                    }
                }

                codeStatementCollection.Add(new CodeConditionStatement(condition, trueCodeStatements, falseCodeStatements));
                
                // Call nextCommonActivtyCodeStatementGeneration
                codeStatementCollection.AddRange(this.GenerateMainCodeStatement(processTransitions, nextCommonActivity, null, activityToServiceMapping));
            }

            return codeStatementCollection;
        }

        private CodeStatementCollection GetActivityInvocationCodeStatement(string activityName, Dictionary<string, CodeStatementCollection> activityToInvocation)
        {
            if (activityToInvocation.ContainsKey(activityName))
            {
                return activityToInvocation[activityName];
            }

            return null;

        }
    }
}

