namespace EaiConverter.Test.Parser
{
    using System.Xml.Linq;

    using EaiConverter.Model;
    using EaiConverter.Parser;
    using EaiConverter.Utils;

    using NUnit.Framework;

    [TestFixture]
	public class TibcoBWProcessLinqParserTest
	{
        private const string xmlWithGroup = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                            <pd:name>repertoire/myProcessName.process</pd:name>
<pd:group name=""groupName"">
<pd:type>com.tibco.pe.core.LoopGroup</pd:type>
<config>
    <pd:groupType>inputLoop</pd:groupType>
<pd:over>myCollection</pd:over>
<pd:iterationElementSlot>current</pd:iterationElementSlot>
<pd:indexSlot>index</pd:indexSlot>
<pd:accumulateOutput>false</pd:accumulateOutput>
</config>
    <pd:transition>
                                <pd:from>startgroup</pd:from><pd:to>endgroup</pd:to>
                                <pd:conditionType>always</pd:conditionType>
                            </pd:transition>
</pd:group>
<pd:transition>
                                <pd:from>Start</pd:from><pd:to>End</pd:to>
                                <pd:conditionType>always</pd:conditionType>
                            </pd:transition></pd:ProcessDefinition>";

        private TibcoBWProcessLinqParser tibcoBwProcessLinqParser;

        [SetUp]
		public void SetUp()
		{
			this.tibcoBwProcessLinqParser = new TibcoBWProcessLinqParser();
		}
	
        [Test]
        public void Should_return_full_process_name_is_repertoire_dash_myProcessName ()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name></pd:ProcessDefinition>";
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual("repertoire/myProcessName.process", tibcoBWProcess.FullProcessName);
        }

        [Test]
        public void Should_return_description ()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:label><pd:description>my description</pd:description></pd:label></pd:ProcessDefinition>";
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("my description", tibcoBWProcess.Description);
        }

        [Test]
        public void Should_return_Xsd_Import ()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name>
            <xsd:import namespace=""http://www.tibco.com/ns/no_namspace_schema_location/XmlSchemas/DAI/PNO/XSD/RM3D.xsd"" schemaLocation=""/XmlSchemas/DAI/PNO/XSD/RM3D.xsd""/>
</pd:ProcessDefinition>"

                ;
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("http://www.tibco.com/ns/no_namspace_schema_location/XmlSchemas/DAI/PNO/XSD/RM3D.xsd", tibcoBWProcess.XsdImports[0].Namespace);
            Assert.AreEqual ("/XmlSchemas/DAI/PNO/XSD/RM3D.xsd", tibcoBWProcess.XsdImports[0].SchemaLocation);
        }

		[Test]
		public void Should_return_start_Activity_name_is_Start ()
		{
			string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:startName>Start</pd:startName><pd:startType/></pd:ProcessDefinition>";
			var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
			Assert.AreEqual ("Start", tibcoBWProcess.StartActivity.Name);
		}

        [Test]
        public void Should_return_start_Activity_name_When_StartType_is_not_present()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:startName>Start</pd:startName></pd:ProcessDefinition>";
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual("Start", tibcoBWProcess.StartActivity.Name);
        }

		[Test]
		public void Should_return_start_Activity_with_StartType_Defined_in_the_Process_xml ()
		{
			string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:startName>Start</pd:startName><pd:startType><xsd:element name=""adminID"" type=""xsd:string"" /></pd:startType></pd:ProcessDefinition>";
			var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("adminID", tibcoBWProcess.StartActivity.Parameters[0].Name);
		}
            
        [Test]
        public void Should_return_start_Activity_with_StartType_Defined_in_reference()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:pfx1=""http://testschema/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:startName>Start</pd:startName><pd:startType ref=""pfx1:myType""/></pd:ProcessDefinition>";
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("myType", tibcoBWProcess.StartActivity.Parameters[0].Name);
        }

		[Test]
		public void Should_return_1_transition_when_Only_Start_and_End_are_defined_with_condition_type_always ()
		{
			string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
							<pd:name>repertoire/myProcessName.process</pd:name><pd:transition>
								<pd:from>Start</pd:from><pd:to>End</pd:to>
								<pd:conditionType>always</pd:conditionType>
							</pd:transition></pd:ProcessDefinition>";
			var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
			Assert.AreEqual (1, tibcoBWProcess.Transitions.Count);
			Assert.AreEqual ("Start", tibcoBWProcess.Transitions[0].FromActivity);
			Assert.AreEqual ("End", tibcoBWProcess.Transitions[0].ToActivity);
			Assert.AreEqual (ConditionType.always, tibcoBWProcess.Transitions[0].ConditionType);
		}
        
        [Test]
		public void Should_return_remove_dot_or_space_in_activityname_in_transition_when_present()
		{
			string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
							<pd:name>repertoire/myProcessName.process</pd:name><pd:transition>
								<pd:from>Start.begin</pd:from><pd:to>End</pd:to>
								<pd:conditionType>always</pd:conditionType>
							</pd:transition></pd:ProcessDefinition>";
			var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
			Assert.AreEqual (1, tibcoBWProcess.Transitions.Count);
			Assert.AreEqual ("Start_begin", tibcoBWProcess.Transitions[0].FromActivity);
			Assert.AreEqual ("End", tibcoBWProcess.Transitions[0].ToActivity);
			Assert.AreEqual (ConditionType.always, tibcoBWProcess.Transitions[0].ConditionType);
		}
			
		[Test]
		public void Should_return_1_activity_when_Only_One_is_defined ()
		{
			string xml =
				@"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
					<pd:name>repertoire/myProcessName.process</pd:name>
					<pd:activity name = ""activity2"">
						<pd:type>NotHandleYet</pd:type>
					</pd:activity>
				</pd:ProcessDefinition>";
			var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
			Assert.AreEqual (1, tibcoBWProcess.Activities.Count);
			Assert.AreEqual ("activity2", tibcoBWProcess.Activities[0].Name);
            Assert.AreEqual ("NotHandleYet", tibcoBWProcess.Activities[0].Type.ToString());
		}

        [Test]
        public void Should_return_One_Process_Variable_named_variable1 ()
        {
            string xml =
                @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                    <pd:name>repertoire/myProcessName.process</pd:name>
                    <pd:processVariables>
                        <variable1>
<xsd:element name=""UdlCcy"" type=""xsd:string""/>
                        </variable1>
                    </pd:processVariables>
                </pd:ProcessDefinition>";
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("variable1", tibcoBWProcess.ProcessVariables[0].Parameter.Name);
        }

        [Test]
        public void Should_return_One_Process_Variable_of_type_String()
        {
            string xml =
                @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                    <pd:name>repertoire/myProcessName.process</pd:name>
                    <pd:processVariables>
                        <variable1>
                            <xsd:element name=""UdlCcy"" type=""xsd:string""/>
                        </variable1>
                    </pd:processVariables>
                </pd:ProcessDefinition>";
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual (CSharpTypeConstant.SystemString, tibcoBWProcess.ProcessVariables[0].Parameter.Type);
        }

        [Test]
        public void Should_return_1_transition_even_when_there_is_a_group()
        {

            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xmlWithGroup));
            Assert.AreEqual (1, tibcoBWProcess.Transitions.Count);
        }

        [Test]
        public void Should_return_1_transition_in_the_group()
        {
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xmlWithGroup));
            Assert.AreEqual (1, ((GroupActivity)tibcoBWProcess.Activities[0]).Transitions.Count);
        }

        [Test]
        public void Should_return_group_config()
        {
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xmlWithGroup));
            Assert.AreEqual("INPUTLOOP", ((GroupActivity)tibcoBWProcess.Activities[0]).GroupType.ToString());
            Assert.AreEqual("myCollection", ((GroupActivity)tibcoBWProcess.Activities[0]).Over);
            Assert.AreEqual("current", ((GroupActivity)tibcoBWProcess.Activities[0]).IterationElementSlot);
            Assert.AreEqual("index", ((GroupActivity)tibcoBWProcess.Activities[0]).IndexSlot);
        }

        [Test]
        public void Should_return_parse_starter_Activity()
        {
            string xml =
                @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                    <pd:name>repertoire/myProcessName.process</pd:name>
                    <pd:starter name=""Rendezvous"">
                        <pd:type>NotHandleYet</pd:type>
                    </pd:starter>
                </pd:ProcessDefinition>";
            var tibcoBWProcess = this.tibcoBwProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual("NotHandleYet", tibcoBWProcess.StarterActivity.Type.ToString());
        }
	}
}

