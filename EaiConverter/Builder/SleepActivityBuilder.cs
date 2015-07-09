using System.CodeDom;

namespace EaiConverter.Builder
{
	using EaiConverter.Model;

    public class SleepActivityBuilder : IActivityBuilder
	{
        XslBuilder xslBuilder;

        public SleepActivityBuilder (XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
        }

        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.GenerateCodeInvocation(activity);
            return activityCodeDom;
        }

        public CodeStatementCollection GenerateCodeInvocation(Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));
            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(activity.InputBindings));

            // Add the invocation new Timer (timerValue)
            var code = new CodeSnippetStatement ("new Timer("+ activity.Parameters[0].Name+");");

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }
	}

}
