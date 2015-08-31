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
            var classes = new CodeNamespace[2]
                              {
                                  this.GenerateSubscriberInterface(),
                                  this.GenerateResponseReceivedEventHandler()
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

            var event1 = new CodeMemberEvent();

            // Sets a name for the event.
            event1.Name = "ResponseReceived";

            // Sets the type of event.
            event1.Type = new CodeTypeReference("EventHandler");

            subscriberInterfaceClass.Members.Add(event1);
            subscriberInterfaceClass.Members.Add(
                CodeDomUtils.GeneratePropertyWithoutSetter("WaitingTimeLimit", CSharpTypeConstant.SystemInt32));
            subscriberInterfaceClass.Members.Add(
                CodeDomUtils.GeneratePropertyWithoutSetter("IsStarted", CSharpTypeConstant.SystemBoolean));

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

            subscriberInterfaceNamespace.Types.Add(subscriberInterfaceClass);
            return subscriberInterfaceNamespace;
        }

        private CodeNamespace GenerateResponseReceivedEventHandler()
        {
            return new CodeNamespace();
        }
        
    }
}