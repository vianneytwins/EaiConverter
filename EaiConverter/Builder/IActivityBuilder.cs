using EaiConverter.Model;
using System.CodeDom;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public interface IActivityBuilder
    {
        CodeNamespaceCollection GenerateClassesToGenerate (Activity activity);

        CodeStatementCollection GenerateInvocationCode (Activity activity);

        List<CodeNamespaceImport> GenerateImports (Activity activity);

        CodeParameterDeclarationExpressionCollection GenerateConstructorParameter (Activity activity);

        CodeStatementCollection GenerateConstructorCodeStatement(Activity activity);

        List<CodeMemberField> GenerateFields(Activity activity);

    }
}

