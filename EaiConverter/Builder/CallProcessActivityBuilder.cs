using EaiConverter.Parser;
using EaiConverter.Processor;

namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class CallProcessActivityBuilder : AbstractActivityBuilder
    {
        private readonly XslBuilder xslBuilder;
        private readonly TibcoBWProcessLinqParser parser;

        public CallProcessActivityBuilder(XslBuilder xslBuilder, TibcoBWProcessLinqParser parser)
        {
            this.xslBuilder = xslBuilder;
            this.parser = parser;
        }

		public override List<CodeNamespaceImport> GenerateImports(Activity activity)
        {
			var import4Activities = new List<CodeNamespaceImport>();
			var callProcessActivity = (CallProcessActivity)activity;
            import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.myAppName() + "." + TargetAppNameSpaceService.RemoveFirstDot(callProcessActivity.TibcoProcessToCall.ShortNameSpace)));
            if (IsTheProcessInputRequiresAnImport(callProcessActivity))
            {
                import4Activities.Add(new CodeNamespaceImport(TargetAppNameSpaceService.myAppName() + "." + TargetAppNameSpaceService.RemoveFirstDot(callProcessActivity.TibcoProcessToCall.InputAndOutputNameSpace)));
            }

			return import4Activities;
        }

        public override CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
			var callProcessActivity = (CallProcessActivity)activity;
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType(callProcessActivity), GetServiceFieldName(callProcessActivity))
			};

			return parameters;
        }

        public override CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
			var callProcessActivity = (CallProcessActivity)activity;
			var parameterReference = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), GetServiceFieldName(callProcessActivity));

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName(callProcessActivity)))
			};

			return statements;
        }

        public override List<CodeMemberField> GenerateFields(Activity activity)
        {
			var callProcessActivity = (CallProcessActivity)activity;
			return new List<CodeMemberField>
			{
				new CodeMemberField
				{
					Type = GetServiceFieldType(callProcessActivity),
					Name = GetServiceFieldName(callProcessActivity),
					Attributes = MemberAttributes.Private
				}
			};
        }

        public override List<CodeMemberMethod> GenerateMethods(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethods(activity, variables);

            var callProcessActivity = (CallProcessActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the mapping
            if (IsTheProcessInputRequiresAnImport(callProcessActivity))
            {
                invocationCodeCollection.AddRange(
                    this.xslBuilder.Build(
                        TargetAppNameSpaceService.myAppName() + "." + callProcessActivity.TibcoProcessToCall.InputAndOutputNameSpace,
                        callProcessActivity.InputBindings));
            }
            else
            {
                invocationCodeCollection.AddRange(
                    this.xslBuilder.Build(callProcessActivity.InputBindings));
            }

            // Add the invocation
            var processToCallReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(callProcessActivity.TibcoProcessToCall.ProcessName));

            var parameters = GenerateParameters(callProcessActivity);

            var methodInvocation = new CodeMethodInvokeExpression(processToCallReference, GetCalledProcess(activity).StartActivity.Name, parameters);

            var code = new CodeMethodReturnStatement(methodInvocation);

            invocationCodeCollection.Add(code);

            activityMethod[0].Statements.AddRange(invocationCodeCollection);

            return activityMethod;
        }


        public override string GetReturnType(Activity activity)
        {
            var tibcoProcessToCall = GetCalledProcess(activity);
            if (tibcoProcessToCall.EndActivity.Parameters != null && tibcoProcessToCall.EndActivity.Parameters.Count > 0)
            {
                return tibcoProcessToCall.EndActivity.Parameters[0].Type;
            }
            return CSharpTypeConstant.SystemVoid;
        }

        private TibcoBWProcess GetCalledProcess(Activity activity)
        {
            var processName = ((CallProcessActivity)activity).ProcessName;
            var projectDirectory = ConfigurationApp.GetProperty(MainClass.ProjectDirectory);
            var tibcoProcessToCall = this.parser.Parse(projectDirectory + processName);
            return tibcoProcessToCall;
        }

        private static bool IsTheProcessInputRequiresAnImport(CallProcessActivity callProcessActivity)
        {
            return callProcessActivity.InputBindings != null && callProcessActivity.InputBindings.Count() != 0 && String.IsNullOrEmpty(((XElement)callProcessActivity.InputBindings.First()).Name.Namespace.ToString());
        }

        private static CodeTypeReference GetServiceFieldType(CallProcessActivity callProcessActivity)
        {
            return new CodeTypeReference(TargetAppNameSpaceService.myAppName() + "." + callProcessActivity.TibcoProcessToCall.ShortNameSpace + ".I" + VariableHelper.ToClassName(callProcessActivity.TibcoProcessToCall.ProcessName));
        }

        private static string GetServiceFieldName(CallProcessActivity callProcessActivity)
        {
            return VariableHelper.ToVariableName(callProcessActivity.TibcoProcessToCall.ProcessName);
        }
    }
}