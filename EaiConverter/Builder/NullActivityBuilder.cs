using EaiConverter.Model;

using System.CodeDom;

namespace EaiConverter.Builder
{
    public class NullActivityBuilder : IActivityBuilder
    {
        public NullActivityBuilder(XslBuilder xslbuilder) { }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
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

        public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));

            return invocationCodeCollection;
        }
    }
}

