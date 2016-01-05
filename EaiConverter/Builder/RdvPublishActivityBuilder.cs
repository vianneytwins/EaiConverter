using System;
using EaiConverter.Model;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.Builder.Utils;
using EaiConverter.Processor;
using EaiConverter.Utils;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
	public class RdvPublishActivityBuilder : IActivityBuilder
	{
		private readonly XslBuilder xslBuilder;

		public const string interfaceSubscriberName = "IPublisher";

		public const string isPublisherInterfaceAlreadyGenerated = "IsPublisherInterfaceAlreadyGenerated";
		public const string isTibcoPublisherImplemAlreadyGenerated = "IsTibcoPublisherImplemAlreadyGenerated";

		public RdvPublishActivityBuilder(XslBuilder xslBuilder)
		{
			this.xslBuilder = xslBuilder;
		}

		public CodeNamespaceCollection GenerateClassesToGenerate (EaiConverter.Model.Activity activity)
		{
			var namespaces = new CodeNamespaceCollection ();
			if (ConfigurationApp.GetProperty(isPublisherInterfaceAlreadyGenerated) != "true")
			{
				namespaces.Add(this.GeneratePublisherInterface());
				ConfigurationApp.SaveProperty (isPublisherInterfaceAlreadyGenerated, "true");
			}
				
			if (ConfigurationApp.GetProperty(isTibcoPublisherImplemAlreadyGenerated) != "true")
			{
				namespaces.Add (this.GenerateTibcoPublisherImplementation ());
				ConfigurationApp.SaveProperty (isTibcoPublisherImplemAlreadyGenerated, "true");
			}
            ModuleBuilder.AddServiceToRegister(interfaceSubscriberName, "RdvPublisher");
			return namespaces;
		}

		public CodeNamespace GeneratePublisherInterface ()
		{
			var publisherInterfaceNamespace = new CodeNamespace ();
            publisherInterfaceNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace();
			publisherInterfaceNamespace.Imports.Add (new CodeNamespaceImport("System"));

			var publisherInterfaceClass = new CodeTypeDeclaration(interfaceSubscriberName);
			publisherInterfaceClass.IsInterface = true;

			var sendMethod = this.GenerateSendMethod ();

			publisherInterfaceClass.Members.Add(sendMethod);

			publisherInterfaceNamespace.Types.Add (publisherInterfaceClass);
			return publisherInterfaceNamespace;
		}

		CodeNamespace GenerateTibcoPublisherImplementation ()
		{
			var publisherRdvImplementationNamespace = new CodeNamespace ();
            publisherRdvImplementationNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace();
			publisherRdvImplementationNamespace.Imports.Add (new CodeNamespaceImport("System"));

			var publisherImplementationClass = new CodeTypeDeclaration("RdvPublisher");
			publisherImplementationClass.IsClass = true;
            publisherImplementationClass.BaseTypes.Add(TargetAppNameSpaceService.EventSourcingNameSpace() + "." + interfaceSubscriberName);

			var sendMethod = this.GenerateSendMethod();

			publisherImplementationClass.Members.Add(sendMethod);

			publisherRdvImplementationNamespace.Types.Add (publisherImplementationClass);
			return publisherRdvImplementationNamespace;
		}

		public CodeMemberMethod GenerateSendMethod ()
		{
			var sendMethod = new CodeMemberMethod {
				Name = "Send",
				ReturnType = new CodeTypeReference (CSharpTypeConstant.SystemVoid),
				Attributes = MemberAttributes.Public
			};
			sendMethod.Parameters.Add (new CodeParameterDeclarationExpression (new CodeTypeReference (CSharpTypeConstant.SystemString), "subject"));
            sendMethod.Parameters.Add (new CodeParameterDeclarationExpression (new CodeTypeReference (CSharpTypeConstant.SystemString), "message"));

			return sendMethod;
		}

		public System.CodeDom.CodeStatementCollection GenerateInvocationCode (EaiConverter.Model.Activity activity)
		{
			var rdvPublishActivity = (RdvPublishActivity) activity;
			var invocationCodeCollection = new CodeStatementCollection();

			// Add the Log
			invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(rdvPublishActivity));

			// Add the mapping
			invocationCodeCollection.AddRange(this.xslBuilder.Build(rdvPublishActivity.InputBindings));
            invocationCodeCollection.Add(new CodeSnippetStatement("string subject = \"" + rdvPublishActivity.Subject+ "\";"));

			// Add the invocation itself
			var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.GetServiceFieldName(activity));

            var initParameters = new List<string>
                {
                    "subject"
                };
			var parameters = DefaultActivityBuilder.GenerateParameters(initParameters, rdvPublishActivity);

			var codeInvocation = new CodeMethodInvokeExpression(activityServiceReference, "Send", parameters);
			invocationCodeCollection.Add(codeInvocation);

			return invocationCodeCollection;
		}

		public List<System.CodeDom.CodeNamespaceImport> GenerateImports (EaiConverter.Model.Activity activity)
		{
			return new List<CodeNamespaceImport>
			{
				new CodeNamespaceImport(TargetAppNameSpaceService.EventSourcingNameSpace()),
			};
		}

		public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter (EaiConverter.Model.Activity activity)
		{
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(), GetServiceFieldName(activity))
			};

			return parameters;
		}

		public CodeStatementCollection GenerateConstructorCodeStatement (EaiConverter.Model.Activity activity)
		{
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName(activity));

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName(activity)))
			};

			return statements;
		}

		public List<System.CodeDom.CodeMemberField> GenerateFields (EaiConverter.Model.Activity activity)
		{
			var fields = new List<CodeMemberField>
			{new CodeMemberField
				{
					Type = GetServiceFieldType(),
					Name = GetServiceFieldName(activity),
					Attributes = MemberAttributes.Private
				}
			};

			return fields;
		}

		CodeTypeReference GetServiceFieldType ()
		{
			return new CodeTypeReference (interfaceSubscriberName);
		}

		string GetServiceFieldName (Activity activity)
		{
			return VariableHelper.ToVariableName(activity.Name)+ "RdvPublisher";
		}
	}

}

