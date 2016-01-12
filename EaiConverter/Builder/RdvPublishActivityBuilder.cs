namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;
    using EaiConverter.Utils;

    public class RdvPublishActivityBuilder : AbstractActivityBuilder
	{
        public const string IsPublisherInterfaceAlreadyGenerated = "IsPublisherInterfaceAlreadyGenerated";
		public const string IsTibcoPublisherImplemAlreadyGenerated = "IsTibcoPublisherImplemAlreadyGenerated";
        private const string InterfaceSubscriberName = "IPublisher";
        private const string ImplementationName = "RdvPublisher";
        private readonly XslBuilder xslBuilder;

        public RdvPublishActivityBuilder(XslBuilder xslBuilder)
		{
			this.xslBuilder = xslBuilder;
		}

        public override CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables)
		{
			var namespaces = new CodeNamespaceCollection ();
			if (ConfigurationApp.GetProperty(IsPublisherInterfaceAlreadyGenerated) != "true")
			{
				namespaces.Add(this.GeneratePublisherInterface());
				ConfigurationApp.SaveProperty (IsPublisherInterfaceAlreadyGenerated, "true");
			}
				
			if (ConfigurationApp.GetProperty(IsTibcoPublisherImplemAlreadyGenerated) != "true")
			{
				namespaces.Add(this.GenerateTibcoPublisherImplementation());
				ConfigurationApp.SaveProperty(IsTibcoPublisherImplemAlreadyGenerated, "true");
			}

            ModuleBuilder.AddServiceToRegister(InterfaceSubscriberName, ImplementationName);
			return namespaces;
		}

		public CodeNamespace GeneratePublisherInterface()
		{
			var publisherInterfaceNamespace = new CodeNamespace { Name = TargetAppNameSpaceService.EventSourcingNameSpace() };
		    publisherInterfaceNamespace.Imports.Add(new CodeNamespaceImport("System"));

			var publisherInterfaceClass = new CodeTypeDeclaration(InterfaceSubscriberName) { IsInterface = true };

		    var sendMethod = this.GenerateSendMethod();

			publisherInterfaceClass.Members.Add(sendMethod);

			publisherInterfaceNamespace.Types.Add (publisherInterfaceClass);
			return publisherInterfaceNamespace;
		}

		public CodeMemberMethod GenerateSendMethod()
		{
			var sendMethod = new CodeMemberMethod
            {
				Name = "Send",
				ReturnType = new CodeTypeReference (CSharpTypeConstant.SystemVoid),
				Attributes = MemberAttributes.Public
			};
			sendMethod.Parameters.Add (new CodeParameterDeclarationExpression (new CodeTypeReference (CSharpTypeConstant.SystemString), "subject"));
            sendMethod.Parameters.Add (new CodeParameterDeclarationExpression (new CodeTypeReference (CSharpTypeConstant.SystemString), "message"));

			return sendMethod;
		}

        public override CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethod(activity, variables);
			var rdvPublishActivity = (RdvPublishActivity) activity;
			var invocationCodeCollection = new CodeStatementCollection();
            
			// Add the mapping
			invocationCodeCollection.AddRange(this.xslBuilder.Build(rdvPublishActivity.InputBindings));
            invocationCodeCollection.Add(new CodeSnippetStatement("string subject = \"" + rdvPublishActivity.Subject+ "\";"));

			// Add the invocation itself
			var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.GetServiceFieldName(activity));

            var initParameters = new List<string>
                {
                    "subject"
                };
			var parameters = GenerateParameters(initParameters, rdvPublishActivity);

			var codeInvocation = new CodeMethodInvokeExpression(activityServiceReference, "Send", parameters);
			invocationCodeCollection.Add(codeInvocation);

            activityMethod.Statements.AddRange(invocationCodeCollection);

            return activityMethod;
		}

		public override List<System.CodeDom.CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>
			{
				new CodeNamespaceImport(TargetAppNameSpaceService.EventSourcingNameSpace()),
			};
		}

        public override CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
		{
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(), GetServiceFieldName(activity))
			};

			return parameters;
		}

        public override CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
		{
			var parameterReference = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), this.GetServiceFieldName(activity));

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(this.GetServiceFieldName(activity)))
			};

			return statements;
		}

		public override List<CodeMemberField> GenerateFields(Activity activity)
		{
			var fields = new List<CodeMemberField>
			{
                new CodeMemberField
				{
					Type = this.GetServiceFieldType(),
					Name = this.GetServiceFieldName(activity),
					Attributes = MemberAttributes.Private
				}
			};

			return fields;
		}

        public override string GetReturnType(Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }

        private CodeTypeReference GetServiceFieldType()
		{
			return new CodeTypeReference (InterfaceSubscriberName);
		}

		private string GetServiceFieldName (Activity activity)
		{
			return VariableHelper.ToVariableName(activity.Name)+ ImplementationName;
		}

        private CodeNamespace GenerateTibcoPublisherImplementation()
        {
            var publisherRdvImplementationNamespace = new CodeNamespace();
            publisherRdvImplementationNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace();
            publisherRdvImplementationNamespace.Imports.Add(new CodeNamespaceImport("System"));

            var publisherImplementationClass = new CodeTypeDeclaration(ImplementationName) { IsClass = true };
            publisherImplementationClass.BaseTypes.Add(TargetAppNameSpaceService.EventSourcingNameSpace() + "." + InterfaceSubscriberName);

            var sendMethod = this.GenerateSendMethod();

            publisherImplementationClass.Members.Add(sendMethod);

            publisherRdvImplementationNamespace.Types.Add(publisherImplementationClass);
            return publisherRdvImplementationNamespace;
        }
	}

}

