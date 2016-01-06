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

        public override CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethod(activity, variables);
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));

            // Add the invocation new Timer (timerValue)
            var code = new CodeSnippetStatement("new Timer(" + activity.Parameters[0].Name + ");");
            invocationCodeCollection.Add(code);

            activityMethod.Statements.AddRange(invocationCodeCollection);
            return activityMethod;
        }

        public override string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
	}

}
