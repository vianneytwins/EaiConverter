using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Model;

    public class GenerateErrorActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;

        public GenerateErrorActivityBuilder(XslBuilder xslBuilder)
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
            var errorActivity = (GenerateErrorActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // add log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(errorActivity));
            //add the input
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));

            // Add the exception Call
            invocationCodeCollection.Add(this.GenerateExceptionStatement(errorActivity));

            return invocationCodeCollection;
        }

        private CodeThrowExceptionStatement GenerateExceptionStatement(GenerateErrorActivity activity)
        {
            var parameters =
                DefaultActivityBuilder.GenerateParameters(
                    new List<string> { @"""Message : {0}\nMessage code : {1} """ },
                    activity);

            var stringFormatCall = new CodeMethodInvokeExpression();
            stringFormatCall.Parameters.AddRange(parameters);

            var formatMethod = new CodeMethodReferenceExpression { MethodName = "Format" };
            var stringObject = new CodeVariableReferenceExpression { VariableName = "String" };
            formatMethod.TargetObject = stringObject;
            stringFormatCall.Method = formatMethod;

            var throwException =
                new CodeThrowExceptionStatement(
                    new CodeObjectCreateExpression(new CodeTypeReference(typeof(System.Exception)), stringFormatCall));
            return throwException;
        }


        public string GetReturnType (Activity activity)
        {
            return CSharpTypeConstant.SystemException;
        }
    }
}

