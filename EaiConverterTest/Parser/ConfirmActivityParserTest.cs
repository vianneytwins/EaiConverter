using EaiConverter.Parser;
using System.Xml.Linq;
using NUnit.Framework;
using EaiConverter.Model;

namespace EaiConverter.Test.Parser
{
    [TestFixture]
	public class ConfirmActivityParserTest
    {
		ConfirmActivityParser confirmActivityParser;
        XElement doc;

        [SetUp]
        public void SetUp ()
        {
	
			confirmActivityParser = new ConfirmActivityParser ();
            var xml =
                @"<pd:activity name=""Mappe Equity"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.pe.core.ConfirmActivity</pd:type>
<config>
<ConfirmEvent>Rendez vous suscriber</ConfirmEvent>
</config>
<pd:inputBindings/>
</pd:activity>";
            doc = XElement.Parse(xml);
        }

        [Test]
        public void Should_Return_Activity_Type_Is_ConfirmActivity (){
			ConfirmActivity confirmActivity = (ConfirmActivity) confirmActivityParser.Parse (doc);

			Assert.AreEqual (ActivityType.ConfirmActivityType.ToString(), confirmActivity.Type.ToString());
        }


        [Test]
        public void Should_Return_subscriber_to_confirm_activity_name_in_Element_config(){
			var confirmActivity = (ConfirmActivity) confirmActivityParser.Parse (doc);

			Assert.AreEqual ("Rendez_vous_suscriber", confirmActivity.ActivityNameToConfirm);
        }
    
    }
}

