namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class DataAccessBuilder
    {
        private const string bodyMethodStart = "using (IDataAccess db = this.dataAccessFactory.CreateAccess())\n{\n";
        private const string dbQuery = "db.Query";
        const string iDataAccessFactory = "IDataAccessFactory";

        const string SqlQueryStatement = "sqlQueryStatement";


        public CodeNamespace Build(JdbcQueryActivity jdbcQueryActivity)
        {
            var dataAccessNameSpace = new CodeNamespace(TargetAppNameSpaceService.dataAccessNamespace);
            dataAccessNameSpace.Imports.AddRange(this.GenerateImport(jdbcQueryActivity));

            var dataAccessToGenerate = new CodeTypeDeclaration();
            dataAccessToGenerate.IsClass = true;
            dataAccessToGenerate.TypeAttributes = TypeAttributes.Public;

            dataAccessToGenerate.Name = VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "DataAccess";

            dataAccessToGenerate.Members.AddRange(this.GenererateFields(jdbcQueryActivity));
            dataAccessToGenerate.Members.AddRange(this.GenererateContructors(jdbcQueryActivity, dataAccessToGenerate));
            dataAccessToGenerate.Members.AddRange(this.GenererateMethods(jdbcQueryActivity));

            dataAccessNameSpace.Types.Add(dataAccessToGenerate);

            return dataAccessNameSpace;
        }

        public CodeNamespaceImport[] GenerateImport(JdbcQueryActivity jdbcQueryActivity)
        {
            var imports = new List<CodeNamespaceImport>
            {
                new CodeNamespaceImport("System"),
                new CodeNamespaceImport("System.Linq"),
                new CodeNamespaceImport(TargetAppNameSpaceService.dataAccessCommonNamespace)
            };

            if (jdbcQueryActivity.QueryOutputStatementParameters != null)
            {
                imports.Add(new CodeNamespaceImport(TargetAppNameSpaceService.domainContractNamespaceName));
                imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            }

            return imports.ToArray();
        }

        private CodeMemberField[] GenererateFields(JdbcQueryActivity jdbcQueryActivity)
        {
            var fields = new List<CodeMemberField> {
                new CodeMemberField {
                    Name = SqlQueryStatement,
                    Type = new CodeTypeReference(typeof(System.String)),
                    Attributes = MemberAttributes.Private | MemberAttributes.Const,
                    InitExpression = new CodePrimitiveExpression( jdbcQueryActivity.QueryStatement )
                },
                new CodeMemberField {
                    Name = "dataAccessFactory",
                    Type = new CodeTypeReference (iDataAccessFactory),
                    Attributes = MemberAttributes.Private 
                },
            };
            return fields.ToArray();
        }

        public CodeMemberMethod[] GenererateMethods(JdbcQueryActivity jdbcQueryActivity)
        {
            return new CodeMemberMethod[1] { this.GenerateExecuteQueryMethod(jdbcQueryActivity) };
        }

        public CodeConstructor[] GenererateContructors(JdbcQueryActivity jdbcQueryActivity, CodeTypeDeclaration classModel)
        {
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            foreach (CodeMemberField field in classModel.Members)
            {
                if (field.Type.BaseType == iDataAccessFactory)
                {
                    constructor.Parameters.Add(
                        new CodeParameterDeclarationExpression()
                        {
                            Type = field.Type,
                            Name = field.Name,
                            // TODO verifier que ca marche
                            CustomAttributes = new CodeAttributeDeclarationCollection {
                                new CodeAttributeDeclaration(
                                    JdbcQueryBuilderUtils.ConvertJDBCConnectionName (jdbcQueryActivity.JdbcSharedConfig)
                                )
                            }
                        });

                    var parameterReference = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), field.Name);

                    constructor.Statements.Add(new CodeAssignStatement(parameterReference,
                        new CodeArgumentReferenceExpression(field.Name)));
                }
            }

            return new List<CodeConstructor> { constructor }.ToArray();
        }

        public CodeMemberMethod GenerateExecuteQueryMethod(JdbcQueryActivity jdbcQueryActivity)
        {
            var method = DataAccessServiceBuilder.GenerateExecuteQuerySignature(jdbcQueryActivity);

            method.Statements.Add(this.GenerateExecuteQueryBody(jdbcQueryActivity, method));


            return method;
        }

        public CodeSnippetStatement GenerateExecuteQueryBody(JdbcQueryActivity jdbcQueryActivity, CodeMemberMethod method)
        {
            var sb = new StringBuilder();
            sb.Append(bodyMethodStart);
            var tabulation = new Tab();
            if (method.ReturnType.BaseType == CSharpTypeConstant.SystemVoid)
            {
                sb.AppendLine(string.Format("{0}(", dbQuery));
            }
            else
            {
                sb.AppendLine(string.Format("return {0} <{1}>(", dbQuery, VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "ResultSet"));
            }
            tabulation.Increment();
            sb.Append(string.Format("{0}{1}", tabulation, SqlQueryStatement));

            if (method.Parameters != null && method.Parameters.Count >= 1)
            {
                sb.AppendLine(",");
                sb.AppendLine(string.Format("{0}new", tabulation));
                sb.AppendLine(string.Format("{0}{{", tabulation.Increment()));
                tabulation.Increment();
                foreach (CodeParameterDeclarationExpression inputParameter in method.Parameters)
                {
                    sb.AppendLine(string.Format("{0}{1} = {1},", tabulation, inputParameter.Name));
                }
                // remove last comma
                sb.Remove(sb.ToString().LastIndexOf(','), 1);
                tabulation.Decrement();
                sb.AppendLine(string.Format("{0}}}", tabulation));
            }
            //ferme le dbQuery
            tabulation.Decrement();
            if (method.ReturnType.BaseType != CSharpTypeConstant.SystemVoid)
            {
                if (ActivityType.jdbcCallActivityType != jdbcQueryActivity.Type)
                {
                    sb.AppendLine(string.Format("{0}).ToList();", tabulation));
                }
                else
                {
                    sb.AppendLine(string.Format("{0}).FirstOrDefault();", tabulation));
                }
            }
            else
            {
                sb.AppendLine(string.Format("{0});", tabulation));
            }

            // ferme le using
            sb.AppendLine("}");
            return new CodeSnippetStatement(sb.ToString());
        }
    }
}

