using EaiConverter.Parser;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
	using EaiConverter.Model;

    public class GetSharedVariableActivityBuilder : AbstractSharedVariableActivityBuilder
	{
        private readonly XslBuilder xslBuilder;

        public GetSharedVariableActivityBuilder(XslBuilder xslBuilder) : base()
        {
            this.xslBuilder = xslBuilder;
        }

        public override CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var sharedVariableActivity = (SharedVariableActivity) activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add log at the beginning
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(sharedVariableActivity));

            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(sharedVariableActivity.InputBindings));
            invocationCodeCollection.Add(new CodeSnippetStatement("var configName = \"" + sharedVariableActivity.VariableConfig + "\";"));

            // Add the invocation itself

            var variableName = VariableHelper.ToVariableName(sharedVariableActivity.Name);

            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(SharedVariableServiceBuilder.SharedVariableServiceName));

            var parameters = new CodeExpression[1]{new CodeSnippetExpression("configName")};

            var codeInvocation = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(activityServiceReference, SharedVariableServiceBuilder.GetMethodName), parameters);

            var code = new CodeVariableDeclarationStatement(new CodeTypeReference("var"), variableName, codeInvocation);

            invocationCodeCollection.Add(code);
            return invocationCodeCollection;
        }
	}

}
