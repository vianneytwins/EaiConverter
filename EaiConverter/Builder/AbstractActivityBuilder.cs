namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public abstract class AbstractActivityBuilder : IActivityBuilder
    {

        protected string GetMethodName(Activity activity)
        {
            return VariableHelper.ToClassName(activity.Name) + "Call";
        }

        public virtual CodeNamespaceCollection GenerateClassesToGenerate(Activity activity, Dictionary<string, string> variables)
        {
            return new CodeNamespaceCollection();
        }

        public virtual List<CodeNamespaceImport> GenerateImports(Activity activity)
        {
            return new List<CodeNamespaceImport>();
        }

        public virtual CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            return new CodeParameterDeclarationExpressionCollection();
        }

        public virtual CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            return new CodeStatementCollection();
        }

        public virtual List<CodeMemberField> GenerateFields(Activity activity)
        {
            return new List<CodeMemberField>();
        }

        public abstract string GetReturnType(Activity activity);

        public virtual CodeStatementCollection GenerateInvocationCode(Activity activity, Dictionary<string, string> variables)
        {
            var existingParamaters = new XpathUtils().GetVariableNames(activity.InputBindings);
            for (int i = 0; i < existingParamaters.Count; i++)
            {
                existingParamaters[i] = VariableHelper.ToVariableName(existingParamaters[i]);
            }
            var parameters = GenerateParameters(existingParamaters, null);

            var invocationCodeCollection = new CodeStatementCollection();

            var returnType = this.GetReturnType(activity);
            if (!returnType.Equals(CSharpTypeConstant.SystemVoid))
            {
                var codeInvocation = new CodeVariableDeclarationStatement(
                    new CodeTypeReference(returnType),
                    this.GetReturnVariableName(activity),
                    new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.GetMethodName(activity), parameters));
                invocationCodeCollection.Add(codeInvocation);
            }
            else
            {
                var codeInvocation = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), this.GetMethodName(activity), parameters);
                invocationCodeCollection.Add(codeInvocation);
            }

            return invocationCodeCollection;
        }

        protected virtual string GetReturnVariableName(Activity activity)
        {
            return VariableHelper.ToVariableName(activity.Name);
        }

        public virtual List<CodeMemberMethod> GenerateMethods(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = new CodeMemberMethod
                                     {
                                         Attributes = MemberAttributes.Private | MemberAttributes.Final,
                                         Name = VariableHelper.ToClassName(activity.Name) + "Call"
                                     };
            var dependantVariables = new XpathUtils().GetVariableNames(activity.InputBindings);

            foreach(var variable in dependantVariables)
            {
                if (variables.ContainsKey(variable))
                {
                    activityMethod.Parameters.Add(new CodeParameterDeclarationExpression(variables[variable], VariableHelper.ToVariableName(variable)));
                }
            }

            activityMethod.ReturnType = new CodeTypeReference(this.GetReturnType(activity));

            var invocationLogCodeCollection = new CodeStatementCollection();
            invocationLogCodeCollection.AddRange(LogActivity(activity));
            activityMethod.Statements.AddRange(invocationLogCodeCollection);

            return new List<CodeMemberMethod>{activityMethod};
        }

        public static CodeStatementCollection LogActivity(Activity activity)
        {
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName("logger"));
            var methodInvocation = new CodeMethodInvokeExpression(
                activityServiceReference,
                "Info",
                new CodeExpression[]
                    {
                        new CodePrimitiveExpression("Start Activity: " + activity.Name + " of type: " + activity.Type)
                });

            var logCallStatements = new CodeStatementCollection();
            logCallStatements.Add(methodInvocation);
            return logCallStatements;
        }

        public static CodeExpression[] GenerateParameters(Activity activity)
        {
            return GenerateParameters(null, activity);
        }



        public static  CodeExpression[] GenerateParameters(List<string> existingParamaters, Activity activity)
        {
            var parameterLists = new List<CodeExpression> { };
            //Add existing Parameter
            if (existingParamaters != null)
            {
                foreach (var parameter in existingParamaters)
                {
                    parameterLists.Add(new CodeSnippetExpression(VariableHelper.ToSafeType(parameter)));
                }
            }

            //Add Activity Parameters
            if (activity != null && activity.Parameters != null)
            {
                foreach (var parameter in activity.Parameters)
                {
                    parameterLists.Add(new CodeSnippetExpression(parameter.Name));
                }
            }

            return parameterLists.ToArray();
        }
    }
}

