using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EaiConverter.CodeGenerator.Utils;
using EaiConverter.Mapper.Utils;
using EaiConverter.Model;

namespace EaiConverter.Mapper
{
	public class DataAccessBuilder
	{
		private const string bodyMethodStart = "using (IDataAccess db = this.dataAccessFactory.CreateAccess())\n{\n";
		private const string dbQuery = "db.Query";
		const string iDataAccessFactory = "IDataAccessFactory";

        const string SqlQueryStatement = "sqlQueryStatement";

		const string voidString = "void";

		readonly JdbcQueryBuilderUtils jdbcQueryBuilderUtils;

		public DataAccessBuilder (JdbcQueryBuilderUtils jdbcQueryBuilderUtils){
			this.jdbcQueryBuilderUtils = jdbcQueryBuilderUtils;
		}

		public CodeNamespace Build (JdbcQueryActivity jdbcQueryActivity)
		{
			var dataAccessNameSpace = new CodeNamespace (TargetAppNameSpaceService.dataAccessNamespace);
			dataAccessNameSpace.Imports.AddRange (this.GenerateImport (jdbcQueryActivity));

			var dataAccessToGenerate = new CodeTypeDeclaration ();
			dataAccessToGenerate.IsClass = true;
			dataAccessToGenerate.TypeAttributes = TypeAttributes.Public;

			dataAccessToGenerate.Name = VariableHelper.ToClassName (jdbcQueryActivity.Name) + "DataAccess";


			//dataAccessToGenerate.Imports = this.GenerateImport (jdbcQueryActivity);
			dataAccessToGenerate.Members.AddRange(this.GenererateFields (jdbcQueryActivity));
			dataAccessToGenerate.Members.AddRange(this.GenererateContructors (jdbcQueryActivity, dataAccessToGenerate));
			dataAccessToGenerate.Members.AddRange(this.GenererateMethods(jdbcQueryActivity));

			dataAccessNameSpace.Types.Add (dataAccessToGenerate);

			return dataAccessNameSpace;
		}

		public CodeNamespaceImport[] GenerateImport (JdbcQueryActivity jdbcQueryActivity)
		{
			return new CodeNamespaceImport[3] {
				new CodeNamespaceImport ("System"),
				new CodeNamespaceImport ("System.Linq"),
				new CodeNamespaceImport (TargetAppNameSpaceService.dataAccessCommonNamespace)
			};
		}

		private CodeMemberField[] GenererateFields (JdbcQueryActivity jdbcQueryActivity)
		{
			var fields = new List<CodeMemberField> {
				new CodeMemberField {
                    Name = SqlQueryStatement,
					Type = new CodeTypeReference(typeof(System.String)),
					Attributes = MemberAttributes.Private | MemberAttributes.Const,
					InitExpression = new CodePrimitiveExpression( jdbcQueryActivity.QueryStatement )
				},
				new CodeMemberField {
					Name = "dataAccessFactory",
					Type = new CodeTypeReference (iDataAccessFactory),
					Attributes = MemberAttributes.Private 
				},
			};
			return fields.ToArray();
		}

		public CodeMemberMethod[] GenererateMethods (JdbcQueryActivity jdbcQueryActivity)
		{
			return new CodeMemberMethod[1]{ this.GenerateExecuteQueryMethod(jdbcQueryActivity)};
		}

		public CodeConstructor[] GenererateContructors (JdbcQueryActivity jdbcQueryActivity, CodeTypeDeclaration classModel)
		{
			var constructor = new CodeConstructor();
			constructor.Attributes = MemberAttributes.Public;

			foreach (CodeMemberField field in classModel.Members) {
				if (field.Type.BaseType == iDataAccessFactory) {
					constructor.Parameters.Add (
						new CodeParameterDeclarationExpression () {
							Type = field.Type,
							Name = field.Name,
							// TODO verifier que ca marche
							CustomAttributes = new CodeAttributeDeclarationCollection {
								new CodeAttributeDeclaration (
									this.jdbcQueryBuilderUtils.ConvertJDBCConnectionName (jdbcQueryActivity.JdbcSharedConfig)
								)
							}
						});

					var parameterReference = new CodeFieldReferenceExpression (
						new CodeThisReferenceExpression (), field.Name);

					constructor.Statements.Add (new CodeAssignStatement (parameterReference,
						new CodeArgumentReferenceExpression (field.Name)));
				}

			}

			return new List<CodeConstructor> { constructor }.ToArray();
		}

		public CodeMemberMethod GenerateExecuteQueryMethod (JdbcQueryActivity jdbcQueryActivity)
		{
			var method = new CodeMemberMethod ();
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            method.Name = DataAccessServiceBuilder.ExecuteSqlQueryMethodName;

			method.ReturnType = this.jdbcQueryBuilderUtils.ConvertSQLTypeToObjectType (jdbcQueryActivity.QueryOutputCachedSchemaDataTypes.ToString ());

			method.Parameters.AddRange(this.jdbcQueryBuilderUtils.ConvertQueryStatementParameter(jdbcQueryActivity.QueryStatementParameters));

			method.Statements.Add (this.GenerateExecuteQueryBody (jdbcQueryActivity, method));


			return method;
		}

		public CodeSnippetStatement GenerateExecuteQueryBody (JdbcQueryActivity jdbcQueryActivity, CodeMemberMethod method)
		{
			var sb = new StringBuilder ();
			sb.Append (bodyMethodStart);
			var tabulation = new Tab ();
			if (method.ReturnType.BaseType == voidString) {
				sb.AppendLine (string.Format ("{0}(", dbQuery));
			}
			else {
				sb.AppendLine (string.Format ("return {0} <{1}>(", dbQuery, method.ReturnType.BaseType));
			}
			tabulation.Increment ();
            sb.Append (string.Format ("{0}{1}", tabulation, SqlQueryStatement));

			if (method.Parameters != null && method.Parameters.Count >= 1) {
				sb.AppendLine (",");
				sb.AppendLine (string.Format ("{0}new", tabulation));
				sb.AppendLine (string.Format ("{0}{{", tabulation.Increment ()));
				tabulation.Increment ();
				foreach (CodeParameterDeclarationExpression inputParameter in method.Parameters) {
					sb.AppendLine (string.Format ("{0}{1} = {1},", tabulation, inputParameter.Name));
				}
				// remove last comma
                sb.Remove (sb.ToString().LastIndexOf(','), 1);
				tabulation.Decrement ();
				sb.AppendLine (string.Format ("{0}}}", tabulation));
			}
			//ferme le dbQuery
			tabulation.Decrement ();
			if (method.ReturnType.BaseType != voidString) {
				sb.AppendLine (string.Format ("{0}).FirstOrDefault();", tabulation));
			} else {
				sb.AppendLine (string.Format ("{0});", tabulation));
			}
			//ferme le using
			sb.AppendLine ("}");
			return new CodeSnippetStatement ( sb.ToString ());
		}


	}
}

