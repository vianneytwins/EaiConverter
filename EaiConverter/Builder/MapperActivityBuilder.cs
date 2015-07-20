using EaiConverter.Parser;

namespace EaiConverter.Builder
{
    using System.CodeDom;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

    public class MapperActivityBuilder : IActivityBuilder
    { 
        private readonly XslBuilder xslBuilder;
		private readonly XsdBuilder xsdBuilder;
		private readonly XsdParser xsdParser;

		public MapperActivityBuilder(XslBuilder xslBuilder, XsdBuilder xsdBuilder, XsdParser xsdParser)
        {
            this.xslBuilder = xslBuilder;
			this.xsdBuilder = xsdBuilder;
			this.xsdParser = xsdParser;
        }

        public ActivityCodeDom Build(Activity activity)
        {
            var result = new ActivityCodeDom();

            result.ClassesToGenerate = this.GenerateClassesToGenerate(activity);
            result.InvocationCode = this.GenerateInvocationCode(activity);

            return result;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            
            var result = new CodeNamespaceCollection();
            if (activity.ObjectXNodes != null)
            {
				result.Add(this.xsdBuilder.Build(activity.ObjectXNodes, this.TargetNamespace(activity)));
            }

            return result;
        }

        public CodeNamespaceImportCollection GenerateImports(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.List<CodeMemberField> GenerateFields(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var mapperActivity = (MapperActivity) activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(mapperActivity));

            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(mapperActivity.InputBindings));

            // Add the invocation
			// TODO : need to put it in the parser to get the real ReturnType !!
			var variableReturnType = string.Empty;
			if (mapperActivity.XsdReference != null) {
				variableReturnType = mapperActivity.XsdReference.Split (':') [1];
			}
			else
			{
				// TODO : make a utils method in the parser to simplify this
				variableReturnType = (this.xsdParser.Parse (mapperActivity.ObjectXNodes, this.TargetNamespace (activity)))[0].Type;
			}

            var variableName = VariableHelper.ToVariableName(mapperActivity.Name);

            var parameter = new CodeVariableReferenceExpression(mapperActivity.Parameters[0].Name);

            var code = new CodeVariableDeclarationStatement(variableReturnType, variableName, parameter);

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }

		private string TargetNamespace (Activity activity)
		{
			return TargetAppNameSpaceService.domainContractNamespaceName + "." + VariableHelper.ToClassName(activity.Name); 
		}
    }
}

