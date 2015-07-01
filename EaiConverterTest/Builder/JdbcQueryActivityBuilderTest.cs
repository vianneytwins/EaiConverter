﻿using NUnit.Framework;
using EaiConverter.Model;
using EaiConverter.Builder;
using System.Collections.Generic;
using System.CodeDom;

using EaiConverter.Test.Utils;
using System.Xml.Linq;

namespace EaiConverter.Test.Builder
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
			jdbcQueryActivityBuilder = new JdbcQueryActivityBuilder (
                new DataAccessBuilder (jdbcQueryBuilderUtils),
                new DataAccessServiceBuilder (jdbcQueryBuilderUtils),
                new DataAccessInterfacesCommonBuilder(),
                new XslBuilder(new XpathBuilder())
            );

		}

		[Test]
		public void Should_Return_One_DataAccess_Classes_To_Generate_When_JdbcQueryActivity_is_Mapped(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.AreEqual ("CurrencyDataAccess", classToGenerate [0].Types[0].Name);
		}

		[Test]
		public void Should_Return_One_constant_fields_for_the_sqlQueryStatement(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.IsTrue(classToGenerate [0].Types[0].Members[0].Attributes.HasFlag(MemberAttributes.Const));
		}

		[Test]
		public void Should_Return_One_constant_fields_Named_sqlQueryStatement(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.AreEqual ("sqlQueryStatement", classToGenerate [0].Types[0].Members[0].Name);
		}

		[Test]
		public void Should_Return_One_constant_fields_With_Value_equals_to_the_activity_Query_Value(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.AreEqual (jdbcQueryActivity.QueryStatement, ((CodePrimitiveExpression)((CodeMemberField)(classToGenerate [0].Types[0].Members[0])).InitExpression).Value);
		}

		[Test]
		public void Should_Return_One_Method_With_Name_ExecuteQuery(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.AreEqual ("ExecuteQuery", classToGenerate [0].Types[0].Members[3].Name);
		}

		[Test]
		public void Should_Return_One_public_Method_to_ExecuteQuery(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.IsTrue(((CodeMemberMethod)(classToGenerate [0].Types[0].Members[3])).Attributes.HasFlag(MemberAttributes.Public));
		}

		[Test]
		public void Should_Return_One_Method_With_one_inputParameter_of_type_string(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.AreEqual ("System.String", ((CodeMemberMethod)(classToGenerate [0].Types[0].Members[3])).Parameters[0].Type.BaseType);
		}

		[Test]
		public void Should_Return_One_Method_With_one_inputParameter_of_name_idBbUnique(){
            CodeNamespaceCollection classToGenerate = jdbcQueryActivityBuilder.Build (jdbcQueryActivity).ClassesToGenerate;
			Assert.AreEqual ("idBbUnique", ((CodeMemberMethod)(classToGenerate [0].Types[0].Members[3])).Parameters[0].Name);
		}

        [Test]
        public void Should_Return_void_Invocation_Code_When_Activity_has_no_return_type_And_No_Input(){
            CodeStatementCollection invocationExpression = jdbcQueryActivityBuilder.GenerateCodeInvocation ("MyService", this.jdbcQueryActivity);
            Assert.AreEqual ("this.logger.Info(\"Start Activity: Currency of type: com.tibco.plugin.jdbc.JDBCQueryActivity\");\n\nthis.myService.ExecuteQuery();\n", TestCodeGeneratorUtils.GenerateCode(invocationExpression));
        }

        [Test]
        public void Should_Return_void_Invocation_Code_When_Activity_has_return_type_And_No_Input(){
            this.jdbcQueryActivity.QueryOutputCachedSchemaColumns = "";
            CodeStatementCollection invocationExpression = jdbcQueryActivityBuilder.GenerateCodeInvocation ("MyService", this.jdbcQueryActivity);
            Assert.AreEqual ("this.logger.Info(\"Start Activity: Currency of type: com.tibco.plugin.jdbc.JDBCQueryActivity\");\n\nvar currency = this.myService.ExecuteQuery();\n", TestCodeGeneratorUtils.GenerateCode(invocationExpression));
        }

        [Test]
        public void Should_Return_void_Invocation_Code_When_Activity_has_no_return_type_And_1_input_parameter(){
            
            var xml =
                @"
        <jdbcQueryActivityInput xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
            <IdBbUnique xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
                <xsl:value-of select=""'test'""/>
            </IdBbUnique>
        </jdbcQueryActivityInput>
";
            XElement doc = XElement.Parse(xml);

            this.jdbcQueryActivity.InputBindings = doc.Nodes();
            jdbcQueryActivity.Parameters = new List<ClassParameter> {
                new ClassParameter{ Name = "IdBbUnique", Type = "String" }
            };

            CodeStatementCollection invocationExpression = jdbcQueryActivityBuilder.GenerateCodeInvocation ("MyService", this.jdbcQueryActivity);
            Assert.AreEqual (
                @"this.logger.Info(""Start Activity: Currency of type: com.tibco.plugin.jdbc.JDBCQueryActivity"");
string IdBbUnique = ""test"";

this.myService.ExecuteQuery(IdBbUnique);
", TestCodeGeneratorUtils.GenerateCode(invocationExpression));
        }
	}
}

