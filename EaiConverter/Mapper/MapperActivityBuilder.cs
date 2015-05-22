using System;
using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Mapper
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
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(mapperActivity.Name));

            invocationCodeCollection.AddRange(this.xslBuilder.Build(mapperActivity.InputBindings));

            var variableToAssignReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(mapperActivity.Name));
            var codeInvocation = new CodeAssignStatement (variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToVariableName(mapperActivity.Name)));
            invocationCodeCollection.Add(codeInvocation);
            return invocationCodeCollection;
        }

    }
}

