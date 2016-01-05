namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Reflection;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Utils;

    public class EngineCommandServiceHelperBuilder
    {
        public const string IEngineCommandServiceName = "IEngineCommandServiceName";
        public const string EngineCommandServiceName = "EngineCommandServiceName";

        public const string GetProcessInstanceInfoMethodName = "GetProcessInstanceInfo";
        public const string ProcessInstanceInfoclassName = "ProcessInstanceInfo";

        public const string GetProcessInstanceExceptionMethodName = "GetProcessInstanceExceptions";
        public const string ProcessInstanceExceptionClassName = "ProcessInstanceException";

        public const string returnType = "List<ProcessInstanceInfo>";

        public CodeNamespaceCollection Build()
        {
            var engineCommandNamespace = new CodeNamespace(TargetAppNameSpaceService.EngineCommandNamespace());

            // Generate the Service
            engineCommandNamespace.Imports.AddRange(this.GenerateImports());
            var engineCommandServiceClass = this.GenerateClass();
            engineCommandNamespace.Types.Add(engineCommandServiceClass);

            // Generate the corresponding interface
            var engineCommandInterface = InterfaceExtractorFromClass.Extract(engineCommandServiceClass, TargetAppNameSpaceService.EngineCommandNamespace());
            engineCommandInterface.Types.AddRange(this.GenerateReturnOutputClasses());

            ModuleBuilder.AddServiceToRegister(IEngineCommandServiceName, EngineCommandServiceName);
            return new CodeNamespaceCollection { engineCommandNamespace, engineCommandInterface };
        }


        public CodeNamespaceImport[] GenerateImports()
        {
            return new CodeNamespaceImport[2]
                       {
                           new CodeNamespaceImport("System"),
                           new CodeNamespaceImport("System.Collections.Generic"),
                       };
        }


        public CodeTypeDeclaration GenerateClass()
        {
            var xmlParserHelperService = new CodeTypeDeclaration(EngineCommandServiceName);
            xmlParserHelperService.IsClass = true;
            xmlParserHelperService.TypeAttributes = TypeAttributes.Public;
            xmlParserHelperService.BaseTypes.Add(new CodeTypeReference(IEngineCommandServiceName));

            xmlParserHelperService.Members.Add(this.GenerateGetProcessInstanceInfoMethod());
            xmlParserHelperService.Members.Add(this.GenerateGetProcessInstanceExceptionMethod());

            return xmlParserHelperService;
        }


        public CodeMemberMethod GenerateGetProcessInstanceInfoMethod()
        {
            var newMethod = new CodeMemberMethod();

            newMethod.Name = GetProcessInstanceInfoMethodName;
            newMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;

            newMethod.ReturnType = new CodeTypeReference(returnType);

            newMethod.Statements.Add(new CodeSnippetStatement(@"            return new List<ProcessInstanceInfo>(); "));

            return newMethod;
        }

        public CodeMemberMethod GenerateGetProcessInstanceExceptionMethod()
        {
            var newMethod = new CodeMemberMethod();

            newMethod.Name = GetProcessInstanceExceptionMethodName;
            newMethod.Attributes = MemberAttributes.Final | MemberAttributes.Public;

            newMethod.ReturnType = new CodeTypeReference("List<ProcessInstanceException>");

            newMethod.Statements.Add(new CodeSnippetStatement(@"            return new List<ProcessInstanceException>(); "));

            return newMethod;
        }

        public CodeTypeDeclaration[] GenerateReturnOutputClasses()
        {
            return new CodeTypeDeclaration[2]
                       {
                           this.GenerateProcessInfoClass(),
                           this.GenerateProcessExceptionClass()
                       };
        }

        private CodeTypeDeclaration GenerateProcessExceptionClass()
        {
            var processExceptionClass = new CodeTypeDeclaration(ProcessInstanceExceptionClassName);
            processExceptionClass.IsClass = true;
            processExceptionClass.TypeAttributes = TypeAttributes.Public;

            processExceptionClass.Members.Add(CodeDomUtils.GenerateProperty("TrackingId", CSharpTypeConstant.SystemString));
            processExceptionClass.Members.Add(CodeDomUtils.GenerateProperty("ProcessDefinitionName", CSharpTypeConstant.SystemString));

            return processExceptionClass;
        }

        private CodeTypeDeclaration GenerateProcessInfoClass()
        {
            var processExceptionClass = new CodeTypeDeclaration(ProcessInstanceInfoclassName);
            processExceptionClass.IsClass = true;
            processExceptionClass.TypeAttributes = TypeAttributes.Public;

            processExceptionClass.Members.Add(CodeDomUtils.GenerateProperty("TrackingId", CSharpTypeConstant.SystemString));
            processExceptionClass.Members.Add(CodeDomUtils.GenerateProperty("ProcessStarterName", CSharpTypeConstant.SystemString));
            processExceptionClass.Members.Add(CodeDomUtils.GenerateProperty("ProcessInstanceName", CSharpTypeConstant.SystemString));
            processExceptionClass.Members.Add(CodeDomUtils.GenerateProperty("MainProcessName", CSharpTypeConstant.SystemString));

            return processExceptionClass;
        }
    }
}
