namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

    public class SetSharedVariableActivityBuilder : AbstractSharedVariableActivityBuilder
	{
        private readonly XslBuilder xslBuilder;

        public SetSharedVariableActivityBuilder(XslBuilder xslBuilder) : base()
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
            // TODO : need to put it in the parser to get the real ReturnType !!

            //var variableName = VariableHelper.ToVariableName(sharedVariableActivity.Name);

            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(SharedVariableServiceBuilder.SharedVariableServiceName));

            var parameters = this.GenerateParameters(new List<string> { "configName" }, sharedVariableActivity);

            var codeInvocation = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(activityServiceReference, SharedVariableServiceBuilder.SetMethodName), parameters);

            invocationCodeCollection.Add(codeInvocation);

            activityMethod.Statements.AddRange(invocationCodeCollection);
            return activityMethod;
        }
	}

}

