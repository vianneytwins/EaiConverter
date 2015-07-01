using System.CodeDom;
using System.Collections.Generic;
using EaiConverter.CodeGenerator.Utils;

namespace EaiConverter.Builder
{
    public class JdbcQueryBuilderUtils
    {

        const string voidString = "System.Void";

        private Dictionary<string, string> sqlMapping = new Dictionary<string, string> {
            {"VARCHAR","System.String"},
            {"INT","System.Int"},
            {"12","System.String"},
            {"4", "System.Int"}
        };
        private Dictionary<string, string> jdbcSharedConfigMapping = new Dictionary<string, string> {
            {"/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc","IntegrationDatabase"},
            {"/Configuration/DAI/PNO/JDBC/JDBCPanorama.sharedjdbc","PanoramaDatabase"},
            {"/Configuration/DAI/PNO/JDBC/JDBCPanoramaMasterFiles.sharedjdbc","MasterFilesDatabase"}
        };

        public CodeParameterDeclarationExpressionCollection ConvertQueryStatementParameter(Dictionary<string, string> queryStatementParameters)
        {
            var methodInputParameters = new CodeParameterDeclarationExpressionCollection();
            if (queryStatementParameters == null)
            {
                return methodInputParameters;
            }
            foreach (var queryParam in queryStatementParameters)
            {
                methodInputParameters.Add(new CodeParameterDeclarationExpression
                {
                    Name = VariableHelper.ToVariableName(queryParam.Key),
                    Type = this.ConvertSQLTypeToObjectType(queryParam.Value)
                });
            }
            return methodInputParameters;
        }

        public CodeTypeReference ConvertSQLTypeToObjectType(string type)
        {
            // TODO pour le moment on laisse comme ca car on veut lister tous les types et a mettre dans le dico et donc que cela plente
            string resultType;
            if (sqlMapping.TryGetValue(type, out resultType))
            {
                return new CodeTypeReference(resultType);
            }
            else
            {
                return new CodeTypeReference(voidString);
            }
        }

        public string ConvertJDBCConnectionName(string type)
        {
            string resultType;
            if (jdbcSharedConfigMapping.TryGetValue(type, out resultType))
            {
                return resultType;
            }
            else
            {
                return "PanoramaDatabase";
            }
        }
    }
}

