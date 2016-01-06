namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

    public class GetSharedVariableActivityBuilder : AbstractSharedVariableActivityBuilder
	{
        private readonly XslBuilder xslBuilder;

        public GetSharedVariableActivityBuilder(XslBuilder xslBuilder) : base()
        {
            this.xslBuilder = xslBuilder;
        }

        public override CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethod(activity, variables);

            var sharedVariableActivity = (SharedVariableActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(sharedVariableActivity.InputBindings));
            invocationCodeCollection.Add(new CodeSnippetStatement("var configName = \"" + sharedVariableActivity.VariableConfig + "\";"));

            // Add the invocation itself

            var variableName = VariableHelper.ToVariableName(sharedVariableActivity.Name);

            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(SharedVariableServiceBuilder.SharedVariableServiceName));

            var parameters = new CodeExpression[1]{new CodeSnippetExpression("configName")};

            var codeInvocation = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(activityServiceReference, SharedVariableServiceBuilder.GetMethodName), parameters);

            var code = new CodeMethodReturnStatement(codeInvocation);

            invocationCodeCollection.Add(code);
            activityMethod.Statements.AddRange(invocationCodeCollection);
            return activityMethod;
        }
	}

}
