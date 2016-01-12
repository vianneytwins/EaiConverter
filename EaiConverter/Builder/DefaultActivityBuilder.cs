namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class DefaultActivityBuilder : AbstractActivityBuilder
    {
        public override List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>
			{
				new CodeNamespaceImport (TargetAppNameSpaceService.domainContractNamespaceName())
			};
		}

        public override CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
		{
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(activity), GetServiceFieldName(activity))
			};

			return parameters;
		}

        public override CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
		{
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName(activity));

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName(activity)))
			};

			return statements;
		}

        public override List<CodeMemberField> GenerateFields(Activity activity)
		{
		    var fields = new List<CodeMemberField>
		                     {
		                         new CodeMemberField
		                             {
		                                 Type = GetServiceFieldType(activity),
		                                 Name = GetServiceFieldName(activity),
		                                 Attributes = MemberAttributes.Private
		                             }
		                     };

		    return fields;
		}

        private CodeStatementCollection GenerateCoreMethod(Activity activity)
        {
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(activity.Name));
            var methodInvocation = new CodeMethodInvokeExpression(activityServiceReference, "Execute", new CodeExpression[] { });
            var invocationCodeCollection = new CodeStatementCollection { methodInvocation };
            return invocationCodeCollection;
        }

        private static CodeTypeReference GetServiceFieldType(Activity activity)
        {
            return new CodeTypeReference("I" + VariableHelper.ToClassName(activity.Name + "Service"));
        }

        private static string GetServiceFieldName(Activity activity)
        {
            return VariableHelper.ToVariableName(VariableHelper.ToClassName(activity.Name + "Service"));
        }

        public override string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }

        public override List<CodeMemberMethod> GenerateMethods (Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethods(activity, variables);
            activityMethod[0].Statements.AddRange(this.GenerateCoreMethod(activity));

            return activityMethod;
        }
    }
}

