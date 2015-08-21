using System.Collections.Generic;

namespace EaiConverter.Builder
{
    using System.CodeDom;

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

        public JdbcQueryActivityBuilder(DataAccessBuilder dataAccessBuilder, DataAccessServiceBuilder dataAccessServiceBuilder, DataAccessInterfacesCommonBuilder dataAccessCommonBuilder, XslBuilder xslBuilder, ResultSetBuilder resultSetBuilder)
        {
            this.dataAccessBuilder = dataAccessBuilder;
            this.dataAccessServiceBuilder = dataAccessServiceBuilder;
            this.dataAccessCommonBuilder = dataAccessCommonBuilder;
            this.xslBuilder = xslBuilder;
            this.resultSetBuilder = resultSetBuilder;
        }

        public string ServiceToInvoke { private get; set; }

        public CodeNamespaceCollection GenerateClassesToGenerate(Activity activity)
        {
            var jdbcQueryActivity = (JdbcQueryActivity)activity;

            var result = new CodeNamespaceCollection();

            if (this.HasThisSqlRequestAlreadyGenerateAService(jdbcQueryActivity.QueryStatement))
            {
                this.ServiceToInvoke = this.GetExistingJdbcServiceName(jdbcQueryActivity.QueryStatement);
            }
            else
            {
                jdbcQueryActivity.ClassName = this.GenerateClassName(jdbcQueryActivity);

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

                this.ServiceToInvoke = serviceNameSpace.Types[0].Name;
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
                var codeInvocation = new CodeVariableDeclarationStatement(new CodeTypeReference (VariableHelper.ToClassName(jdbcQueryActivity.ClassName) + "ResultSet"), VariableHelper.ToVariableName(jdbcQueryActivity.ClassName) + "ResultSet", new CodeMethodInvokeExpression(activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters));
                invocationCodeCollection.Add(codeInvocation);
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
			return new List<CodeNamespaceImport>
			{
				new CodeNamespaceImport (TargetAppNameSpaceService.domainContractNamespaceName)
			};
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
			{new CodeMemberField
				{
					Type = GetServiceFieldType((JdbcQueryActivity)activity),
					Name = GetServiceFieldName((JdbcQueryActivity)activity),
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

		private static CodeTypeReference GetServiceFieldType (JdbcQueryActivity activity)
		{
			return new CodeTypeReference("I" + VariableHelper.ToClassName(activity.ClassName + "Service"));
		}

        private static string GetServiceFieldName(JdbcQueryActivity activity)
		{
            return VariableHelper.ToVariableName(VariableHelper.ToClassName(activity.ClassName + "Service"));
		}

        private string GenerateClassName(JdbcQueryActivity jdbcQueryActivity)
        {
            if (jdbcQueryActivity.Type == ActivityType.jdbcCallActivityType)
            {
                return jdbcQueryActivity.QueryStatement;
            }
            else
            {
                if (!SqlRequestToActivityMapper.IsThisJdbcActivityNameUsed(jdbcQueryActivity.Name))
                {
                    SqlRequestToActivityMapper.SetThisJdbcActivityNameHasUsed(jdbcQueryActivity.Name);
                    return jdbcQueryActivity.Name;
                }

                var className = jdbcQueryActivity.Name + SqlRequestToActivityMapper.Counter;
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

