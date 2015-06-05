using System;
using System.CodeDom;
using System.Runtime.InteropServices;
using EaiConverter.Builder.Utils;

namespace EaiConverter.Builder
{
	public class DataAccessInterfacesCommonBuilder
	{

		const string voidString = "System.Void";

		public CodeNamespace Build () {
			var namespaceResult = new CodeNamespace (TargetAppNameSpaceService.dataAccessCommonNamespace);

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
			var sqlQuery = new CodeParameterDeclarationExpression ("System.String", "query");
			var sqlQueryParameter = new CodeParameterDeclarationExpression () {
				Type = new CodeTypeReference ("System.Object"),
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
				ReturnType = new CodeTypeReference (voidString)
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

