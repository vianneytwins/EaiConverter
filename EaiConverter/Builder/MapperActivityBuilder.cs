using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
    public class MapperActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        public MapperActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public ActivityCodeDom Build (Activity activity)
        {
            

            var result = new ActivityCodeDom();

            result.ClassesToGenerate = this.GenerateClassesToGenerate(activity);
            result.InvocationCode = this.GenerateInvocationCode (activity);

            return result;
        }

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
        public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateInvocationCode (Activity activity)
        {
            MapperActivity mapperActivity = (MapperActivity) activity;
            var invocationCodeCollection = new CodeStatementCollection();
            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(mapperActivity));
            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(mapperActivity.InputBindings));

            // Add the invocation
            var variableReturnType = mapperActivity.XsdReference.Split(':')[1];
            var variableName = VariableHelper.ToVariableName(mapperActivity.Name);

            var parameter = new CodeVariableReferenceExpression(mapperActivity.Parameters[0].Name);

            var code = new CodeVariableDeclarationStatement (variableReturnType, variableName, parameter);

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }
    }
}

