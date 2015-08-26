namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

    public class DefaultActivityBuilder : IActivityBuilder
    {
        public DefaultActivityBuilder(XslBuilder xslbuilder)
        {}

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
        }

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>
			{
				new CodeNamespaceImport (TargetAppNameSpaceService.domainContractNamespaceName)
			};
		}

		public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
		{
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(activity), GetServiceFieldName(activity))
			};

			return parameters;
		}

		public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
		{
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName(activity));

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName(activity)))
			};

			return statements;
		}

		public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
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

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(activity.Name));
            var methodInvocation = new CodeMethodInvokeExpression(activityServiceReference, "Execute", new CodeExpression[] { });
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(LogActivity(activity));
            invocationCodeCollection.Add(methodInvocation);
            return invocationCodeCollection;
        }

        public static CodeStatementCollection LogActivity(Activity activity)
        {
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName("logger"));
            var methodInvocation = new CodeMethodInvokeExpression(
                activityServiceReference,
                "Info",
                new CodeExpression[]
                {
                    new CodePrimitiveExpression("Start Activity: " + activity.Name + " of type: " + activity.Type)
                });

            var logCallStatements = new CodeStatementCollection();
            logCallStatements.Add(methodInvocation);
            return logCallStatements;
        }

        public static CodeExpression[] GenerateParameters(List<string> existingParamaters, Activity activity)
        {
            var parameterLists = new List<CodeExpression> { };
            //Add existing Parameter
            if (existingParamaters != null)
            {
                foreach (var parameter in existingParamaters)
                {
                    parameterLists.Add(new CodeSnippetExpression(parameter));
                }
            }

            //Add Activity Paramters
            if (activity.Parameters != null)
            {
                foreach (var parameter in activity.Parameters)
                {
                    parameterLists.Add(new CodeSnippetExpression(parameter.Name));
                }
            }

            return parameterLists.ToArray();
        }

        public static CodeExpression[] GenerateParameters(Activity activity)
        {
            return GenerateParameters(null, activity);
        }

		private static CodeTypeReference GetServiceFieldType (Activity activity)
		{
			return new CodeTypeReference("I" + VariableHelper.ToClassName(activity.Name + "Service"));
		}

		private static string GetServiceFieldName(Activity activity)
		{
			return VariableHelper.ToVariableName(VariableHelper.ToClassName(activity.Name + "Service"));
		}
    }
}

