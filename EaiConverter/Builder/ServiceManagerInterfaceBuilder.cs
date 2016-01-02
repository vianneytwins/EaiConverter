using System;
using System.CodeDom;
using EaiConverter.Builder.Utils;
using System.Reflection;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    public class ServiceManagerInterfaceBuilder
    {

        public const string serviceManagerInterfaceName = "IServiceManager";
        public const string registerServiceMethodName = "RegisterApplicationService";

        public CodeNamespace Build()
        {
            var serviceNameSpace = new CodeNamespace(TargetAppNameSpaceService.MyAppName);
            //serviceNameSpace.Imports.AddRange(this.GenerateImports());

            var serviceToGenerate = new CodeTypeDeclaration();
            serviceToGenerate.IsInterface = true;
            serviceToGenerate.TypeAttributes = TypeAttributes.Public;

            serviceToGenerate.Name = serviceManagerInterfaceName;

            var interfaceParam = new CodeParameterDeclarationExpression ("TInterface", "interfaceName");
            var implemParam = new CodeParameterDeclarationExpression ("TConcreteType", "implemParam");

            var parameters = new CodeParameterDeclarationExpressionCollection ();
            parameters.Add (interfaceParam);
            parameters.Add (implemParam);

            var voidReturnType = new CodeTypeReference (CSharpTypeConstant.SystemVoid);

            var registerServiceMethod = new CodeMemberMethod { Name = registerServiceMethodName, ReturnType = voidReturnType };
            registerServiceMethod.Parameters.AddRange (parameters);

            serviceToGenerate.Members.Add (registerServiceMethod);

            serviceNameSpace.Types.Add(serviceToGenerate);

            return serviceNameSpace;

        }
    }
}

