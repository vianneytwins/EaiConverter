namespace EaiConverter.Builder
{
    using System.CodeDom;
	using System.Collections.Generic;

    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class SleepActivityBuilder : AbstractActivityBuilder
	{
        private readonly XslBuilder xslBuilder;

        public SleepActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

		public override List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
            return new List<CodeNamespaceImport> { new CodeNamespaceImport("System.Timers") };
		}

        public override List<CodeMemberMethod> GenerateMethods(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethods = base.GenerateMethods(activity, variables);
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));

            // Add the invocation new Timer (timerValue)
            var code = new CodeSnippetStatement("new Timer(" + activity.Parameters[0].Name + ");");
            invocationCodeCollection.Add(code);

            activityMethods[0].Statements.AddRange(invocationCodeCollection);
            return activityMethods;
        }

        public override string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
	}

}
