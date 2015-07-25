using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class AssignActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        public AssignActivityBuilder(XslBuilder xslBuilder)
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

