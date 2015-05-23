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
        public void Should_return_One_Process_Variable ()
        {
            string xml =
                @"<pd:ProcessDefinition xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                    <pd:name>repertoire/myProcessName.process</pd:name>
                    <pd:processVariables>
                        <var>
<xsd:element name=""UdlCcy"" type=""xsd:string""/>
                        </var>
                    </pd:processVariables>
                </pd:ProcessDefinition>";
            var tibcoBWProcess = tibcoBWProcessLinqParser.Parse(XElement.Parse(xml));
            Assert.AreEqual (1, tibcoBWProcess.ProcessVariables.Count);
            Assert.AreEqual ("var", tibcoBWProcess.ProcessVariables[0].Name);
        }
	}
}

