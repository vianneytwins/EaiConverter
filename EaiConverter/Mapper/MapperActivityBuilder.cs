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
            // TODO : set the result of the map in an object of type xsd ref with the variable name of the activty
            var variableToAssignReference = new CodeVariableReferenceExpression ( VariableHelper.ToVariableName(mapperActivity.Name));
            var codeInvocation = new CodeAssignStatement (variableToAssignReference, new CodeVariableReferenceExpression(VariableHelper.ToVariableName(mapperActivity.Name)));
            invocationCodeCollection.Add(codeInvocation);
            return invocationCodeCollection;
        }

    }
}

