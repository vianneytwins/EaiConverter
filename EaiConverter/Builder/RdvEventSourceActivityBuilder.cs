namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Utils;

    public class RdvEventSourceActivityBuilder : IActivityBuilder
	{
        private readonly SubscriberInterfaceBuilder subscriberBuilder;

        public RdvEventSourceActivityBuilder(SubscriberInterfaceBuilder subscriberBuilder)
        {
            this.subscriberBuilder = subscriberBuilder;
        }


        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables)
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
            subscriberRdvClassNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace();
			subscriberRdvClassNamespace.Imports.Add (new CodeNamespaceImport("System"));

			var subscriberRdvClass = new CodeTypeDeclaration("TibcoPublisher");
			subscriberRdvClass.IsClass = true;
			subscriberRdvClass.Attributes = MemberAttributes.Public;
			subscriberRdvClass.BaseTypes.Add(SubscriberInterfaceBuilder.Subscriber);

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
            subscriberRdvClass.Members.Add(
                new CodeMemberMethod
                {
                    Name = "Confirm",
                    ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid),
                    Attributes = MemberAttributes.Public
                });
            
			subscriberRdvClassNamespace.Types.Add (subscriberRdvClass);
			return subscriberRdvClassNamespace;
		}

        public CodeStatementCollection GenerateInvocationCode(Activity activity, Dictionary<string, string> variables)
		{
			return new CodeStatementCollection();
		}

        public CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            return new CodeMemberMethod();
        }

        public List<CodeNamespaceImport> GenerateImports(Activity activity)
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
			return new CodeTypeReference(Builder.SubscriberInterfaceBuilder.InterfaceSubscriberName);
		}

		string GetServiceFieldName ()
		{
			return Builder.SubscriberInterfaceBuilder.Subscriber;
		}

        public string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemObject;
        }
	}

}

