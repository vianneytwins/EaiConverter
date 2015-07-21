namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

    public class GroupActivityBuilder : IActivityBuilder
    {
        private XslBuilder xslBuilder;
        private readonly CoreProcessBuilder coreProcessBuilder;
        private Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection>();

        public GroupActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.coreProcessBuilder = new CoreProcessBuilder();
        }

        public ActivityCodeDom Build(Activity activity)
        {
            this.activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection>();
            var activityCodeDom = new ActivityCodeDom
                                      {
                                          ClassesToGenerate = this.GenerateClassesToGenerate(activity),
                                          InvocationCode = this.GenerateInvocationCode(activity)
                                      };
            return activityCodeDom;
        }
            
        public CodeNamespaceImportCollection GenerateImports(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity groupActivity)
        {
            this.activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection>();
            var activities = ((GroupActivity)groupActivity).Activities;
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

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));

            var groupActivity = (GroupActivity)activity;

            if (groupActivity.GroupType == GroupType.INPUTLOOP)
            {
                invocationCodeCollection.Add(this.GenerateForLoop(groupActivity));
            }
            else if (groupActivity.GroupType == GroupType.REPEAT || groupActivity.GroupType == GroupType.WHILE)
            {
                invocationCodeCollection.Add(this.GenerateForRepeat(groupActivity));
            }
            else if (groupActivity.GroupType == GroupType.CRITICALSECTION)
            {
                invocationCodeCollection.AddRange(this.GenerateForCriticalSection(groupActivity));
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
            var iterationElementSlotDeclaration = new CodeVariableDeclarationStatement("var", groupActivity.IterationElementSlot, new CodeVariableReferenceExpression(groupActivity.Over + "[" + groupActivity.IndexSlot + "]"));
            coreGroupMethodStatement.Add(iterationElementSlotDeclaration);

            // Get the core loop code
            coreGroupMethodStatement.AddRange(this.GenerateCoreGroupMethod(groupActivity));
            var coreOfTheLoop = new CodeStatement[coreGroupMethodStatement.Count];
            coreGroupMethodStatement.CopyTo(coreOfTheLoop, 0);

            // put it then in the loop
            var forLoop = new CodeIterationStatement(
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

            var whileLoop = new CodeIterationStatement(
                new CodeSnippetStatement(string.Empty),
                new CodeSnippetExpression(groupActivity.RepeatCondition),
                new CodeSnippetStatement(string.Empty),
                coreOfTheLoop);
            return whileLoop;
        }

        private CodeStatementCollection  GenerateForCriticalSection(GroupActivity groupActivity)
        {
            // TODO ADD the myLock object as a field in the process
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.Add(
                new CodeSnippetStatement("lock (" + VariableHelper.ToVariableName(groupActivity.Name) + "Lock){"));
            invocationCodeCollection.AddRange(this.GenerateCoreGroupMethod(groupActivity));
            invocationCodeCollection.Add(new CodeSnippetStatement("}"));

            return invocationCodeCollection;
        }

        private CodeStatementCollection GenerateCoreGroupMethod(GroupActivity groupActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();

            invocationCodeCollection.AddRange(this.coreProcessBuilder.GenerateStartCodeStatement(groupActivity.Transitions, "start", null, this.activityNameToServiceNameDictionnary));
            return invocationCodeCollection;
        }
    }
}

