namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;


    public class EngineCommandActivityBuilder : IActivityBuilder
    {
        private readonly XslBuilder xslBuilder;
        private readonly EngineCommandServiceHelperBuilder engineCommandServiceHelperBuilder;
		private readonly XsdBuilder xsdBuilder;

        public EngineCommandActivityBuilder(XslBuilder xslBuilder, EngineCommandServiceHelperBuilder engineCommandServiceHelperBuilder, XsdBuilder xsdBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.engineCommandServiceHelperBuilder = engineCommandServiceHelperBuilder;
			this.xsdBuilder = xsdBuilder;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            var result = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsEngineCommandServiceAlreadyGenerated") != "true")
            {
                result.AddRange(this.engineCommandServiceHelperBuilder.Build());
                ConfigurationApp.SaveProperty("IsEngineCommandServiceAlreadyGenerated", "true");
            }

            return result;
        }
 
		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>
			           {
			               new CodeNamespaceImport(TargetAppNameSpaceService.EngineCommandNamespace)
			           };
		}

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(), GetServiceFieldName())
			};

			return parameters;
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName());

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName()))
			};

			return statements;
        }

        public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
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

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var engineCommandActivity = (EngineCommandActivity) activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add log at the beginning
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(engineCommandActivity));

            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(engineCommandActivity.InputBindings));

            // Add the invocation itself

            var variableName = VariableHelper.ToVariableName(engineCommandActivity.Name);

            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetServiceFieldName());

            var codeInvocation = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(activityServiceReference, engineCommandActivity.Command));

            var code = new CodeVariableDeclarationStatement(new CodeTypeReference("var"), variableName, codeInvocation);

            invocationCodeCollection.Add(code);
            return invocationCodeCollection;
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

