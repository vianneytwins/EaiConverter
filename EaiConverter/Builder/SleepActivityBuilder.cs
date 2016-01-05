using System.CodeDom;
using System.Collections.Generic;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
	using EaiConverter.Model;

    public class SleepActivityBuilder : IActivityBuilder
	{
        private readonly XslBuilder xslBuilder;

        public SleepActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
        }

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
            return new List<CodeNamespaceImport> { new CodeNamespaceImport("System.Timers") };
		}

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            return new CodeParameterDeclarationExpressionCollection();
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            return new CodeStatementCollection();
        }

        public List<CodeMemberField> GenerateFields(Activity activity)
        {
            return new List<CodeMemberField>();
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));

            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));

            // Add the invocation new Timer (timerValue)
            var code = new CodeSnippetStatement("new Timer(" + activity.Parameters[0].Name + ");");
            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }

        public string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
	}

}
