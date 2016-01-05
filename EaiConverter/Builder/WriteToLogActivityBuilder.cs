using EaiConverter.Model;

using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    public class WriteToLogActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;

        public WriteToLogActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
        }

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>();
		}

		public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
		{
			return new CodeParameterDeclarationExpressionCollection();
		}

		public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
		{
			return new CodeStatementCollection();
		}

		public List<CodeMemberField> GenerateFields(Activity activity)
		{
			return new List<CodeMemberField>();
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

        public string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
    }
}

