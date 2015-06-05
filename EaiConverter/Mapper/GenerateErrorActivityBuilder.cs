using System;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;
using System.CodeDom;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Mapper
{
    public class GenerateErrorActivityBuilder : IActivityBuilder
	{
        XslBuilder xslBuilder;

        public GenerateErrorActivityBuilder (XslBuilder xslbuilder){
            this.xslBuilder = xslBuilder;
        }

        #region IActivityBuilder implementation
        public ActivityCodeDom Build(Activity activity)
        {
            var activityCodeDom = new ActivityCodeDom();
            activityCodeDom.ClassesToGenerate = new CodeNamespaceCollection();

            var errorActivity = (GenerateErrorActivity)activity;
            activityCodeDom.InvocationCode = this.InvocationMethod(activity);
            return activityCodeDom;
        }
        #endregion

        public CodeStatementCollection InvocationMethod (GenerateErrorActivity errorActivity)
        {
            var invocationCodeCollection = new CodeStatementCollection();

            // add log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(errorActivity.Name));
            //add the input
            invocationCodeCollection.AddRange(this.xslBuilder.Build(errorActivity.InputBindings));

            return invocationCodeCollection;
        }
   
	}

}

