using System;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Mapper
{
    public class WriteToLogActivityBuilder : IActivityBuilder
	{
        XslBuilder xslBuilder;

        public WriteToLogActivityBuilder (XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.InvocationMethod((WriteToLogActivity)activity);
            return activityCodeDom;
        }
        #endregion

        public CodeStatementCollection InvocationMethod (WriteToLogActivity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();

            // add log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity.Name));
            //add the input
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));


            var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName("logger"));
            var methodInvocation = new CodeMethodInvokeExpression (activityServiceReference, activity.Role, new CodeExpression[] {new CodePrimitiveExpression("Todo: ")});

            return invocationCodeCollection;
        }
   
	}

}

