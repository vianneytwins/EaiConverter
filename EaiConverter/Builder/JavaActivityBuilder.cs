using System;
using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
    public class JavaActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        public JavaActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public ActivityCodeDom Build(Activity activity)
        {
            JavaActivity javaActivity = (JavaActivity) activity;

            var result = new ActivityCodeDom();

            result.ClassesToGenerate = new CodeNamespaceCollection();
            result.InvocationCode = this.GenerateCodeInvocation (javaActivity);

            return result;
        }


        public CodeStatementCollection GenerateCodeInvocation(JavaActivity javaActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(javaActivity.Name));

            invocationCodeCollection.AddRange(this.xslBuilder.Build(javaActivity.InputBindings));

            var variableToAssignReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(javaActivity.FileName));
            var codeInvocation = new CodeAssignStatement (variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToVariableName(javaActivity.FileName)));
            invocationCodeCollection.Add(codeInvocation);
            return invocationCodeCollection;
        }

    }
}

