namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Model;

    public interface IActivityBuilder
    {
        CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables);

        CodeStatementCollection GenerateInvocationCode(Activity activity, Dictionary<string, string> variables);

        List<CodeMemberMethod> GenerateMethods(Activity activity, Dictionary<string, string> variables);

        List<CodeNamespaceImport> GenerateImports(Activity activity);

        CodeParameterDeclarationExpressionCollection GenerateConstructorParameter (Activity activity);

        CodeStatementCollection GenerateConstructorCodeStatement(Activity activity);

        List<CodeMemberField> GenerateFields(Activity activity);

        string GetReturnType(Activity activity);

    }
}

