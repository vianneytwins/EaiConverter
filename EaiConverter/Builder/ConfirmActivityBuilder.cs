namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Utils;

    public class ConfirmActivityBuilder : IActivityBuilder
    {

        public ConfirmActivityBuilder()
        {
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(EaiConverter.Model.Activity activity)
		{
			var namespaces = new CodeNamespaceCollection();
			return namespaces;
		}

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
		{
            var confirmActivity = (ConfirmActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // add log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(confirmActivity));

            // Add the callback call to confirm the message
            invocationCodeCollection.Add(new CodeCommentStatement ("TODO: Should be this.subscriber.Confirm(message);"));
            invocationCodeCollection.Add(new CodeSnippetStatement("this.subscriber.Confirm();"));

            return invocationCodeCollection;
		}

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
            return new List<CodeNamespaceImport>();
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
	}

}

