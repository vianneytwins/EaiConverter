using System;
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
            MapperActivity mapperActivity = (MapperActivity) activity;

            var result = new ActivityCodeDom();

            result.ClassesToGenerate = new CodeNamespaceCollection();
            result.InvocationCode = this.GenerateCodeInvocation (mapperActivity);

            return result;
        }


        public CodeStatementCollection GenerateCodeInvocation ( MapperActivity mapperActivity)
        {
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

