using EaiConverter.Model;

using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class DefaultActivityBuilder : IActivityBuilder
    {
        public DefaultActivityBuilder(XslBuilder xslbuilder) { }

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
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(activity.Name));
            var methodInvocation = new CodeMethodInvokeExpression(activityServiceReference, "Execute", new CodeExpression[] { });
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(LogActivity(activity));
            invocationCodeCollection.Add(methodInvocation);
            return invocationCodeCollection;
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

        public static CodeExpression[] GenerateParameters(List<string> existingParamaters, Activity activity)
        {
            var parameterLists = new List<CodeExpression> { };
            //Add existing Parameter
            if (existingParamaters != null)
            {
                foreach (var parameter in existingParamaters)
                {
                    parameterLists.Add(new CodeSnippetExpression(parameter));
                }
            }

            //Add Activity Paramters
            if (activity.Parameters != null)
            {
                foreach (var parameter in activity.Parameters)
                {
                    parameterLists.Add(new CodeSnippetExpression(parameter.Name));
                }
            }

            return parameterLists.ToArray();
        }

        public static CodeExpression[] GenerateParameters(Activity activity)
        {
            return GenerateParameters(null, activity);
        }
    }
}

