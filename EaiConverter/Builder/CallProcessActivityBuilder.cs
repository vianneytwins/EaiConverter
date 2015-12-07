namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

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
			import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.RemoveFirstDot(callProcessActivity.TibcoProcessToCall.ShortNameSpace)));
            if (IsTheProcessInputRequiresAnImport(callProcessActivity))
            {
                import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.RemoveFirstDot(callProcessActivity.TibcoProcessToCall.InputAndOutputNameSpace)));
            }

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
            if (IsTheProcessInputRequiresAnImport(callProcessActivity))
            {
                invocationCodeCollection.AddRange(
                    this.xslBuilder.Build(
                        callProcessActivity.TibcoProcessToCall.InputAndOutputNameSpace,
                        callProcessActivity.InputBindings));
            }
            else
            {
                invocationCodeCollection.AddRange(
                    this.xslBuilder.Build(callProcessActivity.InputBindings));
            }

            // Add the invocation
            var processToCallReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(callProcessActivity.TibcoProcessToCall.ProcessName));

            var parameters = DefaultActivityBuilder.GenerateParameters(callProcessActivity);

            // TODO : WARNING not sure the start method ProcessName is indeed START
            var methodInvocation = new CodeMethodInvokeExpression(processToCallReference, "Start", parameters);

            var code = new CodeVariableDeclarationStatement("var", VariableHelper.ToVariableName(callProcessActivity.Name), methodInvocation);

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }

        private static bool IsTheProcessInputRequiresAnImport(CallProcessActivity callProcessActivity)
        {
            return callProcessActivity.InputBindings != null && callProcessActivity.InputBindings.Count() != 0 && String.IsNullOrEmpty(((XElement)callProcessActivity.InputBindings.First()).Name.Namespace.ToString());
        }

        private static CodeTypeReference GetServiceFieldType(CallProcessActivity callProcessActivity)
		{
            return new CodeTypeReference(callProcessActivity.TibcoProcessToCall.ShortNameSpace + ".I" + VariableHelper.ToClassName(callProcessActivity.TibcoProcessToCall.ProcessName));
		}

		private static string GetServiceFieldName(CallProcessActivity callProcessActivity)
		{
			return VariableHelper.ToVariableName(callProcessActivity.TibcoProcessToCall.ProcessName);
		}

    }
}