using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;

    public class DefaultSubscriberBuilder : IActivityBuilder
    {
        private readonly SubscriberInterfaceBuilder subscriberBuilder;

        public DefaultSubscriberBuilder(SubscriberInterfaceBuilder subscriberBuilder)
        {
            this.subscriberBuilder = subscriberBuilder;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables )
        {
            var namespaces = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsSubscriberInterfaceAlreadyGenerated") != "true")
            {
                namespaces.AddRange(this.subscriberBuilder.GenerateClasses());
                ConfigurationApp.SaveProperty("IsSubscriberInterfaceAlreadyGenerated", "true");
            }

            return namespaces;
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity, Dictionary<string, string> variables)
        {
            return new CodeStatementCollection();
        }

        public CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            return new CodeMemberMethod();
        }


        public List<System.CodeDom.CodeNamespaceImport> GenerateImports(Activity activity)
        {
            return new List<CodeNamespaceImport>
            {
                new CodeNamespaceImport(TargetAppNameSpaceService.EventSourcingNameSpace()),
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
                    new CodeSnippetStatement("this."+ GetServiceFieldName() + ".ResponseReceived += this.OnEvent;")

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

        string GetServiceFieldName()
        {
            return "subscriber";
        }

        public string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemObject;
        }
    }
}

