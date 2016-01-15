namespace EaiConverter.Test.Builder
{
    using System.Xml.Linq;

    using EaiConverter.Builder;
    using EaiConverter.Test.Utils;

    using NUnit.Framework;

    [TestFixture]
	public class XslBuilderTest
	{
        private XslBuilder xslBuilder;
   

        [SetUp]
        public void SetUp()
        {
            this.xslBuilder = new XslBuilder(new XpathBuilder());
        }

        [Test]
        public void Should_Return_1_Variable_assignement_with_String_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual (@"System.String FundName;
FundName = ""testvalue"";

",
 generateCode);
        }

        [Test]
        public void Should_Return_1_Variable_assignement_with_double_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundValue>
            <xsl:value-of select=""number('testvalue')""/>
        </FundValue>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual (@"Double? FundValue;
FundValue = TibcoXslHelper.ParseNumber(""testvalue"");

",
 generateCode);
        }

        [Test]
        public void Should_Return_1_Variable_assignement_with_DateTime_Type ()
        {
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundDate>
            <xsl:value-of select=""tib:parse-dateTime('MMM dd yyyy', $Mystuff/do)""/>
        </FundDate>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual (@"DateTime? FundDate;
FundDate = TibcoXslHelper.ParseDateTime(""MMM dd yyyy"", mystuff.do);

", generateCode);

        }

        [Test]
        public void Should_Return_2_Variable_assignement_with_string_Type (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
        <FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""'EVL'""/>
        </AdminID>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(this.xslBuilder.Build(doc.Nodes()));

            Assert.AreEqual(@"System.String FundName;
FundName = ""testvalue"";
System.String AdminID;
AdminID = ""EVL"";

",
 generateCode);
        }


        [Test]
        public void Should_Return_1_Variable_assignement_with_1_Level_Children (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<sqlParams>        
<FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
        <AdminID>
            <xsl:value-of select=""'EVL'""/>
        </AdminID>
</sqlParams>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build (doc.Nodes()));

            Assert.AreEqual (@"sqlParams sqlParams = new sqlParams();
sqlParams.FundName = ""testvalue"";
sqlParams.AdminID = ""EVL"";

",
 generateCode.ToString());
        }


        [Test]
        public void Should_Return_1_Variable_assignement_with_2_Levels_Of_Children (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<sqlParams>  
    <FundCompany>      
        <FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
    </FundCompany> 
    <AdminID>
        <xsl:value-of select=""'EVL'""/>
    </AdminID>
</sqlParams>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build (doc.Nodes()));

            Assert.AreEqual (@"sqlParams sqlParams = new sqlParams();
sqlParams.FundCompany = new FundCompany();
sqlParams.FundCompany.FundName = ""testvalue"";
sqlParams.AdminID = ""EVL"";

",
 generateCode.ToString());
        }

        [Test]
        public void Should_manage_if_condition(){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<xsl:if test=""true"">        
    <FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
</xsl:if>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual("if (true)\n{\n    System.String FundName;\n    FundName = \"testvalue\";\n}\n\n", generateCode);
        }

	    [Test]
		public void Should_manage_choose_condition_Outside(){
			var xml =
				@"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<FundName>
<xsl:choose>
    <xsl:when test=""true"">        
        
                <xsl:value-of select=""'testvalue1'""/>
    
    </xsl:when>
    <xsl:otherwise>        
       
                <xsl:value-of select=""'testvalue2'""/>
        
    </xsl:otherwise>
</xsl:choose>
</FundName>
</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

			string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual("System.String FundName;\nif (true)\n{\nFundName = \"testvalue1\";\n}\nelse\n{\nFundName = \"testvalue2\";\n}\n\n", generateCode);
		}

        [Test]
        public void Should_manage_Choose_condition(){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<xsl:choose>
    <xsl:when test=""true"">        
        <FundName>
                <xsl:value-of select=""'testvalue1'""/>
            </FundName>
    </xsl:when>
    <xsl:otherwise>        
        <FundName>
                <xsl:value-of select=""'testvalue2'""/>
            </FundName>
    </xsl:otherwise>
</xsl:choose>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual("if (true)\n{\n    System.String FundName;\n    FundName = \"testvalue1\";\n}\nelse\n{\n    System.String FundName;\n    FundName = \"testvalue2\";\n}\n\n", generateCode);
        }

        [Test]
        public void Should_manage_foreach(){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<FundCompany>
    <xsl:for-each select=""items"">       
            <FundName>
                    <xsl:value-of select=""'testvalue1'""/>
            </FundName>
    </xsl:for-each>
</FundCompany>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = xslBuilder.Build (doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual (
                "FundCompany FundCompany = new FundCompany();\nFundCompany.FundNames = new List<System.String>();\nforeach (var item in items)\n{\n    System.String FundName;\n    FundName = \"testvalue1\";\n    FundCompany.FundNames.Add(FundName);\n}\n\n", generateCode);
        }


        [Test]
        public void Should_DefineVariableReference_with_no_parent_When_Children(){
            var xml =
                @"
<FundCompany xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
                <FundName>
                    <xsl:value-of select=""'testvalue1'""/>
            </FundName>
</FundCompany>
";
            XElement doc = XElement.Parse(xml);

            string variableReference = xslBuilder.DefineVariableReference ((doc), null);

            Assert.AreEqual ("FundCompany", variableReference);
        }

        [Test]
        public void Should_DefineVariableReference_with_one_parent(){
            var xml =
                @"
                <FundName xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
                    <xsl:value-of select=""'testvalue1'""/>
            </FundName>
";
            XElement doc = XElement.Parse(xml);

            string variableReference = xslBuilder.DefineVariableReference ((doc), "FundCompany");

            Assert.AreEqual ("FundCompany.FundName", variableReference);
        }

        [Test]
        public void Should_manage_Null_value_when_no_Parent(){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                <FundName  xsi:nil=""true""/>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);
            var codeStatement = xslBuilder.Build (doc.Nodes());
            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual(
                @"object FundName = null;

", generateCode);
        }


        [Test]
        public void Should_Return_manage_null_value_when_parent_are_present (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
<sqlParams>        
<FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
        <AdminID xsi:nil=""true"">
        </AdminID>
</sqlParams>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build (doc.Nodes()));

            Assert.AreEqual (@"sqlParams sqlParams = new sqlParams();
sqlParams.FundName = ""testvalue"";
sqlParams.AdminID = null;

", generateCode.ToString());
        }

        [Test]
        public void Should_Return_manage_List_value (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
    <sqlParams>        
        <parameter>
            <xsl:value-of select=""'testvalue1'""/>
        </parameter>
        <parameter >
            <xsl:value-of select=""'testvalue2'""/>
        </parameter>
    </sqlParams>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build (doc.Nodes()));

            Assert.AreEqual (@"sqlParams sqlParams = new sqlParams();
List<System.String> tempparameterList = new List<System.String>();
System.String tempparameter1;
tempparameter1 = ""testvalue1"";
tempparameterList.Add(tempparameter1);
System.String tempparameter2;
tempparameter2 = ""testvalue2"";
tempparameterList.Add(tempparameter2);
sqlParams.parameter = tempparameterList.ToArray();

", generateCode);
        }

        [Test]
        public void Should_Return_manage_List_With_complex_value (){
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
    <sqlParams>        
        <parameter>
            <subParam>
                <xsl:value-of select=""'testvalue1'""/>
            </subParam>
        </parameter>
        <parameter >
            <subParam>
                <xsl:value-of select=""'testvalue2'""/>
            </subParam>
        </parameter>
    </sqlParams>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build (doc.Nodes()));

            Assert.AreEqual (@"sqlParams sqlParams = new sqlParams();
List<parameter> tempparameterList = new List<parameter>();
parameter tempparameter1 = new parameter();
tempparameter1.subParam = ""testvalue1"";
tempparameterList.Add(tempparameter1);
parameter tempparameter2 = new parameter();
tempparameter2.subParam = ""testvalue2"";
tempparameterList.Add(tempparameter2);
sqlParams.parameter = tempparameterList.ToArray();

", generateCode.ToString());
        }


		[Test]
		public void Should_manage_add_package_name_When_type_is_complex_and_when_its_inputed()
		{
			var packageName = "MyPackage.";
			var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
    <sqlParams>        
        <parameter>
            <xsl:value-of select=""'testvalue1'""/>
        </parameter>
    </sqlParams>  
</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

			var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build(packageName, doc.Nodes()));

            Assert.AreEqual("MyPackage.sqlParams sqlParams = new MyPackage.sqlParams();\nsqlParams.parameter = \"testvalue1\";\n\n", generateCode.ToString());
		}

		[Test]
		public void Should_manage_add_the_dot_At_the_end_Of_package_name_When_its_missing()
		{
			var packageName = "MyPackage";
			var xml =
				@"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
    <sqlParams>        
        <parameter>
            <xsl:value-of select=""'testvalue1'""/>
        </parameter>
    </sqlParams>  
</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

			var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build(packageName, doc.Nodes()));

            Assert.AreEqual ("MyPackage.sqlParams sqlParams = new MyPackage.sqlParams();\nsqlParams.parameter = \"testvalue1\";\n\n", generateCode.ToString());
		}

        [Test]
        public void Should_manage_safeType_when_they_are_child()
        {
            var packageName = "MyPackage";
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
    <logInfo>        
        <param>
            <xsl:value-of select=""'testvalue1'""/>
        </param>
    </logInfo>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build(packageName, doc.Nodes()));

            Assert.AreEqual("MyPackage.logInfo logInfo = new MyPackage.logInfo();\nlogInfo.param = \"testvalue1\";\n\n", generateCode.ToString());
        }

        [Test]
        public void Should_manage_safeType_when_they_are_child_and_List()
        {
            var packageName = "MyPackage";
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
    <logInfo>        
        <param>
            <xsl:value-of select=""'testvalue1'""/>
        </param>
        <param>
            <xsl:value-of select=""'testvalue2'""/>
        </param>
    </logInfo>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(this.xslBuilder.Build(packageName, doc.Nodes()));

            Assert.AreEqual(@"MyPackage.logInfo logInfo = new MyPackage.logInfo();
List<System.String> tempparamList = new List<System.String>();
System.String tempparam1;
tempparam1 = ""testvalue1"";
tempparamList.Add(tempparam1);
System.String tempparam2;
tempparam2 = ""testvalue2"";
tempparamList.Add(tempparam2);
logInfo.param = tempparamList.ToArray();

".RemoveWindowsReturnLineChar(), generateCode.ToString());
        }

        [Test]
        public void Should_manage_safeType_when_they_are_child_and_List_with_children()
        {
            var packageName = "MyPackage";
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
    <logInfo>        
        <param>
            <name>
                <xsl:value-of select=""'testvalue1'""/>
            </name>
        </param>
        <param>
            <name>
                <xsl:value-of select=""'testvalue2'""/>
            </name>
        </param>
    </logInfo>  
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var generateCode = TestCodeGeneratorUtils.GenerateCode(this.xslBuilder.Build(packageName, doc.Nodes()));

            Assert.AreEqual(@"MyPackage.logInfo logInfo = new MyPackage.logInfo();
List<logInfoParam> tempparamList = new List<logInfoParam>();
logInfoParam tempparam1 = new logInfoParam();
tempparam1.name = ""testvalue1"";
tempparamList.Add(tempparam1);
logInfoParam tempparam2 = new logInfoParam();
tempparam2.name = ""testvalue2"";
tempparamList.Add(tempparam2);;
logInfo.param = tempparamList.ToArray();

".RemoveWindowsReturnLineChar(), generateCode.ToString());
        }

		[Test]
		public void Should_manage_NOT_add_package_name_When_type_is_basic_and_when_its_inputed()
		{
			var packageName = "string.";
			var xml =
				@"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" >
        
        <param>
            <xsl:value-of select=""'testvalue1'""/>
        </param>

</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

			var generateCode = TestCodeGeneratorUtils.GenerateCode(xslBuilder.Build(packageName, doc.Nodes()));

            Assert.AreEqual ("System.String @param;\n@param = \"testvalue1\";\n\n", generateCode.ToString());
		}

		[Test]
		public void Should_Return_remove_prefix (){
			var xml =
				@"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx1=""http://www.SomeWhere.com"">
 <pfx1:logInfo>       
<FundName>
            <xsl:value-of select=""'testvalue'""/>
        </FundName>
</pfx1:logInfo>
</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

			var codeStatement = xslBuilder.Build (doc.Nodes());

			string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual ("logInfo logInfo = new logInfo();\nlogInfo.FundName = \"testvalue\";\n\n", generateCode);
		}

        [Test]
		public void Should_Manage_xsl_attribute()
        {
			var xml =
 @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx1=""http://www.SomeWhere.com"">
<NTMMessage>
    <xsl:attribute name=""version"">
        <xsl:value-of select=""'1.03'""/>
    </xsl:attribute>
</NTMMessage>
</pd:inputBindings>
";
			XElement doc = XElement.Parse(xml);

			var codeStatement = this.xslBuilder.Build(doc.Nodes());

			string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual("NTMMessage NTMMessage = new NTMMessage();\nNTMMessage.version = \"1.03\";\n\n", generateCode);
		}

        [Test]
        public void Should_Manage_xsl_attribute_null_value()
        {
            var xml =
 @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx1=""http://www.SomeWhere.com"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
<NTMMessage>
    <xsl:attribute name=""xsi:nil"">
        true
    </xsl:attribute>
</NTMMessage>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual("NTMMessage NTMMessage = new NTMMessage();\nNTMMessage = null;\n\n", generateCode);
        }

        [Test]
        public void Should_Manage_xsl_attribute_on_complextype()
        {
            var xml =
 @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx1=""http://www.SomeWhere.com"">
<NTMMessage>
    <NTMHeader>
        <xsl:attribute name=""version"">
            <xsl:value-of select=""'1.03'""/>
        </xsl:attribute>
    </NTMHeader>
</NTMMessage>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = this.xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual("NTMMessage NTMMessage = new NTMMessage();\nNTMMessage.NTMHeader = new NTMHeader();\nNTMMessage.NTMHeader.version = \"1.03\";\n\n", generateCode);
        }

        [Test]
        public void Should_manage_complex_type_embedded()
        {
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx1=""http://www.SomeWhere.com"">
 <logInfo>       
    <FundStruct>
        <FundName>
                <xsl:value-of select=""'testvalue'""/>
        </FundName>
    </FundStruct>
</logInfo>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual("logInfo logInfo = new logInfo();\nlogInfo.FundStruct = new FundStruct();\nlogInfo.FundStruct.FundName = \"testvalue\";\n\n", generateCode);
        }
        
        [Test]
        public void Should_manage_variable_tag()
        {
            var xml =
                @"<pd:inputBindings xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"" xmlns:pfx1=""http://www.SomeWhere.com"">
            <inputs>
                <xsl:variable name=""params"">
                    <xsl:value-of select=""'myvalue'""/>
                </xsl:variable>
                
                <Message>
                    <xsl:value-of select=""concat($Start/pfx3:logInfo/message, ' ', $params)""/>
                </Message>
                
            </inputs>
</pd:inputBindings>
";
            XElement doc = XElement.Parse(xml);

            var codeStatement = xslBuilder.Build(doc.Nodes());

            string generateCode = TestCodeGeneratorUtils.GenerateCode(codeStatement);
            Assert.AreEqual(@"inputs inputs = new inputs();
System.String @params = ""myvalue"";
inputs.Message = TibcoXslHelper.Concat(start_logInfo.message, "" "", @params);

".RemoveWindowsReturnLineChar(), generateCode.RemoveWindowsReturnLineChar());
        }
	}
}

