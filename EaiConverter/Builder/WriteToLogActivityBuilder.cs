using System;
using EaiConverter.Model;
using EaiConverter.Builder.Utils;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class WriteToLogActivityBuilder : IActivityBuilder
	{
        XslBuilder xslBuilder;

        public WriteToLogActivityBuilder (XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.GenerateCodeInvocation((WriteToLogActivity)activity);
            return activityCodeDom;
        }
        #endregion

        public CodeStatementCollection GenerateCodeInvocation (WriteToLogActivity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();

            // add log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));
            //add the input
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));

            //Add the logger call
            invocationCodeCollection.Add(this.GenerateLoggerCodeInvocation(activity));

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

