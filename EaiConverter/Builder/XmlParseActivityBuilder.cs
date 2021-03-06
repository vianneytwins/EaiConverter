﻿namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Parser;
    using EaiConverter.Processor;

    using log4net;

    public class XmlParseActivityBuilder : AbstractActivityBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(XmlParseActivityBuilder));

        private readonly XslBuilder xslBuilder;
        private readonly XmlParserHelperBuilder xmlParserHelperBuilder;
		private readonly XsdBuilder xsdBuilder;
		private readonly XsdParser xsdParser;

		public XmlParseActivityBuilder(XslBuilder xslBuilder, XmlParserHelperBuilder xmlParserHelperBuilder, XsdBuilder xsdBuilder, XsdParser xsdParser)
        {
            this.xslBuilder = xslBuilder;
            this.xmlParserHelperBuilder = xmlParserHelperBuilder;
			this.xsdBuilder = xsdBuilder;
			this.xsdParser = xsdParser;
        }

        public override CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables)
        {
            var result = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsXmlParserHelperAlreadyGenerated") != "true")
            {
                result.AddRange(this.xmlParserHelperBuilder.Build());
                ConfigurationApp.SaveProperty("IsXmlParserHelperAlreadyGenerated", "true");
            }

            try
            {
                if (activity.ObjectXNodes != null)
                {
                    result.Add(this.xsdBuilder.Build(activity.ObjectXNodes, this.TargetNamespace(activity)));
                }
            }
            catch (Exception e)
            {
                Log.Error("Unable to generate class from XSD file inside XMLParseActivity :" + activity.Name, e);
            }

            return result;
        }

        public override List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>{new CodeNamespaceImport(TargetAppNameSpaceService.xmlToolsNameSpace())};
		}

        public override CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(), GetServiceFieldName())
			};

			return parameters;
        }

        public override CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName());

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName()))
			};

			return statements;
        }

        public override List<CodeMemberField> GenerateFields(Activity activity)
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

        public override List<CodeMemberMethod> GenerateMethods(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethods(activity, variables);
            var xmlParseActivity = (XmlParseActivity) activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(xmlParseActivity.InputBindings));

            // Add the invocation itself
            // TODO : need to put it in the parser to get the real ReturnType !!
			var variableReturnType = this.GetReturnType(activity);

            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(XmlParserHelperBuilder.XmlParserHelperServiceName));

            var parameters = new CodeExpression[] { };
            if (xmlParseActivity.Parameters != null)
            {
                parameters = new CodeExpression[xmlParseActivity.Parameters.Count];
                for (int i = 0; i < xmlParseActivity.Parameters.Count; i++)
                {
                    parameters[i] = new CodeSnippetExpression(xmlParseActivity.Parameters[i].Name);
                }
            }

            var codeInvocation = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(activityServiceReference, XmlParserHelperBuilder.FromXmlMethodName, new CodeTypeReference(variableReturnType)), parameters);

            var code = new CodeMethodReturnStatement(codeInvocation);

            invocationCodeCollection.Add(code);

            activityMethod[0].Statements.AddRange(invocationCodeCollection);

            return activityMethod;
        }

		private static CodeTypeReference GetServiceFieldType()
		{
			return new CodeTypeReference(XmlParserHelperBuilder.IXmlParserHelperServiceName);
		}

		private static string GetServiceFieldName()
		{
			return VariableHelper.ToVariableName(VariableHelper.ToClassName (XmlParserHelperBuilder.XmlParserHelperServiceName));
		}

		private string TargetNamespace(Activity activity)
		{
			return TargetAppNameSpaceService.domainContractNamespaceName() + "." + VariableHelper.ToClassName(activity.Name); 
		}

        public override string GetReturnType(Activity activity)
        {
            XmlParseActivity xmlParseActivity = (XmlParseActivity)activity;
            var variableReturnType = "System.String";
            if (xmlParseActivity.XsdReference != null)
            {
                variableReturnType = xmlParseActivity.XsdReference.Split(':')[1];
            }
            else
            {
                // TODO : make a utils method in the parser to simplify this
                if (this.xsdParser.Parse(xmlParseActivity.ObjectXNodes, this.TargetNamespace(activity)).Count > 0)
                {
                    variableReturnType = (this.xsdParser.Parse(xmlParseActivity.ObjectXNodes, this.TargetNamespace(activity)))[0].Type;
                }
            }
            return variableReturnType;
        }

    }
}

