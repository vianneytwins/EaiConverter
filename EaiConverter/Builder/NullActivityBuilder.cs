using EaiConverter.Model;

using System.CodeDom;

namespace EaiConverter.Builder
{
    public class NullActivityBuilder : IActivityBuilder
    {
        public NullActivityBuilder(XslBuilder xslbuilder) { }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.DefaultInvocationMethod(activity);
            return activityCodeDom;
        }
        #endregion

        public CodeStatementCollection DefaultInvocationMethod(Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));

            return invocationCodeCollection;
        }
    }
}

