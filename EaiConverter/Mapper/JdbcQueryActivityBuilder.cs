using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;
using EaiConverter.Mapper;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Processor;

namespace EaiConverter.Mapper
{
    public class JdbcQueryActivityBuilder : IActivityBuilder
	{
		DataAccessBuilder dataAccessBuilder;
		DataAccessServiceBuilder dataAccessServiceBuilder;

		DataAccessInterfacesCommonBuilder dataAccessCommonBuilder;

		public JdbcQueryActivityBuilder (DataAccessBuilder dataAccessBuilder, DataAccessServiceBuilder dataAccessServiceBuilder, DataAccessInterfacesCommonBuilder dataAccessCommonBuilder){
			this.dataAccessBuilder = dataAccessBuilder;
			this.dataAccessServiceBuilder = dataAccessServiceBuilder;
			this.dataAccessCommonBuilder = dataAccessCommonBuilder;
		}

        public ActivityCodeDom Build (Activity activity)
		{
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) activity;

            var result = new ActivityCodeDom();

            if (this.HasThisSqlRequestAlreadyGenerateAService(jdbcQueryActivity.QueryStatement))
            {
                result.ClassesToGenerate = new CodeNamespaceCollection();
                var existingJdbcServiceName = this.GetExistingJdbcServiceName(jdbcQueryActivity.QueryStatement);
                result.InvocationCode = this.GenerateCodeInvocation (existingJdbcServiceName);
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

                result.InvocationCode = this.GenerateCodeInvocation (serviceNameSpace.Types[0].Name);
            }

			return result;
		}
			

		string GetDataCustomAttributeName (CodeNamespace dataAccessNameSpace)
		{
			return ((CodeMemberMethod) dataAccessNameSpace.Types [0].Members [2]).Parameters[0].CustomAttributes[0].Name;
		}


        public CodeMethodInvokeExpression GenerateCodeInvocation (string serviceToInvoke){
            var activityServiceReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), VariableHelper.ToVariableName(serviceToInvoke));
            return new CodeMethodInvokeExpression (activityServiceReference, DataAccessServiceBuilder.ExecuteSqlQueryMethodName, new CodeExpression[] {});
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

