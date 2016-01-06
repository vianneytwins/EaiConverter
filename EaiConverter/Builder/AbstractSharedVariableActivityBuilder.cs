namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Utils;

    public abstract class AbstractSharedVariableActivityBuilder : AbstractActivityBuilder
	{
        private SharedVariableServiceBuilder sharedVariableServiceBuilder;

        public AbstractSharedVariableActivityBuilder()
        {
            this.sharedVariableServiceBuilder = new SharedVariableServiceBuilder();
        }

        public override CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables)
		{
            var result = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsSharedVariableServiceAlreadyGenerated") != "true")
            {
                result.AddRange(this.sharedVariableServiceBuilder.Build());
                ConfigurationApp.SaveProperty("IsSharedVariableServiceAlreadyGenerated", "true");
            }

            return result;
		}

        public override List<CodeNamespaceImport> GenerateImports(Activity activity)
        {
            return new List<CodeNamespaceImport> {new CodeNamespaceImport(TargetAppNameSpaceService.sharedVariableNameSpace())};
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
            var parameterReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), GetServiceFieldName());

            var statements = new CodeStatementCollection
                {
                    new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName()))
                };

            return statements;
        }

        public override List<CodeMemberField> GenerateFields(Activity activity)
        {
            var fields = new List<CodeMemberField>
                {new CodeMemberField
                    {
                        Type = GetServiceFieldType(),
                        Name = GetServiceFieldName(),
                        Attributes = MemberAttributes.Private
                    }
                };

            return fields;
        }

        private static CodeTypeReference GetServiceFieldType()
        {
            return new CodeTypeReference(SharedVariableServiceBuilder.ISharedVariableServiceName);
        }

        private static string GetServiceFieldName()
        {
            return VariableHelper.ToVariableName(VariableHelper.ToClassName (SharedVariableServiceBuilder.SharedVariableServiceName));
        }
          
        public override string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemObject;
        }



	}

}

