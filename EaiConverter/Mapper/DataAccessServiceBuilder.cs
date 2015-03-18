using System;
using System.CodeDom;
using System.Reflection;
using System.Collections.Generic;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Mapper.Utils;
using EaiConverter.Model;

namespace EaiConverter.Mapper
{
	public class DataAccessServiceBuilder 
	{

		readonly JdbcQueryBuilderUtils jdbcQueryBuilderUtils;

		public DataAccessServiceBuilder (JdbcQueryBuilderUtils jdbcQueryBuilderUtils){
			this.jdbcQueryBuilderUtils = jdbcQueryBuilderUtils;
		}

		//TODO cahnger en 1 seul namespace
		public CodeNamespace Build (JdbcQueryActivity jdbcQueryActivity){

			var serviceNameSpace = new CodeNamespace (TargetAppNameSpaceService.domainServiceNamespaceName);
			serviceNameSpace.Imports.AddRange (this.GenerateServiceImport (jdbcQueryActivity));


			var serviceToGenerate = new CodeTypeDeclaration ();
			serviceToGenerate.IsClass = true;
			serviceToGenerate.TypeAttributes = TypeAttributes.Public;

			serviceToGenerate.Name = VariableHelper.ToClassName (jdbcQueryActivity.Name) + "Service";

			//dataAccessToGenerate.Imports = this.GenerateImport (jdbcQueryActivity);
			serviceToGenerate.Members.AddRange(this.GenererateFields (jdbcQueryActivity));
			serviceToGenerate.Members.AddRange(this.GenererateContructors (jdbcQueryActivity, serviceToGenerate));
			serviceToGenerate.Members.AddRange(this.GenererateMethods(jdbcQueryActivity));

			serviceNameSpace.Types.Add (serviceToGenerate);

			return serviceNameSpace;
		}


		public CodeNamespaceImport[] GenerateServiceImport (JdbcQueryActivity jdbcQueryActivity)
		{
			return new CodeNamespaceImport[3] {
				new CodeNamespaceImport ("System"),
				//TODO : aps beau devrait etre loadé dans une conf
				new CodeNamespaceImport (TargetAppNameSpaceService.domainContractNamespaceName),
				new CodeNamespaceImport (TargetAppNameSpaceService.dataAccessNamespace)
			};
		}


		private CodeMemberField[] GenererateFields (JdbcQueryActivity jdbcQueryActivity)
		{
			var fields = new List<CodeMemberField> {
				new CodeMemberField {
					Name = "dataAccess",
					Type = new CodeTypeReference (VariableHelper.ToClassName (jdbcQueryActivity.Name) + "DataAccess"),
					Attributes = MemberAttributes.Private 
				},
			};
			return fields.ToArray();
		}

		private CodeMemberMethod[] GenererateMethods (JdbcQueryActivity jdbcQueryActivity)
		{
			return new CodeMemberMethod[1]{ this.GenerateExecuteQueryMethod(jdbcQueryActivity)};
		}

		private CodeConstructor[] GenererateContructors (JdbcQueryActivity jdbcQueryActivity, CodeTypeDeclaration classModel)
		{
			var constructor = new CodeConstructor();
			constructor.Attributes = MemberAttributes.Public;

			foreach (CodeMemberField field in classModel.Members) {

					constructor.Parameters.Add (
						new CodeParameterDeclarationExpression () {
							Type = field.Type,
							Name = field.Name
							}
						);

					var parameterReference = new CodeFieldReferenceExpression (
						new CodeThisReferenceExpression (), field.Name);

					constructor.Statements.Add (new CodeAssignStatement (parameterReference,
						new CodeArgumentReferenceExpression (field.Name)));

			}

			return new List<CodeConstructor> { constructor }.ToArray();
		}

		public CodeMemberMethod GenerateExecuteQueryMethod (JdbcQueryActivity jdbcQueryActivity)
		{
			{
				var method = new CodeMemberMethod ();
				method.Attributes = MemberAttributes.Public | MemberAttributes.Final;

				method.Name = DataAccessBuilder.executeQuery;

				method.ReturnType = this.jdbcQueryBuilderUtils.ConvertSQLTypeToObjectType (jdbcQueryActivity.QueryOutputCachedSchemaDataTypes.ToString ());

				method.Parameters.AddRange(this.jdbcQueryBuilderUtils.ConvertQueryStatementParameter(jdbcQueryActivity.QueryStatementParameters));

				method.Statements.Add (this.GenerateExecuteQueryBody (method));


				return method;
			}
		}

		public CodeStatement GenerateExecuteQueryBody (CodeMemberMethod method)
		{
			var dataAccessReference = new CodeFieldReferenceExpression ( new CodeThisReferenceExpression (), "dataAccess");
			var methodArgumentReferences = new CodeArgumentReferenceExpression[method.Parameters.Count];

			for (int i = 0; i < method.Parameters.Count; i++) {
				methodArgumentReferences [i] = new CodeArgumentReferenceExpression (method.Parameters[i].Name);
			}
		
			var invocationExpression = new CodeMethodInvokeExpression (dataAccessReference, "ExecuteQuery", methodArgumentReferences);

			if (method.ReturnType.BaseType != "void") {
				return  new CodeMethodReturnStatement (invocationExpression);
			} else {
				return new CodeExpressionStatement(invocationExpression);
			}
		}
	}
}

