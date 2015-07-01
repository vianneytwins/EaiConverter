using EaiConverter.Model;

using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
    public class CallProcessActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;

        public CallProcessActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            CallProcessActivity callProcessActivity = (CallProcessActivity)activity;

            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.GenerateCodeInvocation(callProcessActivity);
            return activityCodeDom;
        }
        #endregion

        public CodeStatementCollection GenerateCodeInvocation(CallProcessActivity callProcessActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(callProcessActivity));
            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(callProcessActivity.InputBindings));

            // Add the invocation
            var processToCallReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(callProcessActivity.TibcoProcessToCall.ProcessName));

            var parameters = DefaultActivityBuilder.GenerateParameters(callProcessActivity);

            // TODO : WARNING not sure the start method ProcessName is indeed START
            var methodInvocation = new CodeMethodInvokeExpression(processToCallReference, "Start", parameters);

            var code = new CodeVariableDeclarationStatement("var", VariableHelper.ToVariableName(callProcessActivity.Name), methodInvocation);

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }
    }
}