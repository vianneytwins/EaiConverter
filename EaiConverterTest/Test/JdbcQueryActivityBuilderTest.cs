using System;
using NUnit.Framework;
using EaiConverter.Model;
using EaiConverter.Mapper;
using System.Collections.Generic;
using System.CodeDom;
using EaiConverter.Mapper.Utils;

namespace EaiConverter
{
	[TestFixture]
	public class JdbcQueryActivityBuilderTest
	{
		JdbcQueryActivity jdbcQueryActivity;
		JdbcQueryActivityBuilder jdbcQueryActivityBuilder;

		const string select = "Select 1";

		[SetUp]
		public void SetUp() {
            jdbcQueryActivity = new JdbcQueryActivity ("Currency" , ActivityType.jdbcQueryActivityType);
			jdbcQueryActivity.QueryStatement = select;
			jdbcQueryActivity.QueryStatementParameters = new Dictionary<string, string> {
				{
					"IdBbUnique",
					"VARCHAR"
				}
			};
			jdbcQueryActivity.JdbcSharedConfig = string.Empty;
			var jdbcQueryBuilderUtils = new JdbcQueryBuilderUtils ();
			jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder (new DataAccessBuilder (jdbcQueryBuilderUtils), new DataAccessServiceBuilder (jdbcQueryBuilderUtils), new DataAccessInterfacesCommonBuilder());

		}

		[Test]
		public void Should_Return_One_DataAccess_Classes_To_Generate_When_JdbcQueryActivity_is_Mapped(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.AreEqual ("CurrencyDataAccess", classToGenerate [0].Types[0].Name);
		}

		[Test]
		public void Should_Return_One_constant_fields_for_the_sqlQueryStatement(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.IsTrue(classToGenerate [0].Types[0].Members[0].Attributes.HasFlag(MemberAttributes.Const));
		}

		[Test]
		public void Should_Return_One_constant_fields_Named_sqlQueryStatement(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.AreEqual ("sqlQueryStatement", classToGenerate [0].Types[0].Members[0].Name);
		}

		[Test]
		public void Should_Return_One_constant_fields_With_Value_equals_to_the_activity_Query_Value(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.AreEqual (jdbcQueryActivity.QueryStatement, ((CodePrimitiveExpression)((CodeMemberField)(classToGenerate [0].Types[0].Members[0])).InitExpression).Value);
		}

		[Test]
		public void Should_Return_One_Method_With_Name_ExecuteQuery(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.AreEqual ("ExecuteQuery", classToGenerate [0].Types[0].Members[3].Name);
		}

		[Test]
		public void Should_Return_One_public_Method_to_ExecuteQuery(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.IsTrue(((CodeMemberMethod)(classToGenerate [0].Types[0].Members[3])).Attributes.HasFlag(MemberAttributes.Public));
		}

		[Test]
		public void Should_Return_One_Method_With_one_inputParameter_of_type_string(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.AreEqual ("System.String", ((CodeMemberMethod)(classToGenerate [0].Types[0].Members[3])).Parameters[0].Type.BaseType);
		}

		[Test]
		public void Should_Return_One_Method_With_one_inputParameter_of_name_idBbUnique(){
			CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity);
			Assert.AreEqual ("idBbUnique", ((CodeMemberMethod)(classToGenerate [0].Types[0].Members[3])).Parameters[0].Name);
		}
	}
}

