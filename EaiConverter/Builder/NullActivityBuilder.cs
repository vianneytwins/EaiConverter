using EaiConverter.Model;

using System.CodeDom;
using System.Collections.Generic;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    public class NullActivityBuilder : IActivityBuilder
    {
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
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));

            return invocationCodeCollection;
        }


        public string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
    }
}

