namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Parser;

    public class MapperActivityBuilder : AbstractActivityBuilder
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

        public override CodeMemberMethod GenerateMethod(Activity activity, Dictionary<string, string> variables)
        {
            var activityMethod = base.GenerateMethod(activity, variables);
            var mapperActivity = (MapperActivity)activity;
            var invocationCodeCollection = new CodeStatementCollection();

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

            var packageName = this.RemoveFinalType(variableReturnType);
            // Add the mapping
            invocationCodeCollection.AddRange(this.xslBuilder.Build(packageName, mapperActivity.InputBindings));
            activityMethod.Statements.AddRange(invocationCodeCollection);
            
            var code = new CodeMethodReturnStatement(parameter);
            activityMethod.Statements.Add(code);
            return activityMethod;
        }

        private string RemoveFinalType(string variableReturnType)
        {
            var lastIndexOf = variableReturnType.LastIndexOf('.');
            if (variableReturnType == null || lastIndexOf < 0)
            {
                return string.Empty;
            }
            return variableReturnType.Remove(lastIndexOf, variableReturnType.Length - lastIndexOf);
            
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
            return TargetAppNameSpaceService.domainContractNamespaceName() + "." + VariableHelper.ToClassName(activity.Name); 
		}

        public override string GetReturnType (Activity activity)
        {
            var mapperActivity = (MapperActivity)activity;


            string variableReturnType;

            if (mapperActivity.XsdReference != null)
            {
                variableReturnType = this.GetReturnType(mapperActivity.XsdReference);
            }
            else
            {
                // TODO : make a utils method in the parser to simplify this
                variableReturnType = this.xsdParser.Parse(mapperActivity.ObjectXNodes, this.TargetNamespace(activity))[0].Type;
            }

            return variableReturnType;
        }
    }
}

