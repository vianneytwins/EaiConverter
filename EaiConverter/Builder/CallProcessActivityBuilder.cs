using EaiConverter.Model;

using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;
using EaiConverter.Builder.Utils;

namespace EaiConverter.Builder
{
    public class CallProcessActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;

        public CallProcessActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }


        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
        }


		public List<CodeNamespaceImport> GenerateImports(Activity activity)
        {
			var import4Activities = new List<CodeNamespaceImport>();
			var callProcessActivity = (CallProcessActivity)activity;
			import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.ConvertXsdImportToNameSpace(callProcessActivity.TibcoProcessToCall.ShortNameSpace)));
			import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.ConvertXsdImportToNameSpace(callProcessActivity.TibcoProcessToCall.InputAndOutputNameSpace)));

			return import4Activities;
        }

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
			var callProcessActivity = (CallProcessActivity)activity;
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(callProcessActivity), GetServiceFieldName(callProcessActivity))
			};

			return parameters;
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
			var callProcessActivity = (CallProcessActivity)activity;
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName(callProcessActivity));

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName(callProcessActivity)))
			};

			return statements;
        }

        public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
        {
			var callProcessActivity = (CallProcessActivity)activity;
			return new List<CodeMemberField>
			{
				new CodeMemberField
				{
					Type = GetServiceFieldType(callProcessActivity),
					Name = GetServiceFieldName(callProcessActivity),
					Attributes = MemberAttributes.Private
				}
			};
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            CallProcessActivity callProcessActivity = (CallProcessActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();
            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(callProcessActivity));
            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(callProcessActivity.InputBindings));

            // Add the invocation
            var processToCallReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(callProcessActivity.TibcoProcessToCall.ProcessName));

            var parameters = DefaultActivityBuilder.GenerateParameters(callProcessActivity);

            // TODO : WARNING not sure the start method ProcessName is indeed START
            var methodInvocation = new CodeMethodInvokeExpression(processToCallReference, "Start", parameters);

            var code = new CodeVariableDeclarationStatement("var", VariableHelper.ToVariableName(callProcessActivity.Name), methodInvocation);

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }

		private static CodeTypeReference GetServiceFieldType (CallProcessActivity callProcessActivity)
		{
			return new CodeTypeReference(VariableHelper.ToClassName(callProcessActivity.ProcessName));
		}

		private static string GetServiceFieldName(CallProcessActivity callProcessActivity)
		{
			return VariableHelper.ToVariableName(VariableHelper.ToClassName(callProcessActivity.ProcessName));
		}

		private string TargetNamespace (Activity activity)
		{
			return TargetAppNameSpaceService.domainContractNamespaceName + "." + VariableHelper.ToClassName(activity.Name); 
		}
    }
}