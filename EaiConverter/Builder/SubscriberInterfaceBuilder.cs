namespace EaiConverter.Builder
{
    using System.CodeDom;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Utils;

    public class SubscriberInterfaceBuilder
    {
        public const string InterfaceSubscriberName = "ISubscriber";

        public const string Subscriber = "subscriber";
        
        public CodeNamespace[] GenerateClasses()
        {
            var classes = new CodeNamespace[1]
                              {
                                  this.GenerateSubscriberInterface()
                              };

            return classes;
        }

        private CodeNamespace GenerateSubscriberInterface()
        {
            var subscriberInterfaceNamespace = new CodeNamespace();
            subscriberInterfaceNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace;
            subscriberInterfaceNamespace.Imports.Add(new CodeNamespaceImport("System"));

            var subscriberInterfaceClass = new CodeTypeDeclaration(InterfaceSubscriberName);
            subscriberInterfaceClass.IsInterface = true;

            subscriberInterfaceClass.Members.Add(
                CodeDomUtils.GeneratePropertyWithoutSetterForInterface("WaitingTimeLimit", CSharpTypeConstant.SystemInt32));
            subscriberInterfaceClass.Members.Add(
                CodeDomUtils.GeneratePropertyWithoutSetterForInterface("IsStarted", CSharpTypeConstant.SystemBoolean));

            var snippet = new CodeSnippetTypeMember();
            snippet.Text = "        event EventHandler ResponseReceived;";
            subscriberInterfaceClass.Members.Add(snippet);

            subscriberInterfaceClass.Members.Add(
                new CodeMemberMethod
                    {
                        Name = "Start",
                        ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid)
                    });
            subscriberInterfaceClass.Members.Add(
                new CodeMemberMethod
                    {
                        Name = "Stop",
                        ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid)
                    });
            subscriberInterfaceClass.Members.Add(
                new CodeMemberMethod
                {
                    Name = "Confirm",
                    ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid)
                });
            
            subscriberInterfaceNamespace.Types.Add(subscriberInterfaceClass);
            return subscriberInterfaceNamespace;
        }

        
    }
}