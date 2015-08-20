namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Parser;

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

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            
            var result = new CodeNamespaceCollection();
            if (activity.ObjectXNodes != null)
            {
                result.Add(this.xsdBuilder.Build(activity.ObjectXNodes, this.TargetNamespace(activity)));
            }

            return result;
        }

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
			return new List<CodeNamespaceImport>();
		}

		public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
		{
			return new CodeParameterDeclarationExpressionCollection();
		}

		public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
		{
			return new CodeStatementCollection();
		}

		public List<CodeMemberField> GenerateFields(Activity activity)
		{
			return new List<CodeMemberField>();
		}

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var mapperActivity = (MapperActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

            // Add the Log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(mapperActivity));

            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(mapperActivity.InputBindings));

            // Add the invocation
            // TODO : need to put it in the parser to get the real ReturnType !!
            string variableReturnType;
            CodeVariableReferenceExpression parameter;

            if (mapperActivity.XsdReference != null)
            {
                variableReturnType = this.GetReturnType(mapperActivity.XsdReference);
                parameter = new CodeVariableReferenceExpression(variableReturnType);
            }
            else
            {
                // TODO : make a utils method in the parser to simplify this
                variableReturnType = this.xsdParser.Parse(mapperActivity.ObjectXNodes, this.TargetNamespace(activity))[0].Type;
                parameter = new CodeVariableReferenceExpression(mapperActivity.Parameters[0].Name);
            }

            var variableName = VariableHelper.ToVariableName(mapperActivity.Name);

            var code = new CodeVariableDeclarationStatement(variableReturnType, variableName, parameter);

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }

        private string GetReturnType(string xsdReference)
        {
            if (xsdReference.Contains(":"))
            {
                return xsdReference.Split(':')[1];
            }

            return xsdReference;
        }

        private string TargetNamespace (Activity activity)
		{
			return TargetAppNameSpaceService.domainContractNamespaceName + "." + VariableHelper.ToClassName(activity.Name); 
		}
    }
}

