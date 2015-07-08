using EaiConverter.Model;

using System.CodeDom;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class GroupActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;
        CoreProcessBuilder coreProcessBuilder;
        Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection>();

        public GroupActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.coreProcessBuilder = new CoreProcessBuilder();
        }

        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = this.GenerateActivityClasses(((GroupActivity)activity).Activities);
            activityCodeDom.InvocationCode = this.InvocationMethod(activity);
            return activityCodeDom;
        }

        public CodeNamespaceCollection GenerateActivityClasses(List<Activity> activities)
        {
            var activityBuilderFactory = new ActivityBuilderFactory();
            var activityClasses = new CodeNamespaceCollection();
            foreach (var activity in activities)
            {
                var activityBuilder = activityBuilderFactory.Get(activity.Type);

                var activityCodeDom = activityBuilder.Build(activity);

                activityClasses.AddRange(activityCodeDom.ClassesToGenerate);
                this.activityNameToServiceNameDictionnary.Add(activity.Name, activityCodeDom.InvocationCode);
            }
            return activityClasses;
        }

        private CodeStatementCollection InvocationMethod(Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));

            var groupActivity = (GroupActivity)activity;

            if (groupActivity.GroupType == GroupType.inputLoop)
            {
                var forLoop = this.GenerateForLoop(groupActivity);
                invocationCodeCollection.Add(forLoop);
            }
            else if (groupActivity.GroupType == GroupType.repeat)
            {
                var repeatLoop = this.GenerateForRepeat(groupActivity);
                invocationCodeCollection.Add(repeatLoop);
            }
            else
            {
                invocationCodeCollection.AddRange(this.GenerateCoreGroupMethod(groupActivity));
            }

            return invocationCodeCollection;
        }

        private CodeIterationStatement GenerateForLoop(GroupActivity groupActivity)
        {
            var coreGroupMethodStatement = new CodeStatementCollection();

            // put the current element in the declare variable
            // TODO convert the $Variable in variable like in Xpath 
            CodeVariableDeclarationStatement iterationElementSlotDeclaration = new CodeVariableDeclarationStatement("var", groupActivity.IterationElementSlot, new CodeVariableReferenceExpression(groupActivity.Over + "[" + groupActivity.IndexSlot + "]"));
            coreGroupMethodStatement.Add(iterationElementSlotDeclaration);
            // get the core loop code
            coreGroupMethodStatement.AddRange(this.GenerateCoreGroupMethod(groupActivity));
            var coreOfTheLoop = new CodeStatement[coreGroupMethodStatement.Count];
            coreGroupMethodStatement.CopyTo(coreOfTheLoop, 0);

            // put it then in the loop
            CodeIterationStatement forLoop = new CodeIterationStatement(
                new CodeVariableDeclarationStatement(typeof(int), groupActivity.IndexSlot, new CodePrimitiveExpression(0)),
                new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(groupActivity.IndexSlot), CodeBinaryOperatorType.LessThan, new CodeVariableReferenceExpression(groupActivity.Over + ".Lenght")),
                new CodeAssignStatement(new CodeVariableReferenceExpression(groupActivity.IndexSlot), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(groupActivity.IndexSlot), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
                coreOfTheLoop);
            return forLoop;
        }

        private CodeIterationStatement GenerateForRepeat(GroupActivity groupActivity)
        {
            var coreGroupMethodStatement = new CodeStatementCollection();

            // get the core loop code
            coreGroupMethodStatement.AddRange(this.GenerateCoreGroupMethod(groupActivity));
            var coreOfTheLoop = new CodeStatement[coreGroupMethodStatement.Count];
            coreGroupMethodStatement.CopyTo(coreOfTheLoop, 0);

            // put it then in the while loop : code dome don't allow while, so we will trick it with a for(;expression;)
            CodeIterationStatement whileLoop = new CodeIterationStatement(
                new CodeSnippetStatement(string.Empty),
                new CodeSnippetExpression(groupActivity.RepeatCondition),
                new CodeSnippetStatement(string.Empty),
                coreOfTheLoop);
            return whileLoop;
        }

        private CodeStatementCollection GenerateCoreGroupMethod(GroupActivity groupActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();

            invocationCodeCollection.AddRange(this.coreProcessBuilder.GenerateStartCodeStatement(groupActivity.Transitions, "start", null, this.activityNameToServiceNameDictionnary));
            return invocationCodeCollection;
        }
    }
}

