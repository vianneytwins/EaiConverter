using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using EaiConverter.Model;
using EaiConverter.Builder.Utils;
using EaiConverter.Builder;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Processor;

namespace EaiConverter.Builder
{
    public class JdbcQueryActivityBuilder : IActivityBuilder
	{
		DataAccessBuilder dataAccessBuilder;
		DataAccessServiceBuilder dataAccessServiceBuilder;
		DataAccessInterfacesCommonBuilder dataAccessCommonBuilder;
        XslBuilder xslBuilder;

        public JdbcQueryActivityBuilder (DataAccessBuilder dataAccessBuilder, DataAccessServiceBuilder dataAccessServiceBuilder, DataAccessInterfacesCommonBuilder dataAccessCommonBuilder, XslBuilder xslBuilder){
			this.dataAccessBuilder = dataAccessBuilder;
			this.dataAccessServiceBuilder = dataAccessServiceBuilder;
			this.dataAccessCommonBuilder = dataAccessCommonBuilder;
            this.xslBuilder = xslBuilder;
		}

        public ActivityCodeDom Build (Activity activity)
		{
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) activity;

            var result = new ActivityCodeDom();
            string jdbcServiceName;
            if (this.HasThisSqlRequestAlreadyGenerateAService(jdbcQueryActivity.QueryStatement))
            {
                result.ClassesToGenerate = new CodeNamespaceCollection();
                jdbcServiceName = this.GetExistingJdbcServiceName(jdbcQueryActivity.QueryStatement);

            }
            else
            {
                var dataAccessNameSpace = this.dataAccessBuilder.Build (jdbcQueryActivity);
    			var dataAccessInterfaceNameSpace = InterfaceExtractorFromClass.Extract (dataAccessNameSpace.Types[0], TargetAppNameSpaceService.dataAccessNamespace );
    			dataAccessNameSpace.Types[0].BaseTypes.Add(new CodeTypeReference(dataAccessInterfaceNameSpace.Types[0].Name));

                var serviceNameSpace = this.dataAccessServiceBuilder.Build (jdbcQueryActivity);
    			var serviceInterfaceNameSpace = InterfaceExtractorFromClass.Extract (serviceNameSpace.Types[0], TargetAppNameSpaceService.domainContractNamespaceName );
    			serviceNameSpace.Types[0].BaseTypes.Add(new CodeTypeReference(serviceInterfaceNameSpace.Types[0].Name));

    			var dataCommonNamespace = this.dataAccessCommonBuilder.Build ();

    			//TODO : Find a more suitable way to retrieve the CustomAttribute To Build
                var dataBaseAttributeNamespace = new DatabaseAttributeBuilder ().Build (GetDataCustomAttributeName (dataAccessNameSpace));


                result.ClassesToGenerate = new CodeNamespaceCollection {
    				dataAccessNameSpace,
    				dataAccessInterfaceNameSpace,
    				serviceNameSpace,
    				serviceInterfaceNameSpace,
    				dataCommonNamespace,
    				dataBaseAttributeNamespace}
    				;

                jdbcServiceName = serviceNameSpace.Types[0].Name;
            }

            result.InvocationCode = this.GenerateCodeInvocation (jdbcServiceName, jdbcQueryActivity);

			return result;
		}
			

		string GetDataCustomAttributeName (CodeNamespace dataAccessNameSpace)
		{
			return ((CodeMemberMethod) dataAccessNameSpace.Types [0].Members [2]).Parameters[0].CustomAttributes[0].Name;
		}


        public CodeStatementCollection GenerateCodeInvocation (string serviceToInvoke, JdbcQueryActivity jdbcQueryActivity){

            var invocationCodeCollection = new CodeStatementCollection();
            // Add the log
            invocationCodeCollection.AddRange(DefaultActivityBuilder.LogActivity(jdbcQueryActivity));
            // Add the input bindings
            invocationCodeCollection.AddRange(this.xslBuilder.Build(jdbcQueryActivity.InputBindings));

            // Add the invocation itself
            var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(serviceToInvoke));

            var parameters = DefaultActivityBuilder.GenerateParameters(jdbcQueryActivity);
   

            if (jdbcQueryActivity.QueryOutputCachedSchemaColumns != null)
            {
                var codeInvocation = new CodeVariableDeclarationStatement("var", VariableHelper.ToVariableName(jdbcQueryActivity.Name), new CodeMethodInvokeExpression(activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters));
                invocationCodeCollection.Add(codeInvocation);
            }
            else
            {
                var codeInvocation = new CodeMethodInvokeExpression (activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, parameters);
                invocationCodeCollection.Add(codeInvocation);
            }

            return invocationCodeCollection;
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

