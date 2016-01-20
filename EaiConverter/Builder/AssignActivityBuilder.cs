namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class AssignActivityBuilder : AbstractActivityBuilder
    {
        private readonly XslBuilder xslBuilder;

        public AssignActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public override List<CodeMemberMethod> GenerateMethods(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethods(activity, variables);

            var assignActivity = (AssignActivity)activity;

            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(this.xslBuilder.Build(assignActivity.InputBindings));

            var variableToAssignReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToSafeType(assignActivity.VariableName));
            var codeInvocation = new CodeAssignStatement(variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToSafeType(assignActivity.VariableName)));
            invocationCodeCollection.Add(codeInvocation);

            activityMethod[0].Statements.AddRange(invocationCodeCollection);

            return activityMethod;
        }

        public override string GetReturnType(Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }


    }
}

