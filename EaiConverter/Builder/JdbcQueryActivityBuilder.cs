namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;

    using EaiConverter.Builder.Utils;
    using EaiConverter.CodeGenerator.Utils;
    using EaiConverter.Model;
    using EaiConverter.Processor;

    public class JdbcQueryActivityBuilder : IActivityBuilder
    {
        private readonly DataAccessBuilder dataAccessBuilder;
        private readonly DataAccessServiceBuilder dataAccessServiceBuilder;
        private readonly DataAccessInterfacesCommonBuilder dataAccessCommonBuilder;
        private readonly ResultSetBuilder resultSetBuilder;
        private readonly XslBuilder xslBuilder;
        private string serviceToInvoke;


        public JdbcQueryActivityBuilder(DataAccessBuilder dataAccessBuilder, DataAccessServiceBuilder dataAccessServiceBuilder, DataAccessInterfacesCommonBuilder dataAccessCommonBuilder, XslBuilder xslBuilder, ResultSetBuilder resultSetBuilder)
        {
            this.dataAccessBuilder = dataAccessBuilder;
            this.dataAccessServiceBuilder = dataAccessServiceBuilder;
            this.dataAccessCommonBuilder = dataAccessCommonBuilder;
            this.xslBuilder = xslBuilder;
            this.resultSetBuilder = resultSetBuilder;
        }

        public string ServiceToInvoke {
            get
            {
                return this.serviceToInvoke;
            }
            set
            {
                this.serviceToInvoke = value;
            }
        }

        private string GetServiceToInvoke(JdbcQueryActivity jdbcQueryActivity)
        {
            if (!string.IsNullOrEmpty(this.ServiceToInvoke))
            {
                return this.serviceToInvoke;
            }

            if (this.HasThisSqlRequestAlreadyGenerateAService(jdbcQueryActivity.QueryStatement))
            {
                this.serviceToInvoke = this.GetExistingJdbcServiceName(jdbcQueryActivity.QueryStatement);
            }
            else
            {
                jdbcQueryActivity.ClassName = this.GenerateClassName(jdbcQueryActivity);
                this.serviceToInvoke = jdbcQueryActivity.ClassName + "Service";
            }

            return serviceToInvoke;
        }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            var jdbcQueryActivity = (JdbcQueryActivity)activity;

            var result = new CodeNamespaceCollection();
            if (this.HasThisSqlRequestAlreadyGenerateAService(jdbcQueryActivity.QueryStatement))
            {
                this.ServiceToInvoke = this.GetExistingJdbcServiceName(jdbcQueryActivity.QueryStatement);
                jdbcQueryActivity.ClassName = this.ServiceToInvoke.Replace("Service", string.Empty);
            }
            else
            {
				jdbcQueryActivity.ClassName = this.GenerateClassName(jdbcQueryActivity);
                this.ServiceToInvoke = jdbcQueryActivity.ClassName + "Service";

				SqlRequestToActivityMapper.SaveSqlRequest(jdbcQueryActivity.QueryStatement, this.ServiceToInvoke);

                if (jdbcQueryActivity.QueryOutputStatementParameters != null && jdbcQueryActivity.QueryOutputStatementParameters.Count != 0)
                {
                    result.Add(this.resultSetBuilder.Build(jdbcQueryActivity)); 
                }

                var dataAccessNameSpace = this.dataAccessBuilder.Build(jdbcQueryActivity);
                var dataAccessInterfaceNameSpace = InterfaceExtractorFromClass.Extract(dataAccessNameSpace.Types[0], TargetAppNameSpaceService.dataAccessNamespace);
                if (jdbcQueryActivity.QueryOutputStatementParameters != null
                    && jdbcQueryActivity.QueryOutputStatementParameters.Count != 0)
                {
                    dataAccessInterfaceNameSpace.Imports.Add(new CodeNamespaceImport(TargetAppNameSpaceService.domainContractNamespaceName));
                }

                dataAccessNameSpace.Types[0].BaseTypes.Add(new CodeTypeReference(dataAccessInterfaceNameSpace.Types[0].Name));

                var serviceNameSpace = this.dataAccessServiceBuilder.Build(jdbcQueryActivity);
                var serviceInterfaceNameSpace = InterfaceExtractorFromClass.Extract(serviceNameSpace.Types[0], TargetAppNameSpaceService.domainContractNamespaceName);
                
                serviceNameSpace.Types[0].BaseTypes.Add(new CodeTypeReference(serviceInterfaceNameSpace.Types[0].Name));

                result.Add(dataAccessNameSpace);
                result.Add(dataAccessInterfaceNameSpace);
                result.Add(serviceNameSpace);
                result.Add(serviceInterfaceNameSpace);

                if (ConfigurationApp.GetProperty("HasCommonDataAccessAlreadyGenerated") != "true")
                {
                    var dataCommonNamespace = this.dataAccessCommonBuilder.Build();
                    result.Add(dataCommonNamespace);
                    ConfigurationApp.SaveProperty("HasCommonDataAccessAlreadyGenerated", "true");
                }

                //TODO : Find a more suitable way to retrieve the CustomAttribute To Build
                string dataCustomAttributeName = this.GetDataCustomAttributeName(dataAccessNameSpace);
                if (ConfigurationApp.GetProperty(dataCustomAttributeName) != "true")
                {
                    var dataBaseAttributeNamespace = new DatabaseAttributeBuilder().Build(dataCustomAttributeName);
                    result.Add(dataBaseAttributeNamespace);
                    ConfigurationApp.SaveProperty(dataCustomAttributeName, "true");
                }
            }

            return result;
        }

        public CodeStatementCollection GenerateInvocationCode(Activity activity)
        {
            var jdbcQueryActivity = (JdbcQueryActivity)activity;

            var invocationCodeCollection = new CodeStatementCollection();

            // Add the log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(jdbcQueryActivity));

            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(jdbcQueryActivity.InputBindings));

            // Add the invocation itself
            var activityServiceReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), VariableHelper.ToVariableName(this.ServiceToInvoke));

            var parameters = DefaultActivityBuilder.GenerateParameters(jdbcQueryActivity);


			if (jdbcQueryActivity.QueryOutputStatementParameters != null && jdbcQueryActivity.QueryOutputStatementParameters.Count > 0)
            {   
                if (ActivityType.jdbcCallActivityType != jdbcQueryActivity.Type)
                {
                    var codeInvocation = new CodeVariableDeclarationStatement(new CodeTypeReference("List<" + VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "ResultSet>"), VariableHelper.ToVariableName(jdbcQueryActivity.Name) + "ResultSet", new CodeMethodInvokeExpression(activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters));
                    invocationCodeCollection.Add(codeInvocation);
                }
                else
                {
                    var codeInvocation = new CodeVariableDeclarationStatement(new CodeTypeReference(VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "ResultSet"), VariableHelper.ToVariableName(jdbcQueryActivity.Name) + "ResultSet", new CodeMethodInvokeExpression(activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters));
                    invocationCodeCollection.Add(codeInvocation);
                }
            }
            else
            {
                var codeInvocation = new CodeMethodInvokeExpression(activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters);
                invocationCodeCollection.Add(codeInvocation);
            }

            return invocationCodeCollection;
        }

		public List<CodeNamespaceImport> GenerateImports(Activity activity)
		{
		    var jdbcQueryActivity = (JdbcQueryActivity)activity;

            var imports = new List<CodeNamespaceImport>
			{
				new CodeNamespaceImport(TargetAppNameSpaceService.domainContractNamespaceName)
			};

            if (jdbcQueryActivity.QueryOutputStatementParameters != null)
            {
                imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            }

		    return imports;
		}

        public CodeParameterDeclarationExpressionCollection GenerateConstructorParameter(Activity activity)
        {
			var parameters = new CodeParameterDeclarationExpressionCollection
			{
				new CodeParameterDeclarationExpression(GetServiceFieldType((JdbcQueryActivity) activity), GetServiceFieldName((JdbcQueryActivity)activity))
			};

			return parameters;
        }

		public CodeStatementCollection GenerateConstructorCodeStatement(Activity activity)
		{
			var parameterReference = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), GetServiceFieldName((JdbcQueryActivity)activity));

			var statements = new CodeStatementCollection
			{
				new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression(GetServiceFieldName((JdbcQueryActivity)activity)))
			};

			return statements;
		}

		public List<CodeMemberField> GenerateFields(Activity activity)
		{
			var fields = new List<CodeMemberField>
			{
                new CodeMemberField
				{
					Type = this.GetServiceFieldType((JdbcQueryActivity)activity),
					Name = this.GetServiceFieldName((JdbcQueryActivity)activity),
					Attributes = MemberAttributes.Private
				}
			};

			return fields;
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

		private CodeTypeReference GetServiceFieldType(JdbcQueryActivity activity)
		{
            return new CodeTypeReference("I" + this.GetServiceToInvoke(activity));
		}

        private string GetServiceFieldName(JdbcQueryActivity activity)
		{
            return VariableHelper.ToVariableName(this.GetServiceToInvoke(activity));
		}

        private string GenerateClassName(JdbcQueryActivity jdbcQueryActivity)
        {
            if (!string.IsNullOrEmpty(jdbcQueryActivity.ClassName))
            {
                return jdbcQueryActivity.ClassName;
            }

            if (jdbcQueryActivity.Type == ActivityType.jdbcCallActivityType)
            {
                return jdbcQueryActivity.QueryStatement;
            }
            else
            {
                if (!SqlRequestToActivityMapper.IsThisJdbcActivityNameUsed(jdbcQueryActivity.Name))
                {
                    SqlRequestToActivityMapper.SetThisJdbcActivityNameHasUsed(jdbcQueryActivity.Name);
                    return VariableHelper.ToClassName(jdbcQueryActivity.Name);
                }

                var className = VariableHelper.ToClassName(jdbcQueryActivity.Name) + SqlRequestToActivityMapper.Counter;
                SqlRequestToActivityMapper.Counter++;

                return className;
            }
        }

        private string GetDataCustomAttributeName(CodeNamespace dataAccessNameSpace)
        {
            return ((CodeMemberMethod)dataAccessNameSpace.Types[0].Members[2]).Parameters[0].CustomAttributes[0].Name;
        }
    }
}

