using System;
using System.CodeDom;
using EaiConverter.Builder.Utils;
using System.Reflection;
using System.Collections.Generic;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    public class ModuleBuilder
    {
        public const string fieldName = "serviceManager";
        public const string registerMethodName = "RegisterServices";

        private static Dictionary<string,string> myServiceDisctionnary = new Dictionary<string,string>();

        public CodeNamespace Build()
        {
            var serviceNameSpace = new CodeNamespace(TargetAppNameSpaceService.MyAppName);
            serviceNameSpace.Imports.AddRange(this.GenerateImports());

            var serviceToGenerate = new CodeTypeDeclaration();
            serviceToGenerate.IsClass = true;
            serviceToGenerate.TypeAttributes = TypeAttributes.Public;

            serviceToGenerate.Name = "MyAppModule";

            serviceToGenerate.Members.Add(this.GenererateField());
            serviceToGenerate.Members.Add(this.GenererateContructor());
            serviceToGenerate.Members.Add(this.GenererateRegisterServicesMethod());
            serviceNameSpace.Types.Add(serviceToGenerate);

            return serviceNameSpace;
        }

        private CodeNamespaceImport[] GenerateImports()
        {
            var imports = new List<CodeNamespaceImport>
                {
                    new CodeNamespaceImport("System"),
                    new CodeNamespaceImport(TargetAppNameSpaceService.domainContractNamespaceName()),
                    new CodeNamespaceImport(TargetAppNameSpaceService.dataAccessNamespace()),
                    new CodeNamespaceImport(TargetAppNameSpaceService.domainServiceNamespaceName()),
                    new CodeNamespaceImport(TargetAppNameSpaceService.EngineCommandNamespace()),
                    new CodeNamespaceImport(TargetAppNameSpaceService.dataAccessNamespace()),
                    new CodeNamespaceImport(TargetAppNameSpaceService.EventSourcingNameSpace()),
                    new CodeNamespaceImport(TargetAppNameSpaceService.loggerNameSpace()),
                    new CodeNamespaceImport(TargetAppNameSpaceService.xmlToolsNameSpace())
                };

            return imports.ToArray();
        }

        private CodeMemberField GenererateField()
        {
            var field = new CodeMemberField
            {
                Name = fieldName,
                    Type = new CodeTypeReference(ServiceManagerInterfaceBuilder.serviceManagerInterfaceName),
                Attributes = MemberAttributes.Private
            };
            
            return field;
        }

        private CodeConstructor GenererateContructor()
        {
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            constructor.Parameters.Add(
                new CodeParameterDeclarationExpression() { Type = new CodeTypeReference(ServiceManagerInterfaceBuilder.serviceManagerInterfaceName), Name = fieldName });

            var parameterReference = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), fieldName );

            constructor.Statements.Add(new CodeAssignStatement(parameterReference,
                new CodeArgumentReferenceExpression(fieldName )));
        
            return constructor;
        }


        private CodeMemberMethod GenererateRegisterServicesMethod()
        {
            var registerMethod = new CodeMemberMethod();
            registerMethod.Name = registerMethodName;
            registerMethod.ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid);
            registerMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            var registerServiceFieldReference = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), fieldName );

            foreach (var elements in myServiceDisctionnary)
            {
                var methodArgumentReferences = new CodeArgumentReferenceExpression [2];
                methodArgumentReferences[0] = new CodeArgumentReferenceExpression(elements.Key);
                methodArgumentReferences[1] = new CodeArgumentReferenceExpression(elements.Value);

                var invocationExpression = new CodeMethodInvokeExpression(registerServiceFieldReference, ServiceManagerInterfaceBuilder.registerServiceMethodName, methodArgumentReferences);

                registerMethod.Statements.Add(invocationExpression);
            }
            return registerMethod;
        }

        public static void AddServiceToRegister(string interfaceName, string implementationName)
        {
            if (!myServiceDisctionnary.ContainsKey(interfaceName))
            {
                myServiceDisctionnary.Add(interfaceName, implementationName);
            }
        }
    }
}

