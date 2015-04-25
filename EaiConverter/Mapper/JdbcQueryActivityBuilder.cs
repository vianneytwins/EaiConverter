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
            var dataAccessNameSpace = this.dataAccessBuilder.Build (jdbcQueryActivity);
			var dataAccessInterfaceNameSpace = InterfaceExtractorFromClass.Extract (dataAccessNameSpace.Types[0], TargetAppNameSpaceService.dataAccessNamespace );
			dataAccessNameSpace.Types[0].BaseTypes.Add(new CodeTypeReference(dataAccessInterfaceNameSpace.Types[0].Name));

			var serviceNameSpaces = this.dataAccessServiceBuilder.Build (jdbcQueryActivity);
			var serviceInterfaceNameSpace = InterfaceExtractorFromClass.Extract (serviceNameSpaces.Types[0], TargetAppNameSpaceService.domainContractNamespaceName );
			serviceNameSpaces.Types[0].BaseTypes.Add(new CodeTypeReference(serviceInterfaceNameSpace.Types[0].Name));

			var dataCommonNamespace = this.dataAccessCommonBuilder.Build ();

			//TODO inject in construtor ? il faut aller chercher le nom du parametre du custom attributes du constructor ... ugly non ?
			var dataBaseAttributeNamspace = new DatabaseAttributeBuilder ().Build (GetDataCustomAttributeName (dataAccessNameSpace));

            var result = new ActivityCodeDom();
            result.ClassesToGenerate = new CodeNamespaceCollection {
				dataAccessNameSpace,
				dataAccessInterfaceNameSpace,
				serviceNameSpaces,
				serviceInterfaceNameSpace,
				dataCommonNamespace,
				dataBaseAttributeNamspace}
				;

			return result;
		}
			

		string GetDataCustomAttributeName (CodeNamespace dataAccessNameSpace)
		{
			return ((CodeMemberMethod) dataAccessNameSpace.Types [0].Members [2]).Parameters[0].CustomAttributes[0].Name;
		}
	}
}

