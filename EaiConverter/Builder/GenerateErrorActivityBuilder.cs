using EaiConverter.Model;

using System.CodeDom;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class GenerateErrorActivityBuilder : IActivityBuilder
    {
        XslBuilder xslBuilder;

        public GenerateErrorActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = this.GenerateClassesToGenerate(activity);


            activityCodeDom.InvocationCode = this.GenerateInvocationCode(activity);
            return activityCodeDom;
        }
        #endregion

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            return new CodeNamespaceCollection();
        }


        public CodeNamespaceImportCollection GenerateImports(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
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
            CodeThrowExceptionStatement throwException = new CodeThrowExceptionStatement(new CodeObjectCreateExpression(new CodeTypeReference(typeof(System.Exception)), stringFormatCall));
            return throwException;
        }
    }
}

