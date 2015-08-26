namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Utils;

    public class RdvEventSourceActivityBuilder : IActivityBuilder
	{
        private readonly SubscriberBuilder subscriberBuilder;

        public RdvEventSourceActivityBuilder(SubscriberBuilder subscriberBuilder)
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

			if (ConfigurationApp.GetProperty("IsTibcoSubscriberImplemAlreadyGenerated") != "true")
			{
				namespaces.Add (this.GenerateTibcoSubscriberImplementation ());
				ConfigurationApp.SaveProperty("IsTibcoSubscriberImplemAlreadyGenerated", "true");
			}

			return namespaces;
		}

		CodeNamespace GenerateTibcoSubscriberImplementation()
		{
			var subscriberRdvClassNamespace = new CodeNamespace();
			subscriberRdvClassNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace;
			subscriberRdvClassNamespace.Imports.Add (new CodeNamespaceImport("System"));

			var subscriberRdvClass = new CodeTypeDeclaration("TibcoPublisher");
			subscriberRdvClass.IsClass = true;
			subscriberRdvClass.Attributes = MemberAttributes.Public;
			subscriberRdvClass.BaseTypes.Add(SubscriberBuilder.Subscriber);

			CodeMemberEvent event1 = new CodeMemberEvent();
			// Sets a name for the event.
			event1.Name = "ResponseReceived";
			// Sets the type of event.
			event1.Type = new CodeTypeReference("ResponseReceivedEventHandler");

			subscriberRdvClass.Members.Add (event1);
			subscriberRdvClass.Members.Add (CodeDomUtils.GeneratePropertyWithoutSetter ("WaitingTimeLimit",CSharpTypeConstant.SystemInt32));
			subscriberRdvClass.Members.Add (CodeDomUtils.GeneratePropertyWithoutSetter ("IsStarted",CSharpTypeConstant.SystemBoolean));

			subscriberRdvClass.Members.Add(new CodeMemberMethod {
				Name = "Start",
				ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid),
				Attributes = MemberAttributes.Public
			});
			subscriberRdvClass.Members.Add(new CodeMemberMethod {
				Name = "Stop",
				ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid),
				Attributes = MemberAttributes.Public
			});

			subscriberRdvClassNamespace.Types.Add (subscriberRdvClass);
			return subscriberRdvClassNamespace;
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
			return new CodeTypeReference(Builder.SubscriberBuilder.InterfaceSubscriberName);
		}

		string GetServiceFieldName ()
		{
			return Builder.SubscriberBuilder.Subscriber;
		}
	}

}

