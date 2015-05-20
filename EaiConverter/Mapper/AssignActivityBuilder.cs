using System;
using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Mapper
{
    public class AssignActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        public AssignActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public ActivityCodeDom Build (Activity activity)
        {
            AssignActivity assignActivity = (AssignActivity) activity;

            var result = new ActivityCodeDom();

            result.ClassesToGenerate = new CodeNamespaceCollection();
            result.InvocationCode = this.GenerateCodeInvocation (assignActivity);

            return result;
        }


        public CodeStatementCollection GenerateCodeInvocation ( AssignActivity assignActivity){

            var invocationCodeCollection = new CodeStatementCollection();

            invocationCodeCollection.AddRange(this.xslBuilder.Build(assignActivity.InputBindings));

            var variableToAssignReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(assignActivity.VariableName));
            var codeInvocation = new CodeAssignStatement (variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToVariableName(assignActivity.VariableName)));
            invocationCodeCollection.Add(codeInvocation);
            return invocationCodeCollection;
        }

    }
}

