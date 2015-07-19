using EaiConverter.Model;

using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class WriteToLogActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;

        public WriteToLogActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = this.GenerateClassesToGenerate(activity);
            activityCodeDom.InvocationCode = this.GenerateInvocationCode(activity);
            return activityCodeDom;
        }
        #endregion

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
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

        public List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var write2LogActivity = (WriteToLogActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // add log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(write2LogActivity));
            //add the input
            invocationCodeCollection.AddRange(this.xslBuilder.Build(write2LogActivity.InputBindings));

            //Add the logger call
            invocationCodeCollection.Add(this.GenerateLoggerCodeInvocation(write2LogActivity));

            return invocationCodeCollection;
        }

        private CodeMethodInvokeExpression GenerateLoggerCodeInvocation(WriteToLogActivity activity)
        {
            var parameters = DefaultActivityBuilder.GenerateParameters(new List<string> {
                @"""Message : {0}\nMessage code : {1} """
            }, activity);


            CodeMethodInvokeExpression stringFormatCall = new CodeMethodInvokeExpression();
            stringFormatCall.Parameters.AddRange(parameters);

            CodeMethodReferenceExpression formatMethod = new CodeMethodReferenceExpression();
            formatMethod.MethodName = "Format";
            CodeVariableReferenceExpression stringObject = new CodeVariableReferenceExpression();
            stringObject.VariableName = "String";
            formatMethod.TargetObject = stringObject;
            stringFormatCall.Method = formatMethod;


            var loggerReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName("logger"));
            var methodInvocation = new CodeMethodInvokeExpression(loggerReference, activity.Role, stringFormatCall);
            return methodInvocation;
        }
    }
}

