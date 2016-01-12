namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class WriteToLogActivityBuilder : AbstractActivityBuilder
    {
        private XslBuilder xslBuilder;

        public WriteToLogActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public override List<CodeMemberMethod> GenerateMethods(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethods(activity, variables);
            var write2LogActivity = (WriteToLogActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            //add the input
            invocationCodeCollection.AddRange(this.xslBuilder.Build(write2LogActivity.InputBindings));

            //Add the logger call
            invocationCodeCollection.Add(this.GenerateLoggerCodeInvocation(write2LogActivity));

            activityMethod[0].Statements.AddRange(invocationCodeCollection);

            return activityMethod;
        }

        private CodeMethodInvokeExpression GenerateLoggerCodeInvocation(WriteToLogActivity activity)
        {
            var parameters = GenerateParameters(
                new List<string> { @"""Message : {0}\nMessage code : {1} """ },
                activity);

            var stringFormatCall = new CodeMethodInvokeExpression();
            stringFormatCall.Parameters.AddRange(parameters);

            var formatMethod = new CodeMethodReferenceExpression { MethodName = "Format" };
            var stringObject = new CodeVariableReferenceExpression { VariableName = "String" };
            formatMethod.TargetObject = stringObject;
            stringFormatCall.Method = formatMethod;

            var loggerReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName("logger"));
            var methodInvocation = new CodeMethodInvokeExpression(loggerReference, activity.Role, stringFormatCall);
            return methodInvocation;
        }

        public override string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
    }
}

