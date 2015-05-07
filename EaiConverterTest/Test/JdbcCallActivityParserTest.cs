using System;
using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter
{
	[TestFixture]
	public class JdbcCallActivityParserTest
	{
		JdbcQueryActivityParser jdbcQueryActivityParser;
		XElement doc;

		[SetUp]
		public void SetUp ()
		{
			jdbcQueryActivityParser = new JdbcQueryActivityParser ();
			var xml =
				@"<pd:activity name=""GetUndlCurrency"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"">
<pd:type>com.tibco.plugin.jdbc.JDBCCallActivity</pd:type>
<config>
<timeout>10</timeout>
<commit>false</commit>
<maxRows>100</maxRows>
<emptyStrAsNil>false</emptyStrAsNil>
<jdbcSharedConfig>/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc</jdbcSharedConfig>
<ProcedureName>LyxorSetEUTicker;1</ProcedureName>
<parameterTypes>
	<parameter>
		<colName>@Id_Bb_Unique</colName>
		<typeName>int</typeName>
		<dataType>4</dataType>
	</parameter>
	<parameter>
		<colName>@Id_Bb_Unique2</colName>
		<typeName>int</typeName>
		<dataType>4</dataType>
	</parameter>
</parameterTypes>
</config>
</pd:activity>";
			doc = XElement.Parse(xml);
		}

		[Test]
		public void Should_Return_Activity_Type_Is_JDBCQueryActivity (){
            JdbcQueryActivity jdbcQueryActivity = jdbcQueryActivityParser.Parse (doc);

            Assert.AreEqual ("com.tibco.plugin.jdbc.JDBCCallActivity", jdbcQueryActivity.Type.ToString());
		}


		[Test]
		public void Should_Return_QueryStatement_is_select_Something(){
            JdbcQueryActivity jdbcQueryActivity = jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("LyxorSetEUTicker", jdbcQueryActivity.QueryStatement);
		}

		[Test]
		public void Should_Return_QueryStatementParameter_is_named_IdBbUnique_and_type_VARCHAR(){
            JdbcQueryActivity jdbcQueryActivity = jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual ("int", jdbcQueryActivity.QueryStatementParameters["Id_Bb_Unique"]);
		}
	}
}

