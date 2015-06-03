using System;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Mapper
{
    public class WriteToLogActivityBuilder : IActivityBuilder
	{
        public WriteToLogActivityBuilder (XslBuilder xslbuilder){    }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.DefaultInvocationMethod(activity.Name);
            return activityCodeDom;
        }
        #endregion

        public CodeStatementCollection DefaultInvocationMethod (string activityName)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activityName));

            return invocationCodeCollection;
        }
   
	}

}

