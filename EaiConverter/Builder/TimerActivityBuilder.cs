namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Utils;

    public class TimerActivityBuilder : IActivityBuilder
    {
        private readonly SubscriberInterfaceBuilder subscriberBuilder;

        public TimerActivityBuilder(SubscriberInterfaceBuilder subscriberBuilder)
        {
            this.subscriberBuilder = subscriberBuilder;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(EaiConverter.Model.Activity activity)
		{
			var namespaces = new CodeNamespaceCollection();
			if (ConfigurationApp.GetProperty("IsSubscriberInterfaceAlreadyGenerated") != "true")
			{
                namespaces.AddRange(this.subscriberBuilder.GenerateClasses());
				ConfigurationApp.SaveProperty("IsSubscriberInterfaceAlreadyGenerated", "true");
			}

			if (ConfigurationApp.GetProperty("IsTimerSubscriberImplemAlreadyGenerated") != "true")
			{
				namespaces.Add(this.GenerateTimerSubscriberImplementation());
                ConfigurationApp.SaveProperty("IsTimerSubscriberImplemAlreadyGenerated", "true");
			}

			return namespaces;
		}

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
		{
			return new CodeStatementCollection();
		}

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
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
				new CodeParameterDeclarationExpression(this.GetServiceFieldType(), this.GetServiceFieldName())
			};

			return parameters;
		}

		public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
		{
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), this.GetServiceFieldName());

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(this.GetServiceFieldName())),
				new CodeSnippetStatement("this." + this.GetServiceFieldName() + ".ResponseReceived += this.OnEvent;")

			};

			return statements;
		}

		public List<CodeMemberField> GenerateFields(Activity activity)
		{
			var fields = new List<CodeMemberField>
			{
                new CodeMemberField
				{
					Type = this.GetServiceFieldType(),
					Name = this.GetServiceFieldName(),
					Attributes = MemberAttributes.Private
				}
			};

			return fields;
		}

		private CodeTypeReference GetServiceFieldType()
		{
			return new CodeTypeReference(SubscriberInterfaceBuilder.InterfaceSubscriberName);
		}


		private string GetServiceFieldName()
		{
            return "subscriber";
		}


        private CodeNamespace GenerateTimerSubscriberImplementation()
        {
            var timerSubscriberNamespace = new CodeNamespace();
            timerSubscriberNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace;
            timerSubscriberNamespace.Imports.Add(new CodeNamespaceImport("System"));

            var timerSubscriberClass = new CodeTypeDeclaration("TimerSubscriber");
            timerSubscriberClass.IsClass = true;
            timerSubscriberClass.Attributes = MemberAttributes.Public;
            timerSubscriberClass.BaseTypes.Add(SubscriberInterfaceBuilder.InterfaceSubscriberName);

            var event1 = new CodeMemberEvent();

            // Sets a name for the event.
            event1.Name = "ResponseReceived";
            event1.Attributes = MemberAttributes.Public;
            // Sets the type of event.
            event1.Type = new CodeTypeReference("EventHandler");

            timerSubscriberClass.Members.Add(event1);
            timerSubscriberClass.Members.Add(
                CodeDomUtils.GeneratePropertyWithoutSetter("WaitingTimeLimit", CSharpTypeConstant.SystemInt32));
            timerSubscriberClass.Members.Add(
                CodeDomUtils.GeneratePropertyWithoutSetter("IsStarted", CSharpTypeConstant.SystemBoolean));

            timerSubscriberClass.Members.Add(
                new CodeMemberMethod
                {
                    Name = "Start",
                    ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid),
                    Attributes = MemberAttributes.Public
                });
            timerSubscriberClass.Members.Add(
                new CodeMemberMethod
                {
                    Name = "Stop",
                    ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid),
                    Attributes = MemberAttributes.Public
                });
            timerSubscriberClass.Members.Add(
                new CodeMemberMethod
                {
                    Name = "Confirm",
                    ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid),
                    Attributes = MemberAttributes.Public
                });
            
            timerSubscriberNamespace.Types.Add(timerSubscriberClass);
            return timerSubscriberNamespace;
        }
	}

}

