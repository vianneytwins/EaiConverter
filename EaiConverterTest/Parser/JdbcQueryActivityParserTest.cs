using NUnit.Framework;
using System.Xml.Linq;

using EaiConverter.Parser;
using EaiConverter.Model;

namespace EaiConverter.Test.Parser
{

	[TestFixture]
	public class JdbcQueryActivityParserTest
	{
		JdbcQueryActivityParser jdbcQueryActivityParser;
		XElement doc;

		[SetUp]
		public void SetUp ()
		{
			jdbcQueryActivityParser = new JdbcQueryActivityParser ();
			var xml =
                @"<pd:activity name=""GetUndlCurrency"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.jdbc.JDBCQueryActivity</pd:type>
<config>
<timeout>10</timeout>
<commit>false</commit>
<maxRows>100</maxRows>
<emptyStrAsNil>false</emptyStrAsNil>
<jdbcSharedConfig>/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc</jdbcSharedConfig>
<statement>select CRNCY from T_EQUITY_PNO where ID_BB_UNIQUE= ?</statement>
<Prepared_Param_DataType>
	<parameter>
		<parameterName>IdBbUnique</parameterName>
		<dataType>VARCHAR</dataType>
	</parameter>
	<parameter>
		<parameterName>IdBbUnique2</parameterName>
		<dataType>VARCHAR</dataType>
	</parameter>
</Prepared_Param_DataType>
<QueryOutputCachedSchemaColumns>CRNCY</QueryOutputCachedSchemaColumns>
<QueryOutputCachedSchemaDataTypes>12</QueryOutputCachedSchemaDataTypes>
<QueryOutputCachedSchemaStatus>OptionalElement</QueryOutputCachedSchemaStatus>
</config>
<pd:inputBindings>
    <jdbcQueryActivityInput>
        <IdBbUnique>
            <xsl:value-of select=""testvalue""/>
        </IdBbUnique>
        <IdBbUnique2>
            <xsl:value-of select=""EVL""/>
        </IdBbUnique2>
    </jdbcQueryActivityInput>
</pd:inputBindings>
</pd:activity>";
			doc = XElement.Parse(xml);
		}

		[Test]
		public void Should_Return_Activity_Type_Is_JDBCQueryActivity (){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.jdbc.JDBCQueryActivity", jdbcQueryActivity.Type.ToString());
		}

		[Test]
		public void Should_Return_Activity_Name_Is_GetUndlCurrency(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("GetUndlCurrency", jdbcQueryActivity.Name);
		}

		[Test]
		public void Should_Return_timeOut_is_10(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual (10, jdbcQueryActivity.TimeOut);
		}

		[Test]
		public void Should_Return_maxRows_is_100(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual (100, jdbcQueryActivity.MaxRows);
		}

		[Test]
		public void Should_Return_Commit_is_false(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual (false, jdbcQueryActivity.Commit);
		}

		[Test]
		public void Should_Return_emptyStringAsNull_is_false(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual (false, jdbcQueryActivity.EmptyStringAsNull);
		}

		[Test]
		public void Should_Return_jdbcSharedConfig_is_JDBCIntegration_dot_sharedjdbc(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc", jdbcQueryActivity.JdbcSharedConfig);
		}

		[Test]
		public void Should_Return_QueryStatement_is_select_Something(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("select CRNCY from T_EQUITY_PNO where ID_BB_UNIQUE= ?", jdbcQueryActivity.QueryStatement);
		}

		[Test]
		public void Should_Return_QueryOutputCachedSchemaColumns_is_CRNCY(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("CRNCY", jdbcQueryActivity.QueryOutputCachedSchemaColumns);
		}

		[Test]
		public void Should_Return_QueryOutputCachedSchemaDataTypes_is_12(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual (12, jdbcQueryActivity.QueryOutputCachedSchemaDataTypes);
		}

		[Test]
		public void Should_Return_QueryOutputCachedSchemaStatus_is_OptionalElement(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("OptionalElement", jdbcQueryActivity.QueryOutputCachedSchemaStatus);
		}

		[Test]
		public void Should_Return_two_QueryStatementParameter(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual (2, jdbcQueryActivity.QueryStatementParameters.Count);
		}

		[Test]
		public void Should_Return_QueryStatementParameter_is_named_IdBbUnique_and_type_VARCHAR(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("VARCHAR", jdbcQueryActivity.QueryStatementParameters["IdBbUnique"]);
		}

        [Test]
        public void Should_Return_2_input_parameters_of_for_the_activity(){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

            Assert.AreEqual (2, jdbcQueryActivity.Parameters.Count);
        }
	}
}

