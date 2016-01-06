namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class ConfirmActivityBuilder : AbstractActivityBuilder
    {
        public override string GetReturnType(Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }

        public override CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethod(activity, variables);
            
            var confirmActivity = (ConfirmActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the callback call to confirm the message
            invocationCodeCollection.Add(new CodeCommentStatement ("TODO: Should be this.subscriber.Confirm(message);"));
            invocationCodeCollection.Add(new CodeSnippetStatement("this.subscriber.Confirm();"));

            activityMethod.Statements.AddRange(invocationCodeCollection);

            return activityMethod;
        }

	}

}

