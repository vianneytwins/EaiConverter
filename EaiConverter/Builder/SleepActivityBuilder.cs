using System.CodeDom;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
	using EaiConverter.Model;

    public class SleepActivityBuilder : IActivityBuilder
	{
        XslBuilder xslBuilder;

        public SleepActivityBuilder (XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
        }

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>();
		}

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));
            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));

            // Add the invocation new Timer (timerValue)
            var code = new CodeSnippetStatement ("new Timer("+ activity.Parameters[0].Name+");");

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }
	}

}
