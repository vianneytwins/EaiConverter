namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class DataAccessServiceBuilder
    {
        const string DataAccessVariableName = "dataAccess";

        public const string ExecuteSqlQueryMethodName = "ExecuteQuery";


        public CodeNamespace Build(JdbcQueryActivity jdbcQueryActivity)
        {

            var serviceNameSpace = new CodeNamespace(TargetAppNameSpaceService.domainServiceNamespaceName());
            serviceNameSpace.Imports.AddRange(this.GenerateServiceImport(jdbcQueryActivity));


            var serviceToGenerate = new CodeTypeDeclaration();
            serviceToGenerate.IsClass = true;
            serviceToGenerate.TypeAttributes = TypeAttributes.Public;

            serviceToGenerate.Name = VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "Service";

            //dataAccessToGenerate.Imports = this.GenerateImport (jdbcQueryActivity);
            serviceToGenerate.Members.AddRange(this.GenererateFields(jdbcQueryActivity));
            serviceToGenerate.Members.AddRange(this.GenererateContructors(jdbcQueryActivity, serviceToGenerate));
            serviceToGenerate.Members.AddRange(this.GenererateMethods(jdbcQueryActivity));

            serviceNameSpace.Types.Add(serviceToGenerate);

            return serviceNameSpace;
        }


        public CodeNamespaceImport[] GenerateServiceImport(JdbcQueryActivity jdbcQueryActivity)
        {
            var imports = new List<CodeNamespaceImport>
            {
                new CodeNamespaceImport("System"),
                new CodeNamespaceImport(TargetAppNameSpaceService.domainContractNamespaceName()),
                new CodeNamespaceImport(TargetAppNameSpaceService.dataAccessNamespace())
            };

            if (jdbcQueryActivity.QueryOutputStatementParameters != null)
            {
                imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            }

            return imports.ToArray();
        }


        private CodeMemberField[] GenererateFields(JdbcQueryActivity jdbcQueryActivity)
        {
            var fields = new List<CodeMemberField> {
                new CodeMemberField {
                    Name = DataAccessVariableName,
                    Type = new CodeTypeReference(VariableHelper.ToClassName("I" + jdbcQueryActivity.ClassName) + "DataAccess"),
                    Attributes = MemberAttributes.Private 
                },
            };
            return fields.ToArray();
        }

        private CodeMemberMethod[] GenererateMethods(JdbcQueryActivity jdbcQueryActivity)
        {
            return new CodeMemberMethod[1] { this.GenerateExecuteQueryMethod(jdbcQueryActivity) };
        }

        private CodeConstructor[] GenererateContructors(JdbcQueryActivity jdbcQueryActivity, CodeTypeDeclaration classModel)
        {
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            foreach (CodeMemberField field in classModel.Members)
            {

                constructor.Parameters.Add(
                    new CodeParameterDeclarationExpression() { Type = field.Type, Name = field.Name });

                var parameterReference = new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), field.Name);

                constructor.Statements.Add(new CodeAssignStatement(parameterReference,
                    new CodeArgumentReferenceExpression(field.Name)));
            }

            return new List<CodeConstructor> { constructor }.ToArray();
        }

        public CodeMemberMethod GenerateExecuteQueryMethod(JdbcQueryActivity jdbcQueryActivity)
        {
            {
                var method = GenerateExecuteQuerySignature(jdbcQueryActivity);

                method.Statements.Add(this.GenerateExecuteQueryBody(method));

                return method;
            }
        }

        public CodeStatement GenerateExecuteQueryBody(CodeMemberMethod method)
        {
            var dataAccessReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), DataAccessVariableName);
            var methodArgumentReferences = new CodeArgumentReferenceExpression[method.Parameters.Count];

            for (int i = 0; i < method.Parameters.Count; i++)
            {
                methodArgumentReferences[i] = new CodeArgumentReferenceExpression(method.Parameters[i].Name);
            }

            var invocationExpression = new CodeMethodInvokeExpression(dataAccessReference, ExecuteSqlQueryMethodName, methodArgumentReferences);

            if (method.ReturnType.BaseType != CSharpTypeConstant.SystemVoid)
            {
                return new CodeMethodReturnStatement(invocationExpression);
            }
            else
            {
                return new CodeExpressionStatement(invocationExpression);
            }
        }

        public static CodeMemberMethod GenerateExecuteQuerySignature(JdbcQueryActivity jdbcQueryActivity)
        {
            var method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.Name = ExecuteSqlQueryMethodName;
            if (jdbcQueryActivity.QueryOutputStatementParameters != null && jdbcQueryActivity.QueryOutputStatementParameters.Count > 0)
            {
                if (ActivityType.jdbcCallActivityType != jdbcQueryActivity.Type)
                {
                    method.ReturnType = new CodeTypeReference("List<" + VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "ResultSet>");
                }
                else
                {
                    method.ReturnType = new CodeTypeReference(VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "ResultSet");
                }
            }
            else
            {
                method.ReturnType = new CodeTypeReference(CSharpTypeConstant.SystemVoid);
            }
            method.Parameters.AddRange(JdbcQueryBuilderUtils.ConvertQueryStatementParameter(jdbcQueryActivity.QueryStatementParameters));
            return method;
        }
    }
}

