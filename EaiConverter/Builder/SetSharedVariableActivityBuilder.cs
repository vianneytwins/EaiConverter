using System;
using EaiConverter.Builder;
using EaiConverter.Model;
using System.Xml.Linq;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.Processor;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Builder.Utils;

namespace EaiConverter.Builder
{
    public class SetSharedVariableActivityBuilder : AbstractSharedVariableActivityBuilder
	{
        private readonly XslBuilder xslBuilder;
        private SharedVariableServiceBuilder sharedVariableServiceBuilder;

        public SetSharedVariableActivityBuilder(XslBuilder xslBuilder) : base()
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
            invocationCodeCollection.Add(new CodeSnippetStatement("var configName = \""+sharedVariableActivity.VariableConfig+"\";"));

            // Add the invocation itself
            // TODO : need to put it in the parser to get the real ReturnType !!

            //var variableName = VariableHelper.ToVariableName(sharedVariableActivity.Name);

            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(SharedVariableServiceBuilder.SharedVariableServiceName));

            var parameters = DefaultActivityBuilder.GenerateParameters(new List<string> {"configName"}, sharedVariableActivity);

            var codeInvocation = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(activityServiceReference, SharedVariableServiceBuilder.SetMethodName), parameters);

            invocationCodeCollection.Add(codeInvocation);
            return invocationCodeCollection;
        }
	}

}

