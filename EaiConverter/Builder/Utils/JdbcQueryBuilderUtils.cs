namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Utils;

    public static class JdbcQueryBuilderUtils
    {
        private static Dictionary<string, string> sqlMapping = new Dictionary<string, string> {
            { "VARCHAR", CSharpTypeConstant.SystemString },
            { "NVARCHAR", CSharpTypeConstant.SystemString },
            { "TEXT", CSharpTypeConstant.SystemString },
            { "CHAR", CSharpTypeConstant.SystemString },
            { "INT", CSharpTypeConstant.SystemInt32 },
            { "INTEGER", CSharpTypeConstant.SystemInt32 },
            { "SMALLINT", CSharpTypeConstant.SystemInt32 },
            { "NUMBER", CSharpTypeConstant.SystemInt32 },
            { "FLOAT", "System.Double" },
            { "DATE", "System.DateTime" },
            { "DATETIME", "System.DateTime" },
            { "SMALLDATETIME", "System.DateTime" },
            { "TIMESTAMP", "System.DateTime" },
            { "BIT", "System.Boolean" },
            { "VARBINARY", CSharpTypeConstant.SystemInt32 },
            { "REAL",  "System.Double" },
            { "12", CSharpTypeConstant.SystemString },
            { "1", CSharpTypeConstant.SystemString },
            { "4", CSharpTypeConstant.SystemInt32 },
            { "-5", CSharpTypeConstant.SystemInt32 },
            { "2", CSharpTypeConstant.SystemInt32 },
            { "93", "System.DateTime" },
            { "-7", "System.Boolean" },
            { "6", "System.Double" },
            { "-1", CSharpTypeConstant.SystemString },
        };

        private static Dictionary<string, string> jdbcSharedConfigMapping = new Dictionary<string, string> {
            { "/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc", "IntegrationDatabase" },
            { "/Configuration/DAI/PNO/JDBC/JDBCPanorama.sharedjdbc", "PanoramaDatabase" },
            { "/Configuration/DAI/PNO/JDBC/JDBCPanoramaMasterFiles.sharedjdbc", "MasterFilesDatabase" },
            { "/Configuration/DAI/PNO/JDBC/JDBCLNS.sharedjdbc", "LnsDatabase" }
        };

        public static CodeParameterDeclarationExpressionCollection ConvertQueryStatementParameter(Dictionary<string, string> queryStatementParameters)
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
                    Type = ConvertSQLTypeToObjectType(queryParam.Value)
                });
            }
            return methodInputParameters;
        }

        public static CodeTypeReference ConvertSQLTypeToObjectType(string type)
        {
            return new CodeTypeReference(ConvertSQLTypeToTypeInString(type));
        }

        public static string ConvertSQLTypeToTypeInString(string type)
        {
            // TODO pour le moment on laisse comme ca car on veut lister tous les types et a mettre dans le dico et donc que cela plente
            string resultType;
            if (sqlMapping.TryGetValue(type.ToUpper(), out resultType))
            {
                return resultType;
            }
            else
            {
                return CSharpTypeConstant.SystemVoid;
            }
        }

        public static string ConvertJDBCConnectionName(string type)
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

