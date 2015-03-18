using System;
using System.Collections.Generic;
using TibcoBWConverter.Model;
using TibcoBWConverter.CodeGenerator;
using System.Text;
using TibcoBWConverter.CodeGenerator.utils;

namespace TibcoBWConverter.Mapper
{
	public class JdbcQueryActivityMapper
	{
		public JdbcQueryActivityMapper ()
		{
		}

		private const string bodyMethodStart = "#using (var db = this.dataAccessFactory.CreateAccess())\n#{\n";
		private const string dbQuery = "db.Query";

		const string voidString = "void";

		private Dictionary<string,string> sqlMapping = new Dictionary <string, string> {
			{"VARCHAR","string"},
			{"INT","int"},
			{"12","string"},
			{"4", "int"}
		};
		private Dictionary<string,string> jdbcSharedConfigMapping = new Dictionary <string, string> {
			{"/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc","[IntegrationDatabase]"},
			{"/Configuration/DAI/PNO/JDBC/JDBCPanorama.sharedjdbc","[PanoramaDatabase]"},
			{"/Configuration/DAI/PNO/JDBC/JDBCPanoramaMasterFiles.sharedjdbc","[MasterFilesDatabase]"}
		};

		public List<ClassModel> Build (JdbcQueryActivity jdbcQueryActivity)
		{
			var classesToGenerate = new List<ClassModel> ();
			var dataAccessClass = this.BuildDataAccess (jdbcQueryActivity);
			var dataAccessInterface = InterfaceExtractorFromClass.Extract (dataAccessClass);
			dataAccessClass.InterfacesToImplement = new List<string> { dataAccessInterface.Name };
			classesToGenerate.Add(dataAccessClass);
			classesToGenerate.Add(dataAccessInterface);

			return classesToGenerate;
		}

		public ClassModel BuildDataAccess (JdbcQueryActivity jdbcQueryActivity)
		{
			var dataAccessToGenerate = new ClassModel ();
			dataAccessToGenerate.Name = VariableHelper.ToClassName (jdbcQueryActivity.Name) + "DataAccess";
			dataAccessToGenerate.Namespace = "DataAccess";

			dataAccessToGenerate.Imports = this.GenerateImport (jdbcQueryActivity);
			dataAccessToGenerate.Fields = this.GenererateFields (jdbcQueryActivity);
			dataAccessToGenerate.Constructors = this.GenererateContructors (jdbcQueryActivity, dataAccessToGenerate);
			dataAccessToGenerate.Methods = this.GenererateMethods(jdbcQueryActivity);

			return dataAccessToGenerate;
		}


		List<ClassParameter> GenererateFields (JdbcQueryActivity jdbcQueryActivity)
		{
			var fields = new List<ClassParameter> {
				new ClassParameter {
					Name = "sqlQueryStatement",
					Type = "string",
					IsAConstant = true,
					DefaultValue = "\""+ jdbcQueryActivity.QueryStatement +"\""
				},
				new ClassParameter {
					Name = "dataAccessFactory",
					Type = "IDataAccessFactory",
					IsReadOnly = true
				},
			};
			return fields;
		}

		List<Method> GenererateMethods (JdbcQueryActivity jdbcQueryActivity)
		{
			return new List<Method>{ this.GenerateExecuteQueryMethod(jdbcQueryActivity)};
		}

		List<Method> GenererateContructors (JdbcQueryActivity jdbcQueryActivity, ClassModel classModel)
		{
			var constructor = new Method ();
			constructor.IsPublic = true;
			constructor.ReturnParameter = new ClassParameter {
				Name = string.Empty,
				Type = classModel.Name
			};

			var inputParameters = new List<ClassParameter> ();
			var sb = new StringBuilder ();

			sb.AppendLine (string.Format ("#this.{0} = {0};", classModel.Fields [1].Name));
				inputParameters.Add (new ClassParameter {
				Name = classModel.Fields[1].Name,
				Type = classModel.Fields[1].Type,
				SpecialOption = this.ConvertJDBCConnectionName (jdbcQueryActivity.JdbcSharedConfig)
				});

			constructor.InputParameters = inputParameters;
			constructor.MethodBody = sb.ToString ();

			return new List<Method> { constructor };
		}

		Method GenerateExecuteQueryMethod (JdbcQueryActivity jdbcQueryActivity)
		{
			var method = new Method ();
			method.Name = "ExecuteQuery";
			//TODO 
			method.ReturnParameter = new ClassParameter{
				Type = ConvertSQLTypeToObjectType(jdbcQueryActivity.QueryOutputCachedSchemaDataTypes.ToString()),
				Name = string.Empty
			};
			method.IsPublic = true;
			method.InputParameters = this.ConvertQueryStatementParameter(jdbcQueryActivity.QueryStatementParameters);

			this.GenerateExecuteQueryBody (jdbcQueryActivity, method);


			return method;
		}

		public void GenerateExecuteQueryBody (JdbcQueryActivity jdbcQueryActivity, Method method)
		{
			var sb = new StringBuilder ();
			sb.Append (bodyMethodStart);
			var tabulation = new Tab ();
			if (method.ReturnParameter.Type == voidString) {
				sb.AppendLine (string.Format ("#{0}(", dbQuery));
			}
			else {
				sb.AppendLine (string.Format ("#return {0}<{1}> (", dbQuery, method.ReturnParameter.Type));
			}
			tabulation.Increment ();
			sb.AppendLine (string.Format ("#{0}\"{1}\",", tabulation, jdbcQueryActivity.QueryStatement));
			if (method.InputParameters != null && method.InputParameters.Count >= 1) {
				sb.AppendLine (string.Format ("#{0}new", tabulation));
				sb.AppendLine (string.Format ("#{0}{{", tabulation.Increment ()));
				tabulation.Increment ();
				foreach (var inputParameter in method.InputParameters) {
					sb.AppendLine (string.Format ("#{0}{1} = {1},", tabulation, inputParameter.Name));
				}
				// remoev last comma
				sb.Remove (sb.Length - 2, 1);
				tabulation.Decrement ();
				sb.AppendLine (string.Format ("#{0}}}", tabulation));
			}
			//ferme le dbQuery
			tabulation.Decrement ();
			sb.AppendLine (string.Format ("#{0}).FirstOrDefault();", tabulation));
			//ferme le using
			sb.AppendLine ("#}");
			method.MethodBody = sb.ToString ();
		}

		List<ClassParameter> ConvertQueryStatementParameter (Dictionary<string,string> queryStatementParameters)
		{
			var methodInputParameters = new List<ClassParameter> ();
			if (queryStatementParameters == null) {
				return methodInputParameters;
			}
			foreach (var queryParam in queryStatementParameters) {
				methodInputParameters.Add(new ClassParameter {
					Name = VariableHelper.ToVariableName(queryParam.Key),
					Type = this.ConvertSQLTypeToObjectType (queryParam.Value)
				});
			}
			return methodInputParameters;
		}
				
		string ConvertSQLTypeToObjectType (string type)
		{
			// TODO pour le moment on laisse comme ca car on veut lister tous les types et a mettre dans le dico et donc que cela plente
			string resultType;
			if (sqlMapping.TryGetValue (type, out resultType)) {
				return resultType;
			}
			else
			{
				return voidString;

			}
			
		}

		string ConvertJDBCConnectionName (string type)
		{
			string resultType;
			if (jdbcSharedConfigMapping.TryGetValue (type, out resultType)) {
				return resultType;
			}
			else
			{
				return "[PanoramaDatabase]";

			}
		}

		public List<string> GenerateImport (JdbcQueryActivity jdbcQueryActivity)
		{
			return new List<string> {
				"System",
				"System.Linq",
				"Lyxor.Panorama.DataAccess.Common"
			};
		}

		void BuildDataAccessInterface (ClassModel dataAccessClass)
		{
			//throw new NotImplementedException ();
		}
	}
}

