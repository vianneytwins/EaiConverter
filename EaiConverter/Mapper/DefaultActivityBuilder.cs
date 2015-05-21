using System;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Mapper
{
    public class DefaultActivityBuilder : IActivityBuilder
	{
        public DefaultActivityBuilder(XslBuilder xpathbuilder){    }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.DefaultInvocationMethod(activity.Name);
            return activityCodeDom;
        }
        #endregion

        public CodeStatementCollection DefaultInvocationMethod (string activityName){
            var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(activityName));
            var methodInvocation = new CodeMethodInvokeExpression (activityServiceReference, "ExecuteQuery", new CodeExpression[] {});
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.Add(methodInvocation);
            return invocationCodeCollection;
        }
	}

}

