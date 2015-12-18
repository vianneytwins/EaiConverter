namespace EaiConverter.Builder
{
    using System.CodeDom;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Utils;

    public class DataAccessInterfacesCommonBuilder
	{


		public CodeNamespace Build () {
			var namespaceResult = new CodeNamespace (TargetAppNameSpaceService.dataAccessCommonNamespace());

			this.GenerateImports (namespaceResult);

			var dataAccessInterface = this.GenerateDataAccessInterface ();
			namespaceResult.Types.Add (dataAccessInterface);

			var dataAccessFactoryInterface = this.GenerateDataAccessFactoryInterface ();
			namespaceResult.Types.Add (dataAccessFactoryInterface);

			return namespaceResult;
		}

		public void GenerateImports (CodeNamespace namespaceResult)
		{
			namespaceResult.Imports.Add (new CodeNamespaceImport ("System"));
			namespaceResult.Imports.Add (new CodeNamespaceImport ("System.Collections.Generic"));
		}

		public CodeTypeDeclaration GenerateDataAccessInterface ()
		{
			var dataAccessInterface = new CodeTypeDeclaration ();
			dataAccessInterface.Name = "IDataAccess";
			dataAccessInterface.IsInterface = true;
			dataAccessInterface.BaseTypes.Add (new CodeTypeReference ("IDisposable"));

			var parameters = this.GenerateMethodsParameters ();

			var methodeQueryVoid = this.GenerateQueryMethodWithVoidReturnType (parameters);
			dataAccessInterface.Members.Add (methodeQueryVoid);

			var methodQueryList = GenerateQueryMethodWithReturnType (parameters);
			dataAccessInterface.Members.Add (methodQueryList);
			return dataAccessInterface;
		}

		public CodeParameterDeclarationExpressionCollection GenerateMethodsParameters ()
		{
            var sqlQuery = new CodeParameterDeclarationExpression (CSharpTypeConstant.SystemString, "query");
			var sqlQueryParameter = new CodeParameterDeclarationExpression () {
                Type = new CodeTypeReference (CSharpTypeConstant.SystemObject),
				Name = "paramater = null",
			// TODO : find a way to do it properly
			//				CustomAttributes = new CodeAttributeDeclarationCollection {
			//					new CodeAttributeDeclaration ("Optional")
			//				}
			};
			var parameters = new CodeParameterDeclarationExpressionCollection ();
			parameters.Add (sqlQuery);
			parameters.Add (sqlQueryParameter);
			return parameters;
		}

		public CodeMemberMethod GenerateQueryMethodWithVoidReturnType (CodeParameterDeclarationExpressionCollection parameters)
		{
			var methodeQueryVoid = new CodeMemberMethod {
				Name = "Query",
                ReturnType = new CodeTypeReference (CSharpTypeConstant.SystemVoid)
			};
			methodeQueryVoid.Parameters.AddRange (parameters);
			return methodeQueryVoid;
		}

		public CodeMemberMethod GenerateQueryMethodWithReturnType (CodeParameterDeclarationExpressionCollection parameters)
		{
			var methodQueryList = new CodeMemberMethod {
				Name = "Query<TPoco>",
				ReturnType = new CodeTypeReference ("IEnumerable<TPoco>")
			};
			methodQueryList.Parameters.AddRange (parameters);
			return methodQueryList;
		}

		public CodeTypeDeclaration GenerateDataAccessFactoryInterface ()
		{
			var dataAccessFactoryInterface = new CodeTypeDeclaration ();
			dataAccessFactoryInterface.Name = "IDataAccessFactory";
			dataAccessFactoryInterface.IsInterface = true;
			dataAccessFactoryInterface.Members.Add (new CodeMemberMethod {
				Name = "CreateAccess",
				ReturnType = new CodeTypeReference ("IDataAccess")
			});
			return dataAccessFactoryInterface;
		}
	}
}

