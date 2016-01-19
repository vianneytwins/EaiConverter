using NUnit.Framework;
using EaiConverter.Builder;
using EaiConverter.Model;

using System.Collections.Generic;
using EaiConverter.Test.Utils;

namespace EaiConverter.Test.Builder
{
	[TestFixture]
	public class DataAccessServiceBuilderTest
	{
		DataAccessServiceBuilder builder;
        JdbcQueryActivity jdbcQueryActivity;

		[SetUp]
		public void SetUp(){
			this.builder = new DataAccessServiceBuilder ();
            this.jdbcQueryActivity = new JdbcQueryActivity {
                Type = ActivityType.jdbcCallActivityType,
				Name = "TestJbdcQueryActivity",
				JdbcSharedConfig = "Panorama",
				EmptyStringAsNull = false,
				QueryStatement = "select 1 from toto where id= ?"

			};
		}

		[Test]
		public void Should_Return_dataAccess_execution_query_Wtih_no_param_When_ReturnType_is_not_void_and_No_param(){
			var expected = "this.dataAccess.ExecuteQuery();\n";

			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (executeQueryMethod);

            Assert.AreEqual (expected,classesInString);
		}

		[Test]
		public void Should_Return_dataAccess_execution_query_Wtih_no_param_When_ReturnType_is_not_void_and_has_2_params(){
			var expected = "this.dataAccess.ExecuteQuery(idBBUnique1, idBBUnique2);\n";
			this.jdbcQueryActivity.QueryStatementParameters = new Dictionary<string, string>
			{
				{"idBBUnique1","VARCHAR"},
				{"idBBUnique2","VARCHAR"}
			};
			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

            var classesInString = TestCodeGeneratorUtils.GenerateCode (executeQueryMethod);

            Assert.AreEqual (expected,classesInString);
		}


		[Test]
		public void Should_Return_queryMethod_return_When_activity_type_is_JDBCXXXactivity_And_output_paramnotNull(){
			var expected = "List<TestJbdcQueryActivity>";
			this.jdbcQueryActivity.QueryOutputStatementParameters = new List<ClassParameter>
			{
				new ClassParameter {Name = "idBBUnique1", Type = "VARCHAR"},
				new ClassParameter {Name = "idBBUnique2", Type = "VARCHAR"}
			};
			jdbcQueryActivity.Type = ActivityType.jdbcQueryActivityType;
			jdbcQueryActivity.ClassName = jdbcQueryActivity.Name;

			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

			Assert.AreEqual (expected,executeQueryMethod.ReturnType.BaseType);
		}

		[Test]
		public void Should_Return_queryMethod_return_When_activity_type_is_JDBCCallactivity_And_output_paramnotNull(){
			var expected = "TestJbdcQueryActivity";
			this.jdbcQueryActivity.QueryOutputStatementParameters = new List<ClassParameter>
			{
				new ClassParameter {Name = "idBBUnique1", Type = "VARCHAR"},
				new ClassParameter {Name = "idBBUnique2", Type = "VARCHAR"}
			};
		
			jdbcQueryActivity.ClassName = jdbcQueryActivity.Name;

			var executeQueryMethod = this.builder.GenerateExecuteQueryMethod (this.jdbcQueryActivity);

			Assert.AreEqual (expected,executeQueryMethod.ReturnType.BaseType);
		}

	}
}

