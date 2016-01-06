namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;


    public class EngineCommandActivityBuilder : AbstractActivityBuilder
    {
        private readonly XslBuilder xslBuilder;
        private readonly EngineCommandServiceHelperBuilder engineCommandServiceHelperBuilder;

        public EngineCommandActivityBuilder(XslBuilder xslBuilder, EngineCommandServiceHelperBuilder engineCommandServiceHelperBuilder, XsdBuilder xsdBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.engineCommandServiceHelperBuilder = engineCommandServiceHelperBuilder;
        }

        public override CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables)
        {
            var result = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsEngineCommandServiceAlreadyGenerated") != "true")
            {
                result.AddRange(this.engineCommandServiceHelperBuilder.Build());
                ConfigurationApp.SaveProperty("IsEngineCommandServiceAlreadyGenerated", "true");
            }

            return result;
        }

        public override List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>
			           {
			               new CodeNamespaceImport(TargetAppNameSpaceService.EngineCommandNamespace())
			           };
		}

        public override CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(), GetServiceFieldName())
			};

			return parameters;
        }

        public override CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName());

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName()))
			};

			return statements;
        }

        public override System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
        {
			var fields = new List<CodeMemberField>
			{
                new CodeMemberField
				{
					Type = GetServiceFieldType(),
					Name = GetServiceFieldName(),
					Attributes = MemberAttributes.Private
				}
			};

			return fields;
        }

        public override CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethod(activity, variables);
            var engineCommandActivity = (EngineCommandActivity) activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(engineCommandActivity.InputBindings));

            // Add the invocation itself
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetServiceFieldName());

            var codeInvocation = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(activityServiceReference, engineCommandActivity.Command));

            var code = new CodeMethodReturnStatement(codeInvocation);
            invocationCodeCollection.Add(code);
            activityMethod.Statements.AddRange(invocationCodeCollection);

            return activityMethod;
        }

        public override string GetReturnType(Activity activity)
        {
            return EngineCommandServiceHelperBuilder.returnType;
        }

		private static CodeTypeReference GetServiceFieldType()
		{
            return new CodeTypeReference(EngineCommandServiceHelperBuilder.IEngineCommandServiceName);
		}

		private static string GetServiceFieldName()
		{
            return VariableHelper.ToVariableName(EngineCommandServiceHelperBuilder.EngineCommandServiceName);
		}

    }
}

