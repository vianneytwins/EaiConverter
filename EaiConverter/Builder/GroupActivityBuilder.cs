using System;
using EaiConverter.Model;
using EaiConverter.Builder.Utils;
using System.CodeDom;
using System.Collections.Generic;

namespace EaiConverter.Builder
{
    public class GroupActivityBuilder : IActivityBuilder
	{
        XslBuilder xslBuilder;
        CoreProcessBuilder coreProcessBuilder;

        public GroupActivityBuilder (XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.coreProcessBuilder = new CoreProcessBuilder();
        }

        ActivityCodeDom IActivityBuilder.Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();
            activityCodeDom.InvocationCode = this.InvocationMethod(activity);
            return activityCodeDom;
        }

        public CodeStatementCollection InvocationMethod (Activity activity)
        {
            var invocationCodeCollection = new CodeStatementCollection();
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(activity));

            invocationCodeCollection.AddRange(this.GenerateCoreGroupMethod((GroupActivity) activity));


            return invocationCodeCollection;
        }

        public CodeStatementCollection GenerateCoreGroupMethod(GroupActivity groupActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();

            // TODO populate the dictionnary
            var dictionary = new Dictionary<string, CodeStatementCollection>();
            invocationCodeCollection.AddRange(this.coreProcessBuilder.GenerateStartCodeStatement(groupActivity.Transitions, "start", null, dictionary));
            return invocationCodeCollection;
        }
	}

}

