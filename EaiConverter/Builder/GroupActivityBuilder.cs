namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

    using log4net;

    public class GroupActivityBuilder : IActivityBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GroupActivityBuilder));

        private XslBuilder xslBuilder;
        private readonly CoreProcessBuilder coreProcessBuilder;
        private Dictionary<string, CodeStatementCollection> activityNameToServiceNameDictionnary = new Dictionary<string, CodeStatementCollection>();
		private readonly ActivityBuilderFactory activityBuilderFactory;

        private IXpathBuilder xpathBuilder;

        public GroupActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.coreProcessBuilder = new CoreProcessBuilder();
            this.xpathBuilder = new XpathBuilder();
			this.activityBuilderFactory = new ActivityBuilderFactory();
        }
            
		public List<CodeNamespaceImport> GenerateImports(Activity groupActivity)
        {
			var import4Activities = new List<CodeNamespaceImport>();

			foreach (var activity in ((GroupActivity)groupActivity).Activities)
			{
				var activityBuilder = activityBuilderFactory.Get(activity.Type);
				import4Activities.AddRange(activityBuilder.GenerateImports(activity));
			}

			return import4Activities;
        }

		public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity groupActivity)
        {
			var parameters = new CodeParameterDeclarationExpressionCollection ();
			foreach (var activity in ((GroupActivity)groupActivity).Activities)
			{
				var activityBuilder = activityBuilderFactory.Get(activity.Type);
				parameters.AddRange(activityBuilder.GenerateConstructorParameter(activity));
			}

			return parameters;

        }

		public CodeStatementCollection GenerateConstructorCodeStatement(Activity groupActivity)
        {
			var statements = new CodeStatementCollection ();
			foreach (var activity in ((GroupActivity)groupActivity).Activities)
			{
				var activityBuilder = activityBuilderFactory.Get(activity.Type);
				statements.AddRange(activityBuilder.GenerateConstructorCodeStatement(activity));
			}

			return statements;

        }

		public List<CodeMemberField> GenerateFields(Activity groupActivity)
        {
			var fields = new List<CodeMemberField>();

			foreach (var activity in ((GroupActivity)groupActivity).Activities)
			{
				var activityBuilder = this.activityBuilderFactory.Get(activity.Type);
				fields.AddRange(activityBuilder.GenerateFields(activity));
			}

			if (groupActivity.Type == ActivityType.criticalSectionGroupActivityType)
			{
				// Lock for the synchronise section
				fields.Add(new CodeMemberField
					{
						Type = new CodeTypeReference("System.Object"),
						Name = VariableHelper.ToVariableName(VariableHelper.ToVariableName(groupActivity.Name + "Lock")),
						Attributes = MemberAttributes.Private,
						InitExpression = new CodeSnippetExpression("new System.Object()")
					});
			}

			return fields;
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

				activityClasses.AddRange(activityBuilder.GenerateClassesToGenerate(activity));
				this.activityNameToServiceNameDictionnary.Add(activity.Name, activityBuilder.GenerateInvocationCode(activity));
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
            var iterationElementSlotDeclaration = new CodeVariableDeclarationStatement("var", groupActivity.IterationElementSlot, new CodeVariableReferenceExpression(this.xpathBuilder.Build(groupActivity.Over) + "[" + groupActivity.IndexSlot + "]"));
            coreGroupMethodStatement.Add(iterationElementSlotDeclaration);

            // Get the core loop code
            coreGroupMethodStatement.AddRange(this.GenerateCoreGroupMethod(groupActivity));
            var coreOfTheLoop = new CodeStatement[coreGroupMethodStatement.Count];
            coreGroupMethodStatement.CopyTo(coreOfTheLoop, 0);

            // put it then in the loop
            var forLoop = new CodeIterationStatement(
                new CodeVariableDeclarationStatement(typeof(int), groupActivity.IndexSlot, new CodePrimitiveExpression(0)),
                new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(groupActivity.IndexSlot), CodeBinaryOperatorType.LessThan, new CodeVariableReferenceExpression( this.xpathBuilder.Build(groupActivity.Over) + ".Lenght")),
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
                new CodeSnippetExpression(this.xpathBuilder.Build(groupActivity.RepeatCondition)),
                new CodeSnippetStatement(string.Empty),
                coreOfTheLoop);
            return whileLoop;
        }

        private CodeStatementCollection GenerateForCriticalSection(GroupActivity groupActivity)
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

            try
            {
                invocationCodeCollection.AddRange(this.coreProcessBuilder.GenerateMainCodeStatement(groupActivity.Transitions, "start", null, this.activityNameToServiceNameDictionnary));
            }
            catch (Exception e)
            {
                invocationCodeCollection.Add(new CodeSnippetStatement("// TODO : Unable to Generate code for this Group"));
                Log.Error("################ Unable to Generate code for this Group :" + groupActivity.Name, e);
            }

            return invocationCodeCollection;
        }
    }
}

