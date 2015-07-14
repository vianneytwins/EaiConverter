using System.CodeDom;

using EaiConverter.Model;
using EaiConverter.Builder.Utils;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Processor;
using System.Reflection;
using System.Collections.Generic;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
	public class ResultSetBuilder
	{
        public CodeNamespace Build(JdbcQueryActivity jdbcQueryActivity)
        {
            var resultSetNameSpace = new CodeNamespace(TargetAppNameSpaceService.domainServiceNamespaceName);
            resultSetNameSpace.Imports.AddRange(this.GenerateImport(jdbcQueryActivity));

            var resultSetClass = new CodeTypeDeclaration();
            resultSetClass.IsClass = true;
            resultSetClass.TypeAttributes = TypeAttributes.Public;

            resultSetClass.Name = VariableHelper.ToClassName(jdbcQueryActivity.Name) + "ResultSet";

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

