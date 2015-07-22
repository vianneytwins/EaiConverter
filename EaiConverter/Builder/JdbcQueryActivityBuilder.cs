namespace EaiConverter.Builder
{
    using System.CodeDom;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;

    public class JdbcQueryActivityBuilder : IActivityBuilder
    {
        DataAccessBuilder dataAccessBuilder;
        DataAccessServiceBuilder dataAccessServiceBuilder;
        DataAccessInterfacesCommonBuilder dataAccessCommonBuilder;
        ResultSetBuilder resultSetBuilder;
        XslBuilder xslBuilder;

        public string serviceToInvoke { get; set;}

        public JdbcQueryActivityBuilder(DataAccessBuilder dataAccessBuilder, DataAccessServiceBuilder dataAccessServiceBuilder, DataAccessInterfacesCommonBuilder dataAccessCommonBuilder, XslBuilder xslBuilder, ResultSetBuilder resultSetBuilder)
        {
            this.dataAccessBuilder = dataAccessBuilder;
            this.dataAccessServiceBuilder = dataAccessServiceBuilder;
            this.dataAccessCommonBuilder = dataAccessCommonBuilder;
            this.xslBuilder = xslBuilder;
            this.resultSetBuilder = resultSetBuilder;
        }



        string GetDataCustomAttributeName(CodeNamespace dataAccessNameSpace)
        {
            return ((CodeMemberMethod)dataAccessNameSpace.Types[0].Members[2]).Parameters[0].CustomAttributes[0].Name;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity)activity;

            var result = new CodeNamespaceCollection();

            if (this.HasThisSqlRequestAlreadyGenerateAService(jdbcQueryActivity.QueryStatement))
            {
                this.serviceToInvoke = this.GetExistingJdbcServiceName(jdbcQueryActivity.QueryStatement);
            }
            else
            {
                if (jdbcQueryActivity.QueryOutputStatementParameters != null && jdbcQueryActivity.QueryOutputStatementParameters.Count != 0)
                {
                    result.Add(this.resultSetBuilder.Build(jdbcQueryActivity)); 
                }
                var dataAccessNameSpace = this.dataAccessBuilder.Build(jdbcQueryActivity);
                var dataAccessInterfaceNameSpace = InterfaceExtractorFromClass.Extract(dataAccessNameSpace.Types[0], TargetAppNameSpaceService.dataAccessNamespace);
                dataAccessNameSpace.Types[0].BaseTypes.Add(new CodeTypeReference(dataAccessInterfaceNameSpace.Types[0].Name));

                var serviceNameSpace = this.dataAccessServiceBuilder.Build(jdbcQueryActivity);
                var serviceInterfaceNameSpace = InterfaceExtractorFromClass.Extract(serviceNameSpace.Types[0], TargetAppNameSpaceService.domainContractNamespaceName);
                serviceNameSpace.Types[0].BaseTypes.Add(new CodeTypeReference(serviceInterfaceNameSpace.Types[0].Name));

                var dataCommonNamespace = this.dataAccessCommonBuilder.Build();

                //TODO : Find a more suitable way to retrieve the CustomAttribute To Build
                var dataBaseAttributeNamespace = new DatabaseAttributeBuilder().Build(GetDataCustomAttributeName(dataAccessNameSpace));


                result.Add(dataAccessNameSpace);
                result.Add(dataAccessInterfaceNameSpace);
                result.Add(serviceNameSpace);
                result.Add(serviceInterfaceNameSpace);
                result.Add(dataCommonNamespace);
                result.Add(dataBaseAttributeNamespace);

                this.serviceToInvoke = serviceNameSpace.Types[0].Name;
            }

            return result;
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity)activity;

            var invocationCodeCollection = new CodeStatementCollection();
            // Add the log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(jdbcQueryActivity));
            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(jdbcQueryActivity.InputBindings));

            // Add the invocation itself
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(serviceToInvoke));

            var parameters = DefaultActivityBuilder.GenerateParameters(jdbcQueryActivity);


			if (jdbcQueryActivity.QueryOutputStatementParameters != null && jdbcQueryActivity.QueryOutputStatementParameters.Count > 0)
            {
                var codeInvocation = new CodeVariableDeclarationStatement(new CodeTypeReference (VariableHelper.ToClassName(jdbcQueryActivity.Name)+"ResultSet"), VariableHelper.ToVariableName(jdbcQueryActivity.Name)+"ResultSet", new CodeMethodInvokeExpression(activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters));
                invocationCodeCollection.Add(codeInvocation);
            }
            else
            {
                var codeInvocation = new CodeMethodInvokeExpression(activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters);
                invocationCodeCollection.Add(codeInvocation);
            }

            return invocationCodeCollection;
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

        /// <summary>
        /// Determines whether this sql request has already generate A service for the specified queryStatement.
        /// </summary>
        /// <returns><c>true</c> if this sql request already generate A service;
        /// otherwise, <c>false</c>.</returns>
        /// <param name="queryStatement">Query statement.</param>
        private bool HasThisSqlRequestAlreadyGenerateAService(string queryStatement)
        {
            return SqlRequestToActivityMapper.ContainsKey(queryStatement);
        }

        private string GetExistingJdbcServiceName(string queryStatement)
        {
            return SqlRequestToActivityMapper.GetJdbcServiceName(queryStatement);
        }
    }
}

