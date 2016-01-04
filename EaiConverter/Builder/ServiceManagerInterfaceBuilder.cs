using System.CodeDom;
using EaiConverter.Builder.Utils;
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

            var serviceToGenerate = new CodeTypeDeclaration
                                        {
                                            IsInterface = true,
                                            IsClass = false,
                                            Name = serviceManagerInterfaceName
                                        };

            var voidReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid);

            var registerServiceMethod = new CodeMemberMethod
                                            {
                                                Name = registerServiceMethodName, ReturnType = voidReturnType,
                                                Attributes = MemberAttributes.Final | MemberAttributes.Public
                                            };

            var tInterfaceTypeParameter = new CodeTypeParameter("TInterface");
            var tConcreteTypeParameter = new CodeTypeParameter("TConcreteType");

            registerServiceMethod.TypeParameters.Add(tInterfaceTypeParameter);
            registerServiceMethod.TypeParameters.Add(tConcreteTypeParameter);

            serviceToGenerate.Members.Add (registerServiceMethod);

            serviceNameSpace.Types.Add(serviceToGenerate);

            return serviceNameSpace;

        }
    }
}

