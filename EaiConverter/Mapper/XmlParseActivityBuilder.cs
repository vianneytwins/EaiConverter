using System;
using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Processor;

namespace EaiConverter.Mapper
{
    public class XmlParseActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;
        XmlParserHelperBuilder xmlParserHelperBuilder;

        public XmlParseActivityBuilder(XslBuilder xslBuilder, XmlParserHelperBuilder xmlParserHelperBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.xmlParserHelperBuilder = xmlParserHelperBuilder;
        }

        public ActivityCodeDom Build (Activity activity)
        {
            var mapperActivity = (XmlParseActivity) activity;

            var result = new ActivityCodeDom();

            if (ConfigurationApp.GetProperty("IsXmlParserHelperAlreadyGenerated") != "true")
            {
                result.ClassesToGenerate = this.xmlParserHelperBuilder.Build();
                ConfigurationApp.SaveProperty("IsXmlParserHelperAlreadyGenerated", "true");
            }
            else
            {
                result.ClassesToGenerate = new CodeNamespaceCollection();
            }
            result.InvocationCode = this.GenerateCodeInvocation (mapperActivity);

            return result;
        }


        public CodeStatementCollection GenerateCodeInvocation ( XmlParseActivity xmlParseActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            // Add log at the beginning
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(xmlParseActivity.Name));
            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(xmlParseActivity.InputBindings));

            // Add the invocation itself
           // var variableToAssignReference = new CodeVariableReferenceExpression (VariableHelper.ToVariableName(xmlParseActivity.Name));

            //var variableToAssignReference = new CodeVariableReferenceExpression (VariableHelper.ToVariableName(xmlParseActivity.Name));
            //var codeInvocation = new CodeAssignStatement (variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToVariableName(xmlParseActivity.Name)));

            var variableReturnType = xmlParseActivity.XsdReference.Split(':')[1];
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

