﻿using System;
using NUnit.Framework;
using EaiConverter.Mapper;
using EaiConverter.Model;
using EaiConverter.Mapper.Utils;
using System.CodeDom.Compiler;
using System.IO;
using System.CodeDom;
using System.Collections.Generic;
using EaiConverter.Test.Utils;

namespace EaiConverter
{
	[TestFixture]
	public class DataAccessServiceBuilderTest
	{
		DataAccessServiceBuilder builder;
		JdbcQueryActivity jdbcQueryActivity;

		[SetUp]
		public void SetUp(){
			this.builder = new DataAccessServiceBuilder (new JdbcQueryBuilderUtils());
			this.jdbcQueryActivity = new JdbcQueryActivity {
				Type = JdbcQueryActivity.jdbcCallActivityType,
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

			var classesInString = GenerateCode (executeQueryMethod);

            Assert.AreEqual (expected,classesInString.RemoveWindowsReturnLineChar());
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

			var classesInString = GenerateCode (executeQueryMethod);

            Assert.AreEqual (expected,classesInString.RemoveWindowsReturnLineChar());
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

