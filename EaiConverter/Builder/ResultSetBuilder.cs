namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class ResultSetBuilder
	{
        public CodeNamespace Build(JdbcQueryActivity jdbcQueryActivity)
        {
            var resultSetNameSpace = new CodeNamespace(TargetAppNameSpaceService.domainContractNamespaceName);
            resultSetNameSpace.Imports.AddRange(this.GenerateImport(jdbcQueryActivity));

            var resultSetClass = new CodeTypeDeclaration();
            resultSetClass.IsClass = true;
            resultSetClass.TypeAttributes = TypeAttributes.Public;

            resultSetClass.Name = VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "ResultSet";

            resultSetClass.Members.AddRange(this.GenererateProperties(jdbcQueryActivity));

            resultSetNameSpace.Types.Add(resultSetClass);

            return resultSetNameSpace;
        }

        public CodeNamespaceImport[] GenerateImport(JdbcQueryActivity jdbcQueryActivity)
        {
            return new CodeNamespaceImport[1] {
                new CodeNamespaceImport ("System")
            };
        }

        private CodeMemberProperty[] GenererateProperties(JdbcQueryActivity jdbcQueryActivity)
        {
            var fields = new List<CodeMemberProperty>();
            foreach(var element in jdbcQueryActivity.QueryOutputStatementParameters)
            {
                fields.Add(CodeDomUtils.GenerateProperty(element.Name, JdbcQueryBuilderUtils.ConvertSQLTypeToTypeInString(element.Type)));
            }
            return fields.ToArray();
        }
	}

}

