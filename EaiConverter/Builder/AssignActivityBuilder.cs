using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
    public class AssignActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        public AssignActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public ActivityCodeDom Build (Activity activity)
        {
            var result = new ActivityCodeDom();

            result.ClassesToGenerate = this.GenerateClassesToGenerate(activity);
            result.InvocationCode = this.GenerateInvocationCode (activity);

            return result;
        }

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

        public CodeStatementCollection GenerateInvocationCode ( Activity activity){
            AssignActivity assignActivity = (AssignActivity) activity;

            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(assignActivity));

            invocationCodeCollection.AddRange(this.xslBuilder.Build(assignActivity.InputBindings));

            var variableToAssignReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(assignActivity.VariableName));
            var codeInvocation = new CodeAssignStatement (variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToVariableName(assignActivity.VariableName)));
            invocationCodeCollection.Add(codeInvocation);
            return invocationCodeCollection;
        }
    }
}

