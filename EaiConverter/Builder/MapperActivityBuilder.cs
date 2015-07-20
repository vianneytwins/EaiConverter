namespace EaiConverter.Builder
{
    using System.CodeDom;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;

    public class MapperActivityBuilder : IActivityBuilder
    { 
        private readonly XslBuilder xslBuilder;
        private readonly XsdBuilder xsdClassGenerator;

        public MapperActivityBuilder(XslBuilder xslBuilder)
        {
            this.xslBuilder = xslBuilder;
            this.xsdClassGenerator = new XsdBuilder();
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
                result.Add(this.xsdClassGenerator.Build(activity.ObjectXNodes, TargetAppNameSpaceService.domainContractNamespaceName));
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
            // TODO : need to put it in the parser
            var variableReturnType = TargetAppNameSpaceService.domainContractNamespaceName + ".TargetObjectModel";
            if (mapperActivity.XsdReference != null)
            {
                variableReturnType = mapperActivity.XsdReference.Split(':')[1];
            }

            var variableName = VariableHelper.ToVariableName(mapperActivity.Name);

            var parameter = new CodeVariableReferenceExpression(mapperActivity.Parameters[0].Name);

            var code = new CodeVariableDeclarationStatement(variableReturnType, variableName, parameter);

            invocationCodeCollection.Add(code);

            return invocationCodeCollection;
        }
    }
}

