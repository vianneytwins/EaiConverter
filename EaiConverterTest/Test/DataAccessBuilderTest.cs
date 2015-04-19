using System;
using NUnit.Framework;
using EaiConverter.Mapper;
using EaiConverter.Mapper.Utils;
using System.CodeDom.Compiler;
using System.IO;
using EaiConverter.Model;
using System.CodeDom;
using System.Collections.Generic;
using EaiConverter.Test.Utils;

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
                Type = ActivityType.jdbcCallActivityType,
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
  sqlQueryStatement,
  new
    {
      idParam = idParam
    }
  );
}

";
			this.jdbcQueryActivity.QueryStatementParameters = new Dictionary <string,string> { { "idParam","System.String" } };

			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (executeQueryMethod);

            Assert.AreEqual (expected, classesInString);
		}

		[Test]
		public void Should_Return_void_body_Statement_Of_executeQuery_Method_When_return_type_is_Void_with_No_Param(){
			var expected = @"using (IDataAccess db = this.dataAccessFactory.CreateAccess())
{
db.Query(
  sqlQueryStatement);
}

";


			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (executeQueryMethod);

            Assert.AreEqual (expected,classesInString);
		}

		[Test]
		public void Should_Return_string_body_Statement_Of_executeQuery_Method_When_return_type_is_Void_with_No_Param(){
			var expected = @"using (IDataAccess db = this.dataAccessFactory.CreateAccess())
{
return db.Query <System.Int>(
  sqlQueryStatement).FirstOrDefault();
}

";
			this.jdbcQueryActivity.QueryOutputCachedSchemaDataTypes = 4;

			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (executeQueryMethod);

            Assert.AreEqual (expected,classesInString);
		}



	}
}

