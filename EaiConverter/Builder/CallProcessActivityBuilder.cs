using EaiConverter.Model;

using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;
using EaiConverter.Builder.Utils;

namespace EaiConverter.Builder
{
    public class CallProcessActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;

        public CallProcessActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }


        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
        }


		public List<CodeNamespaceImport> GenerateImports(Activity activity)
        {
			var import4Activities = new List<CodeNamespaceImport>();
			var callProcessActivity = (CallProcessActivity)activity;
			import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.ConvertXsdImportToNameSpace(callProcessActivity.TibcoProcessToCall.ShortNameSpace)));
			import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.ConvertXsdImportToNameSpace(callProcessActivity.TibcoProcessToCall.InputAndOutputNameSpace)));

			return import4Activities;
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
            CallProcessActivity callProcessActivity = (CallProcessActivity)activity;
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