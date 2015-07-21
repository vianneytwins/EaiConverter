namespace EaiConverter.Builder
{
    using System.CodeDom;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;

    public class XmlParseActivityBuilder : IActivityBuilder
    { 
        private readonly XslBuilder xslBuilder;
        private readonly XmlParserHelperBuilder xmlParserHelperBuilder;
        private readonly XsdBuilder xsdClassGenerator;

        public XmlParseActivityBuilder(XslBuilder xslBuilder, XmlParserHelperBuilder xmlParserHelperBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.xmlParserHelperBuilder = xmlParserHelperBuilder;
            this.xsdClassGenerator = new XsdBuilder();
        }

        public ActivityCodeDom Build (Activity activity)
        {
            var result = new ActivityCodeDom();

            result.ClassesToGenerate = this.GenerateClassesToGenerate(activity);
            result.InvocationCode = this.GenerateInvocationCode(activity);

            return result;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            var result = new CodeNamespaceCollection();
            if (ConfigurationApp.GetProperty("IsXmlParserHelperAlreadyGenerated") != "true")
            {
                result.AddRange(this.xmlParserHelperBuilder.Build());
                ConfigurationApp.SaveProperty("IsXmlParserHelperAlreadyGenerated", "true");
            }

            if (activity.ObjectXNodes != null)
            {
                result.Add(this.xsdClassGenerator.Build(activity.ObjectXNodes, TargetAppNameSpaceService.domainContractNamespaceName));
            }

            return result;
        }
        public CodeNamespaceImportCollection GenerateImports(Activity activity)
        {
            throw new System.NotImplementedException();
        }
        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            throw new System.NotImplementedException();
        }
        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            throw new System.NotImplementedException();
        }
        public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var xmlParseActivity = (XmlParseActivity) activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add log at the beginning
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(xmlParseActivity));

            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(xmlParseActivity.InputBindings));

            // Add the invocation itself
            // TODO : need to put it in the parser to get the real ReturnType !!
            var variableReturnType = TargetAppNameSpaceService.domainContractNamespaceName + ".TargetObjectModel";
            if (xmlParseActivity.XsdReference != null)
            {
                variableReturnType = xmlParseActivity.XsdReference.Split(':')[1];
            }

            var variableName = VariableHelper.ToVariableName(xmlParseActivity.Name);

            var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(XmlParserHelperBuilder.XmlParserHelperServiceName));

            var parameters = new CodeExpression[]{};
            if (xmlParseActivity.Parameters != null)
            {
                parameters = new CodeExpression[xmlParseActivity.Parameters.Count];
                for (int i = 0; i < xmlParseActivity.Parameters.Count; i++)
                {
                    parameters[i] = new CodeSnippetExpression(xmlParseActivity.Parameters[i].Name);
                }
            }
            var codeInvocation = new CodeMethodInvokeExpression (activityServiceReference, XmlParserHelperBuilder.FromXmlMethodName, parameters);

            var code = new CodeVariableDeclarationStatement (variableReturnType, variableName, codeInvocation);

            invocationCodeCollection.Add(code);
            return invocationCodeCollection;
        }
    }
}

