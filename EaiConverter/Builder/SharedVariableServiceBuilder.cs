using System;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.Processor;
using EaiConverter.Builder.Utils;
using System.Reflection;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
	public class SharedVariableServiceBuilder
	{
        public const string GetMethodName = "Get";
        public const string SetMethodName = "Set";

        public const string ISharedVariableServiceName = "ISharedVariableService";
        public const string SharedVariableServiceName = "SharedVariableService";

        public CodeNamespaceCollection Build()
        {
            var sharedVariableServiceNameSpace = new CodeNamespace(TargetAppNameSpaceService.sharedVariableNameSpace);

            // Generate the Service
            sharedVariableServiceNameSpace.Imports.AddRange(this.GenerateImports());
            var sharedVariableService = this.GenerateClass();
            sharedVariableServiceNameSpace.Types.Add(sharedVariableService);

            // Generate the corresponding interface
            var sharedVariableServiceInterfaceNameSpace = InterfaceExtractorFromClass.Extract(sharedVariableService, TargetAppNameSpaceService.sharedVariableNameSpace);

            return new CodeNamespaceCollection{sharedVariableServiceNameSpace, sharedVariableServiceInterfaceNameSpace};
        }

        public CodeNamespaceImport[] GenerateImports()
        {
            return new CodeNamespaceImport[1] {
                new CodeNamespaceImport("System")
            };
        }


        public CodeTypeDeclaration GenerateClass()
        {
            var xmlParserHelperService = new CodeTypeDeclaration(SharedVariableServiceName);
            xmlParserHelperService.IsClass = true;
            xmlParserHelperService.TypeAttributes = TypeAttributes.Public;
            xmlParserHelperService.BaseTypes.Add(new CodeTypeReference(ISharedVariableServiceName));

            xmlParserHelperService.Members.Add(this.GenerateGetMethod());
            xmlParserHelperService.Members.Add(this.GenerateSetMethod());

            return xmlParserHelperService;
        }


        public CodeMemberMethod GenerateGetMethod()
        {
            var getMethod = new CodeMemberMethod();

            getMethod.Name = GetMethodName;
            getMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;

            getMethod.ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemObject);
            //fromXmlMethod.Statements.Add();
            getMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                new CodeTypeReference(CSharpTypeConstant.SystemString), "paramName"));
            getMethod.Statements.Add(new CodeSnippetStatement(@"
        return new Object(); "));
            
            return getMethod;
        }

        public CodeMemberMethod GenerateSetMethod()
        {
            var setMethod = new CodeMemberMethod();

            setMethod.Name = SetMethodName;
            setMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;

            setMethod.ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid);
            //fromXmlMethod.Statements.Add();
            setMethod.Parameters.Add(new CodeParameterDeclarationExpression( new CodeTypeReference(CSharpTypeConstant.SystemString), "paramName"));
            setMethod.Parameters.Add(new CodeParameterDeclarationExpression( new CodeTypeReference(CSharpTypeConstant.SystemObject), "objectToSet"));


            return setMethod;
        }
	}


}

