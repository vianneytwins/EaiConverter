using System;
using EaiConverter.Model;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Mapper
{
    public class XmlParseActivityBuilder : IActivityBuilder
    { 
        XslBuilder xslBuilder;

        public XmlParseActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public ActivityCodeDom Build (Activity activity)
        {
            var mapperActivity = (XmlParseActivity) activity;

            var result = new ActivityCodeDom();

            result.ClassesToGenerate = new CodeNamespaceCollection();
            result.InvocationCode = this.GenerateCodeInvocation (mapperActivity);

            return result;
        }


        public CodeStatementCollection GenerateCodeInvocation ( XmlParseActivity mapperActivity)
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

