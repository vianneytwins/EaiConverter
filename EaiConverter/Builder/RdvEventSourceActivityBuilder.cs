using System;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.Builder.Utils;
using EaiConverter.Processor;
using System.Reflection;
using EaiConverter.Utils;
using EaiConverter.Model;

namespace EaiConverter.Builder
{
	public class RdvEventSourceActivityBuilder : IActivityBuilder
	{
		public const string interfaceSubscriberName = "ISubscriber";

		private const string subscriber = "subscriber";

		public CodeNamespaceCollection GenerateClassesToGenerate (EaiConverter.Model.Activity activity)
		{
			var namespaces = new CodeNamespaceCollection ();
			if (ConfigurationApp.GetProperty("IsSubscriberInterfaceAlreadyGenerated") != "true")
			{
				namespaces.Add(this.GenerateSubscriberInterface());
				namespaces.Add(this.GenerateResponseReceivedEventHandler());
				ConfigurationApp.SaveProperty("IsSubscriberInterfaceAlreadyGenerated", "true");
			}

			if (ConfigurationApp.GetProperty("IsTibcoSubscriberImplemAlreadyGenerated") != "true")
			{
				namespaces.Add (this.GenerateTibcoSubscriberImplementation ());
				ConfigurationApp.SaveProperty("IsTibcoSubscriberImplemAlreadyGenerated", "true");
			}

			return namespaces;
		}

		public CodeNamespace GenerateSubscriberInterface ()
		{
			var subscriberInterfaceNamespace = new CodeNamespace ();
			subscriberInterfaceNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace;
			subscriberInterfaceNamespace.Imports.Add (new CodeNamespaceImport("System"));

			var subscriberInterfaceClass = new CodeTypeDeclaration(interfaceSubscriberName);
			subscriberInterfaceClass.IsInterface = true;

			CodeMemberEvent event1 = new CodeMemberEvent();
			// Sets a name for the event.
			event1.Name = "ResponseReceived";
			// Sets the type of event.
			event1.Type = new CodeTypeReference("ResponseReceivedEventHandler");

			subscriberInterfaceClass.Members.Add (event1);
			subscriberInterfaceClass.Members.Add (CodeDomUtils.GeneratePropertyWithoutSetter ("WaitingTimeLimit",CSharpTypeConstant.SystemInt32));
			subscriberInterfaceClass.Members.Add (CodeDomUtils.GeneratePropertyWithoutSetter ("IsStarted",CSharpTypeConstant.SystemBoolean));

			subscriberInterfaceClass.Members.Add(new CodeMemberMethod {
				Name = "Start",
				ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid)
			});
			subscriberInterfaceClass.Members.Add(new CodeMemberMethod {
				Name = "Stop",
				ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid)
			});

			subscriberInterfaceNamespace.Types.Add (subscriberInterfaceClass);
			return subscriberInterfaceNamespace;
		}

		CodeNamespace GenerateResponseReceivedEventHandler ()
		{
			return new CodeNamespace ();
		}

		CodeNamespace GenerateTibcoSubscriberImplementation ()
		{
			var subscriberRdvClassNamespace = new CodeNamespace ();
			subscriberRdvClassNamespace.Name = TargetAppNameSpaceService.EventSourcingNameSpace;
			subscriberRdvClassNamespace.Imports.Add (new CodeNamespaceImport("System"));

			var subscriberRdvClass = new CodeTypeDeclaration("TibcoPublisher");
			subscriberRdvClass.IsClass = true;
			subscriberRdvClass.Attributes = MemberAttributes.Public;
			subscriberRdvClass.BaseTypes.Add(subscriber);

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
			return new CodeTypeReference(interfaceSubscriberName);
		}

		string GetServiceFieldName ()
		{
			return subscriber;
		}
	}

}

