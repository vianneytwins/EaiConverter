namespace EaiConverter.Test.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser;

    using NUnit.Framework;

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
                @"<pd:activity name=""GetUndlCurrency"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.jdbc.JDBCCallActivity</pd:type>
<config>
<timeout>10</timeout>
<commit>false</commit>
<maxRows>100</maxRows>
<emptyStrAsNil>false</emptyStrAsNil>
<jdbcSharedConfig>/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc</jdbcSharedConfig>
<ProcedureName>MySetEUTicker;1</ProcedureName>
<parameterTypes>
	<parameter>
		<colName>@Id_Bb_Unique</colName>
		<typeName>int</typeName>
		<dataType>4</dataType>
        <colType>1</colType>
	</parameter>
	<parameter>
		<colName>@Id_Bb_Unique2</colName>
		<typeName>int</typeName>
		<dataType>4</dataType>
        <colType>1</colType>
	</parameter>
	<parameter>
		<colName>@Id_Bb_Unique3</colName>
		<typeName>int</typeName>
		<dataType>4</dataType>
        <colType>4</colType>
	</parameter>
</parameterTypes>
</config>
<pd:inputBindings>
     <inputs>
        <inputSet>
        <FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>
 </inputSet>
                </inputs>
               
</pd:inputBindings>
</pd:activity>";
			doc = XElement.Parse(xml);
		}

		[Test]
		public void Should_Return_Activity_Type_Is_JDBCQueryActivity (){
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

            Assert.AreEqual("com.tibco.plugin.jdbc.JDBCCallActivity", jdbcQueryActivity.Type.ToString());
		}


		[Test]
		public void Should_Return_QueryStatement_is_select_Something(){
            var jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual("MySetEUTicker", jdbcQueryActivity.QueryStatement);
		}

		[Test]
		public void Should_Return_QueryStatementParameter_is_named_IdBbUnique_and_type_VARCHAR(){
            var jdbcQueryActivity = (JdbcQueryActivity) jdbcQueryActivityParser.Parse (doc);

			Assert.AreEqual("int", jdbcQueryActivity.QueryStatementParameters["Id_Bb_Unique"]);
		}

        [Test]
        public void Should_Return_QueryOutputStatementParameter()
        {
            var jdbcQueryActivity = (JdbcQueryActivity)jdbcQueryActivityParser.Parse(doc);

            Assert.AreEqual("VARCHAR", jdbcQueryActivity.QueryOutputStatementParameters[0].Type);
            Assert.AreEqual("int", jdbcQueryActivity.QueryOutputStatementParameters[1].Type);
        }

	    [Test]
	    public void Should_Return_QueryStatement_When_no_parameters()
	    {
            var xml =
    @"<pd:activity name=""GetUndlCurrency"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.jdbc.JDBCCallActivity</pd:type>
<config>
<timeout>10</timeout>
<commit>false</commit>
<maxRows>100</maxRows>
<emptyStrAsNil>false</emptyStrAsNil>
<jdbcSharedConfig>/Configuration/DAI/PNO/JDBC/JDBCIntegration.sharedjdbc</jdbcSharedConfig>
<ProcedureName>LyxorSetEUTicker</ProcedureName>
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
<pd:inputBindings>
      <inputs>
                <inputSet>
        <FundName>
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>
</inputSet>
       </inputs>
                
</pd:inputBindings>
</pd:activity>";
            doc = XElement.Parse(xml);

            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity)jdbcQueryActivityParser.Parse(doc);

            Assert.AreEqual("LyxorSetEUTicker", jdbcQueryActivity.QueryStatement);
	    }

        [Ignore]
        [Test]
        public void Should_Return_input_bindings_inside_tags()
        {
            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity)jdbcQueryActivityParser.Parse(doc);
            string expected = @"<FundName xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
            <xsl:value-of select=""testvalue""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""EVL""/>
        </AdminID>";
            Assert.AreEqual(expected, jdbcQueryActivity.InputBindings);
        }


	    [Test]
	    public void Should_Return_QueryOutPutElement_When_Resultset_is_present()
	    {
	        string specialXml =
	            @"<pd:activity name=""JDBC Call Procedure"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"">
  <pd:type>com.tibco.plugin.jdbc.JDBCCallActivity</pd:type>
  <pd:resourceType>ae.activities.JDBCCallActivity</pd:resourceType>
  <pd:x>260</pd:x>
  <pd:y>233</pd:y>
  <config>
    <timeout>10</timeout>
    <maxRows>100</maxRows>
    <emptyStrAsNil>false</emptyStrAsNil>
    <jdbcSharedConfig>/Configuration/DAI/PNO/JDBC/JDBCPanoramaPanorama.sharedjdbc</jdbcSharedConfig>
    <ProcedureName>LyxorGetEmptyIssuer;1</ProcedureName>
    <useSchema>true</useSchema>
    <oraObjects />
    <oraTables />
    <ResultSets>
      <ResultSet>
        <QueryOutputCachedSchemaColumns>IssueID</QueryOutputCachedSchemaColumns>
        <QueryOutputCachedSchemaDataTypes>1</QueryOutputCachedSchemaDataTypes>
        <QueryOutputCachedSchemaStatus>RequiredElement</QueryOutputCachedSchemaStatus>
        <QueryOutputCachedSchemaColumns>LONG_COMP_NAME</QueryOutputCachedSchemaColumns>
        <QueryOutputCachedSchemaDataTypes>12</QueryOutputCachedSchemaDataTypes>
        <QueryOutputCachedSchemaStatus>NillableElement</QueryOutputCachedSchemaStatus>
        <QueryOutputCachedSchemaColumns>INDUSTRY_SECTOR</QueryOutputCachedSchemaColumns>
        <QueryOutputCachedSchemaDataTypes>12</QueryOutputCachedSchemaDataTypes>
        <QueryOutputCachedSchemaStatus>NillableElement</QueryOutputCachedSchemaStatus>
        <QueryOutputCachedSchemaColumns>INDUSTRY_GROUP</QueryOutputCachedSchemaColumns>
        <QueryOutputCachedSchemaDataTypes>12</QueryOutputCachedSchemaDataTypes>
        <QueryOutputCachedSchemaStatus>NillableElement</QueryOutputCachedSchemaStatus>
        <QueryOutputCachedSchemaColumns>INDUSTRY_SUBGROUP</QueryOutputCachedSchemaColumns>
        <QueryOutputCachedSchemaDataTypes>12</QueryOutputCachedSchemaDataTypes>
        <QueryOutputCachedSchemaStatus>NillableElement</QueryOutputCachedSchemaStatus>
        <QueryOutputCachedSchemaColumns>CNTRY_OF_DOMICILE</QueryOutputCachedSchemaColumns>
        <QueryOutputCachedSchemaDataTypes>12</QueryOutputCachedSchemaDataTypes>
        <QueryOutputCachedSchemaStatus>NillableElement</QueryOutputCachedSchemaStatus>
      </ResultSet>
    </ResultSets>
    <parameterTypes>
      <parameter>
        <colName>@RETURN_VALUE</colName>
        <colType>5</colType>
        <dataType>4</dataType>
        <typeName>int</typeName>
      </parameter>
    </parameterTypes>
  </config>
  <pd:inputBindings>
    <inputs />
  </pd:inputBindings>
</pd:activity>";

            doc = XElement.Parse(specialXml);

            JdbcQueryActivity jdbcQueryActivity = (JdbcQueryActivity)jdbcQueryActivityParser.Parse(doc);

            Assert.AreEqual("IssueID", jdbcQueryActivity.QueryOutputStatementParameters[2].Name);
            Assert.AreEqual("1", jdbcQueryActivity.QueryOutputStatementParameters[2].Type);
	    }
	}
}

