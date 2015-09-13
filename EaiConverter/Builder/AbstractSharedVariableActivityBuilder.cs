using System;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.Processor;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Builder.Utils;

namespace EaiConverter.Builder
{
	public abstract class AbstractSharedVariableActivityBuilder : IActivityBuilder
	{
        private SharedVariableServiceBuilder sharedVariableServiceBuilder;

        public AbstractSharedVariableActivityBuilder()
        {
            this.sharedVariableServiceBuilder = new SharedVariableServiceBuilder();
        }

		public CodeNamespaceCollection GenerateClassesToGenerate (Activity activity)
		{
            var result = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsSharedVariableServiceAlreadyGenerated") != "true")
            {
                result.AddRange(this.sharedVariableServiceBuilder.Build());
                ConfigurationApp.SaveProperty("IsSharedVariableServiceAlreadyGenerated", "true");
            }

            return result;
		}


        public List<CodeNamespaceImport> GenerateImports(Activity activity)
        {
            return new List<CodeNamespaceImport>{new CodeNamespaceImport(TargetAppNameSpaceService.sharedVariableNameSpace)};
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
                {new CodeMemberField
                    {
                        Type = GetServiceFieldType(),
                        Name = GetServiceFieldName(),
                        Attributes = MemberAttributes.Private
                    }
                };

            return fields;
        }


        public abstract CodeStatementCollection GenerateInvocationCode(Activity activity);

        private static CodeTypeReference GetServiceFieldType()
        {
            return new CodeTypeReference(SharedVariableServiceBuilder.ISharedVariableServiceName);
        }

        private static string GetServiceFieldName()
        {
            return VariableHelper.ToVariableName(VariableHelper.ToClassName (SharedVariableServiceBuilder.SharedVariableServiceName));
        }
          
		
		
	}

}

