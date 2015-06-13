using System;
using NUnit.Framework;
using System.Xml.Linq;
using EaiConverter.Parser;
using EaiConverter.Model;

namespace EaiConverter
{
	[TestFixture]
	public class TibcoBWProcessLinqParserTest
	{

		TibcoBWProcessLinqParser tibcoBWProcessLinqParser;

		[SetUp]
		public void SetUp ()
		{
			tibcoBWProcessLinqParser = new TibcoBWProcessLinqParser();
		}
	

        [Test]
        public void Should_return_full_process_name_is_repertoire_dash_myProcessName ()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name></pd:ProcessDefinition>";
            var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("repertoire/myProcessName.process", tibcoBWProcess.FullProcessName);
        }

        [Test]
        public void Should_return_Xsd_Import ()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name>
            <xsd:import namespace=""http://www.tibco.com/ns/no_namspace_schema_location/XmlSchemas/DAI/PNO/XSD/RM3D.xsd"" schemaLocation=""/XmlSchemas/DAI/PNO/XSD/RM3D.xsd""/>
</pd:ProcessDefinition>"

                ;
            var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("http://www.tibco.com/ns/no_namspace_schema_location/XmlSchemas/DAI/PNO/XSD/RM3D.xsd", tibcoBWProcess.XsdImports[0].Namespace);
            Assert.AreEqual ("/XmlSchemas/DAI/PNO/XSD/RM3D.xsd", tibcoBWProcess.XsdImports[0].SchemaLocation);
        }

		[Test]
		public void Should_return_start_Activity_name_is_Start ()
		{
			string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:startName>Start</pd:startName><pd:startType/></pd:ProcessDefinition>";
			var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
			Assert.AreEqual ("Start", tibcoBWProcess.StartActivity.Name);
		}

		[Test]
		public void Should_return_start_Activity_with_one_Parameter_When_xsd_Contains_one_line ()
		{
			string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:startName>Start</pd:startName><pd:startType><xsd:element name=""adminID"" type=""xsd:string"" /></pd:startType></pd:ProcessDefinition>";
			var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
			Assert.AreEqual (1, tibcoBWProcess.StartActivity.Parameters.Count);
		}

        [Ignore]
        [Test]
        public void Should_return_start_Activity_with_referenceName_When_ref_is_assigned()
        {
            string xml = @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:pfx1=""http://testschema/2001/XMLSchema""><pd:name>repertoire/myProcessName.process</pd:name><pd:startName ref=""pfx1:myType"">Start</pd:startName></pd:ProcessDefinition>";
            var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
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
			var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
			Assert.AreEqual (1, tibcoBWProcess.Transitions.Count);
			Assert.AreEqual ("Start", tibcoBWProcess.Transitions[0].FromActivity);
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
			var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
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
            var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("variable1", tibcoBWProcess.ProcessVariables[0].Parameter.Name);
        }

        [Test]
        public void Should_return_One_Process_Variable_of_type_String ()
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
            var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual ("string", tibcoBWProcess.ProcessVariables[0].Parameter.Type);
        }
	}
}

