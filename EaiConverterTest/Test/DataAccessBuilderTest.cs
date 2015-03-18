using System;
using NUnit.Framework;
using EaiConverter.Mapper;
using EaiConverter.Mapper.Utils;
using System.CodeDom.Compiler;
using System.IO;
using EaiConverter.Model;
using System.CodeDom;
using System.Collections.Generic;

namespace EaiConverter
{
	[TestFixture]
	public class DataAccessBuilderTest
	{
		DataAccessBuilder builder;
		JdbcQueryActivity jdbcQueryActivity;

		[SetUp]
		public void SetUp(){
			this.builder = new DataAccessBuilder (new JdbcQueryBuilderUtils());
			this.jdbcQueryActivity = new JdbcQueryActivity {
				Type = JdbcQueryActivity.jdbcCallActivityType,
				Name = "TestJbdcQueryActivity",
				JdbcSharedConfig = "Panorama",
				EmptyStringAsNull = false,
				QueryStatement = "select 1 from toto where id= ?"

			};
		}

		[Test]
		public void Should_Return_void_body_Statement_Of_executeQuery_Method_When_return_type_is_Void_with_One_Param(){
			var expected = @"using (IDataAccess db = this.dataAccessFactory.CreateAccess())
{
db.Query(
  ""select 1 from toto where id= ?"",
  new
    {
      idParam = idParam
    }
  );
}

";
			this.jdbcQueryActivity.QueryStatementParameters = new Dictionary <string,string> { { "idParam","System.String" } };

			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

			var classesInString = GenerateCode (executeQueryMethod);

			Assert.AreEqual (expected,classesInString);
		}

		[Test]
		public void Should_Return_void_body_Statement_Of_executeQuery_Method_When_return_type_is_Void_with_No_Param(){
			var expected = @"using (IDataAccess db = this.dataAccessFactory.CreateAccess())
{
db.Query(
  ""select 1 from toto where id= ?"");
}

";


			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

			var classesInString = GenerateCode (executeQueryMethod);

			Assert.AreEqual (expected,classesInString);
		}

		[Test]
		public void Should_Return_string_body_Statement_Of_executeQuery_Method_When_return_type_is_Void_with_No_Param(){
			var expected = @"using (IDataAccess db = this.dataAccessFactory.CreateAccess())
{
return db.Query <System.Int>(
  ""select 1 from toto where id= ?"").FirstOrDefault();
}

";
			this.jdbcQueryActivity.QueryOutputCachedSchemaDataTypes = 4;

			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

			var classesInString = GenerateCode (executeQueryMethod);

			Assert.AreEqual (expected,classesInString);
		}


		private string GenerateCode (CodeMemberMethod executeQueryMethod)
		{
			var classGenerator = CodeDomProvider.CreateProvider ("CSharp");
			var options = new CodeGeneratorOptions ();
			options.BracingStyle = "C";
			string classesInString;
			using (StringWriter writer = new StringWriter ()) {
				classGenerator.GenerateCodeFromStatement (executeQueryMethod.Statements [0], writer, options);
				classesInString = writer.GetStringBuilder ().ToString ();
			}
			return classesInString;
		}
	}
}

