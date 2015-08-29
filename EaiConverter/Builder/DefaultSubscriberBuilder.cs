using System;
using System.CodeDom;
using EaiConverter.Processor;
using EaiConverter.Model;
using EaiConverter.Builder.Utils;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class DefaultSubscriberBuilder : IActivityBuilder
    {
        private readonly SubscriberInterfaceBuilder subscriberBuilder;

        public DefaultSubscriberBuilder(SubscriberInterfaceBuilder subscriberBuilder)
        {
            this.subscriberBuilder = subscriberBuilder;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            var namespaces = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsSubscriberInterfaceAlreadyGenerated") != "true")
            {
                namespaces.AddRange(this.subscriberBuilder.GenerateClasses());
                ConfigurationApp.SaveProperty("IsSubscriberInterfaceAlreadyGenerated", "true");
            }

            return namespaces;
        }
            

        public System.CodeDom.CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            return new CodeStatementCollection();
        }

        public List<System.CodeDom.CodeNamespaceImport> GenerateImports(Activity activity)
        {
            return new List<CodeNamespaceImport>
            {
                new CodeNamespaceImport(TargetAppNameSpaceService.EventSourcingNameSpace),
                new CodeNamespaceImport("System.Threading")
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
                    new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName())),
                    new CodeSnippetStatement ("this."+ GetServiceFieldName() + ".ResponseReceived += this.OnEvent;")

                };

            return statements;
        }

        public List<System.CodeDom.CodeMemberField> GenerateFields(Activity activity)
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

        CodeTypeReference GetServiceFieldType()
        {
            return new CodeTypeReference(Builder.SubscriberInterfaceBuilder.InterfaceSubscriberName);
        }

        string GetServiceFieldName ()
        {
            return Builder.SubscriberInterfaceBuilder.Subscriber;
        }
    }
}

