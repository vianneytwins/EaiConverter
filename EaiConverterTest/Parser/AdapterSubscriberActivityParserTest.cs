using System;
using NUnit.Framework;
using EaiConverter.Parser;
using System.Xml.Linq;
using EaiConverter.Model;

namespace EaiConverterTest.Parser
{
	[TestFixture]
	public class AdapterSubscriberActivityParserTest
	{
		IActivityParser activityParser;

		XElement doc;

		AdapterSubscriberActivity activity;

		[SetUp]
		public void SetUp()
		{
			this.activityParser = new AdapterSubscriberActivityParser();
			var xml =
				@"<pd:activity name=""GetUndlCurrency"" xmlns:pd=""http://xmlns.tibco.com/bw/process/2003"" xmlns:xsl=""http://w3.org/1999/XSL/Transform"">
<pd:type>com.tibco.plugin.ae.AESubscriberActivity</pd:type>
<config>
	<FrequencyIndex>Minute</FrequencyIndex>
	<Frequency>false</Frequency>
	<TimeInterval>10</TimeInterval>
	<StartTime>86400000</StartTime>
</config>
</pd:activity>";
			doc = XElement.Parse(xml);

			this.activity = (AdapterSubscriberActivity) this.activityParser.Parse(doc);
		}

	}
}

